using System;
using RollicGames.Math.Runtime.Model;
using RollicGames.ColorBlockJamClone.Level.Runtime.Model;
using RollicGames.ColorBlockJamClone.Level.Runtime.View;
using Zenject;

namespace RollicGames.ColorBlockJamClone.Level.Runtime.Controller
{
    public class LevelFlowController : IInitializable, IDisposable
    {
        [Inject] private readonly LevelModel _levelModel;
        [Inject] private readonly LevelGridModel _levelGridModel;
        [Inject] private readonly ILevelInputView _levelInputView;
        [Inject] private readonly ILevelMoveController _moveController;

        private int2 _lastTouchedGridPosition;
        private LevelBlockObjectModel _draggingObjectModel = null;

        public void Initialize()
        {
            _levelInputView.OnBlockClickStarted += OnBlockClicked;
            _levelInputView.OnTouchMoved += OnTouchMoved;
            _levelInputView.OnTouchEnded += OnTouchEnded;
        }

        private void TryToMoveBlock(int2 gridPosition)
        {
            var distance = gridPosition - _lastTouchedGridPosition;
            if(distance.X == 0 && distance.Y == 0) return;

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

            // TODO : Check for absorbing

            if(!isMoved && hasSecondDirection)
            {
                isMoved = _moveController.Move(_draggingObjectModel.Id, secondDirection);
                if(isMoved)
                {
                    _lastTouchedGridPosition += secondDirection.GetDirectionVector();
                }

                // TODO : Check for absorbing
            }
        }

        private void OnBlockClicked(int blockId, int2 startingTouchGridPosition)
        {
            if(_levelModel.IsLevelPaused) return;

            if(!_levelGridModel.Blocks.ObjectsById.TryGetValue(blockId, out var blockModel)) return;
            if(!blockModel.IsActive) return;

            // TODO selected block view
            _lastTouchedGridPosition = startingTouchGridPosition;
            _draggingObjectModel = blockModel;
        }

        private void OnTouchMoved(int2 gridPosition)
        {
            if(_levelModel.IsLevelPaused || _draggingObjectModel == null) return;

            TryToMoveBlock(gridPosition);
        }

        private void OnTouchEnded()
        {
            if(_draggingObjectModel == null) return;

            // TODO deselected block view
            _draggingObjectModel = null;
        }

        public void Dispose()
        {
            _levelInputView.OnBlockClickStarted -= OnBlockClicked;
            _levelInputView.OnTouchMoved -= OnTouchMoved;
            _levelInputView.OnTouchEnded -= OnTouchEnded;
        }
    }
}