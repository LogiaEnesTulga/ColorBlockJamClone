using RollicGames.Math.Runtime.Model;
using RollicGames.ColorBlockJamClone.Level.Runtime.Model;
using Zenject;
using RollicGames.ColorBlockJamClone.Level.Runtime.View;

namespace RollicGames.ColorBlockJamClone.Level.Runtime.Controller
{
    public interface ILevelMoveController
    {
        bool Move(int id, LevelDirection direction);
    }
    
    public class LevelMoveController : ILevelMoveController
    {
        [Inject] private readonly LevelGridModel _gridModel;
        [Inject] private readonly ILevelGridView _gridView;

        public bool Move(int id, LevelDirection direction)
        {
            if(!_gridModel.Blocks.ObjectsById.TryGetValue(id, out var blockModel)) return false;

            var directionVector = direction.GetDirectionVector();
            if(!CanMoveToDirection(blockModel, directionVector)) return false;
            
            MoveBlockToDirection(blockModel, directionVector);
            return true;
        }

        private bool CanMoveToDirection(LevelBlockObjectModel blockModel, int2 directionVector)
        {
            foreach(var localPosition in blockModel.BlocksLocalPositions)
            {
                var gridPosition = blockModel.GridPosition + localPosition;
                
                if(!_gridModel.Cells.Contains(gridPosition + directionVector)) return false;
                if(_gridModel.Blocks.ObjectsByPosition.TryGetValue(gridPosition + directionVector, out var collidedBlockModel))
                {
                    if(blockModel != collidedBlockModel) return false;
                }
            }

            return true;
        }

        private void MoveBlockToDirection(LevelBlockObjectModel blockModel, int2 directionVector)
        {
            foreach(var localPosition in blockModel.BlocksLocalPositions)
            {
                var gridPosition = blockModel.GridPosition + localPosition;
                _gridModel.Blocks.ObjectsByPosition.Remove(gridPosition);
            }

            var newPosition = blockModel.GridPosition + directionVector;
            blockModel.SetPosition(newPosition);
            foreach(var localPosition in blockModel.BlocksLocalPositions)
            {
                var gridPosition = blockModel.GridPosition + localPosition;
                _gridModel.Blocks.ObjectsByPosition.Add(gridPosition, blockModel);
            }

            _gridView.MoveBlock(blockModel.Id, newPosition);
        }
    }
}