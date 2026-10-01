using System;
using RollicGames.Math.Runtime.Model;
using RollicGames.ColorBlockJamClone.Level.Runtime.Model;
using RollicGames.ColorBlockJamClone.Level.Runtime.View;
using Zenject;

namespace RollicGames.ColorBlockJamClone.Level.Runtime.Controller
{
    public interface ILevelFlowController
    {
        event Action OnLevelFailed;

        void PrepareForReuse();
    }

    public class LevelFlowController : ILevelFlowController, IInitializable, IDisposable
    {
        [Inject] private readonly LevelModel _levelModel;
        [Inject] private readonly LevelGridModel _levelGridModel;
        [Inject] private readonly ILevelGridView _levelGridView;
        [Inject] private readonly ILevelInputView _levelInputView;
        [Inject] private readonly ILevelMoveController _moveController;
        [Inject] private readonly ILevelDoorsController _doorsController;
        [Inject] private readonly ILevelTimerController _timerController;

        private float2 _grabWorldPosition;
        private int2 _lastTouchedGridPosition;
        private int2 _lastDoorCheckPosition;
        private LevelBlockObjectModel _draggingObjectModel = null;

        public event Action OnLevelFailed;

        public void Initialize()
        {
            _levelInputView.OnBlockClickStarted += OnBlockClicked;
            _levelInputView.OnTouchMoved += OnTouchMoved;
            _levelInputView.OnTouchEnded += OnTouchEnded;
            _timerController.OnLevelTimerExpired += OnLevelTimerExpired;
        }

        public void PrepareForReuse()
        {
            CancelDragging();
        }

        private void OnLevelTimerExpired()
        {
            CancelDragging();

            OnLevelFailed?.Invoke();
        }

        private void CancelDragging()
        {
            if(_draggingObjectModel == null) return;

            _levelGridView.EndDragBlock(_draggingObjectModel.Id);
            _draggingObjectModel = null;
        }

        private async void TryToMoveBlock(int2 gridPosition)
        {
            var distance = gridPosition - _lastTouchedGridPosition;
            if(distance.X != 0 || distance.Y != 0)
            {
                var absX = System.Math.Abs(distance.X);
                var absY = System.Math.Abs(distance.Y);

                var xBigger = absX >= absY;
                var firstDirection = xBigger ? 
                                    (distance.X > 0 ? LevelDirection.Right : LevelDirection.Left) :
                                    (distance.Y > 0 ? LevelDirection.Down : LevelDirection.Up);

                var hasSecondDirection = xBigger ? absY > 0 : absX > 0;
                var secondDirection = xBigger ? 
                                    (distance.Y > 0 ? LevelDirection.Down : LevelDirection.Up) :
                                    (distance.X > 0 ? LevelDirection.Right : LevelDirection.Left);
                                    
                                    
                var isMoved = _moveController.Move(_draggingObjectModel.Id, firstDirection);
                if(isMoved)
                {
                    _lastTouchedGridPosition += firstDirection.GetDirectionVector();
                }

                if(!isMoved && hasSecondDirection)
                {
                    isMoved = _moveController.Move(_draggingObjectModel.Id, secondDirection);
                    if(isMoved)
                    {
                        _lastTouchedGridPosition += secondDirection.GetDirectionVector();
                    }
                }
            }

            if(_draggingObjectModel.GridPosition == _lastDoorCheckPosition) return;

            _lastDoorCheckPosition = _draggingObjectModel.GridPosition;
            if(!_doorsController.TryToAbsorbBlock(_draggingObjectModel.Id, out var absorbAnimationTask)) return;

            // TODO : Check for level is ended here?
            CancelDragging();

            // TODO : update here
            var isLevelEnded = false;
            if(isLevelEnded)
            {
                await absorbAnimationTask;
                // TODO : Open Win Level Popup
            }
        }

        private void OnBlockClicked(int blockId, int2 startingTouchGridPosition)
        {
            if(_levelModel.IsLevelPaused || _levelModel.IsLevelFinished) return;

            if(!_levelGridModel.Blocks.ObjectsById.TryGetValue(blockId, out var blockModel)) return;
            if(!blockModel.IsActive) return;

            _levelModel.IsLevelStarted = true;

            _lastTouchedGridPosition = startingTouchGridPosition;
            _draggingObjectModel = blockModel;
            _grabWorldPosition = _levelInputView.TouchWorldPosition;
            _levelGridView.BeginDragBlock(blockId);
            _lastDoorCheckPosition = new int2(-999, -999);
        }

        private void OnTouchMoved(int2 gridPosition)
        {
            if(_levelModel.IsLevelPaused || _draggingObjectModel == null) return;

            TryToMoveBlock(gridPosition);
            if(_draggingObjectModel == null || _levelModel.IsLevelFinished) return;

            var id = _draggingObjectModel.Id;
            _levelGridView.DragBlock(id,
                _levelInputView.TouchWorldPosition - _grabWorldPosition,
                _moveController.CanMove(id, LevelDirection.Left),
                _moveController.CanMove(id, LevelDirection.Right),
                _moveController.CanMove(id, LevelDirection.Up),
                _moveController.CanMove(id, LevelDirection.Down));
        }

        private void OnTouchEnded()
        {
            CancelDragging();
        }

        public void Dispose()
        {
            _levelInputView.OnBlockClickStarted -= OnBlockClicked;
            _levelInputView.OnTouchMoved -= OnTouchMoved;
            _levelInputView.OnTouchEnded -= OnTouchEnded;
        }
    }
}