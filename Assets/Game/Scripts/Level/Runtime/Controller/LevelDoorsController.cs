using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using RollicGames.Math.Runtime.Model;
using RollicGames.ColorBlockJamClone.Level.Runtime.Model;
using RollicGames.ColorBlockJamClone.Level.Runtime.View;
using Zenject;

namespace RollicGames.ColorBlockJamClone.Level.Runtime.Controller
{
    public interface ILevelDoorsController
    {
        bool TryToAbsorbBlock(int blockId, out UniTask absorbAnimationTask);
    }

    public class LevelDoorsController : ILevelDoorsController
    {
        private static readonly List<LevelDirection> CheckDirections = new List<LevelDirection>(){
            LevelDirection.Up,
            LevelDirection.Down,
            LevelDirection.Right,
            LevelDirection.Left,
        };

        [Inject] private readonly LevelModel _levelModel;
        [Inject] private readonly LevelGridModel _gridModel;
        [Inject] private readonly ILevelGridView _gridView;

        public bool TryToAbsorbBlock(int blockId, out UniTask absorbAnimationTask)
        {
            absorbAnimationTask = UniTask.CompletedTask;
            if(!_gridModel.Blocks.ObjectsById.TryGetValue(blockId, out var blockModel)) return false;

            foreach(var localPosition in blockModel.BlocksLocalPositions)
            {
                var gridPosition = blockModel.GridPosition + localPosition;

                foreach(var checkDirection in CheckDirections)
                {
                    if(!CanBlockAbsorbableFromDirection(blockModel, gridPosition, checkDirection, out var doorModel)) continue;

                    absorbAnimationTask = AbsorbBlockWithDoor(blockModel, doorModel);
                    return true;
                }
            }

            return false;
        }

        private bool CanBlockAbsorbableFromDirection(LevelBlockObjectModel blockModel, int2 gridPosition, LevelDirection direction, out LevelDoorObjectModel doorObjectModel)
        {
            var directionVector = direction.GetDirectionVector();
            doorObjectModel = null;
            if(!_gridModel.Doors.ObjectsByPosition.TryGetValue(gridPosition + directionVector, out var doorModel)) return false;
            if(blockModel.Color != doorModel.Color || direction != doorModel.AbsorbDirection) return false;

            foreach(var localPosition in blockModel.BlocksLocalPositions)
            {
                var checkPosition = blockModel.GridPosition + localPosition + directionVector;
                while(true)
                {
                    if(_gridModel.Doors.ObjectsByPosition.TryGetValue(checkPosition, out var hitDoor))
                    {
                        if(hitDoor == doorModel) break;

                        return false;
                    }

                    if(_gridModel.Blocks.ObjectsByPosition.TryGetValue(checkPosition, out var hitBlock))
                    {
                        if(hitBlock == blockModel) break;

                        return false;
                    }

                    if(!_gridModel.Cells.Contains(checkPosition)) return false;
                    
                    checkPosition += directionVector;
                }
            }

            doorObjectModel = doorModel;
            return true;
        }

        private UniTask AbsorbBlockWithDoor(LevelBlockObjectModel blockModel, LevelDoorObjectModel doorModel)
        {
            blockModel.SetActive(false);
            var blockLengthInDirection = 1;
            var directionVector = doorModel.AbsorbDirection.GetDirectionVector();

            foreach(var localPosition in blockModel.BlocksLocalPositions)
            {
                var gridPosition = blockModel.GridPosition + localPosition;
                _gridModel.Blocks.ObjectsByPosition.Remove(gridPosition);

                var checkingValue = directionVector.X == 0 ? localPosition.Y : localPosition.X;
                if(checkingValue >= blockLengthInDirection)
                {
                    blockLengthInDirection = checkingValue + 1;
                }
            }

            var lastPosition = blockModel.GridPosition + (directionVector * (blockLengthInDirection + 1));
            return _gridView.StartAbsorbingBlockAnimation(doorModel.Id, blockModel.Id, lastPosition, doorModel.AbsorbDirection, _levelModel.LevelCancellationToken);
        }
    }
}