using System.Collections.Generic;
using System.Linq;
using RollicGames.Math.Runtime.Model;
using RollicGames.ColorBlockJamClone.Level.Runtime.Model;

namespace RollicGames.ColorBlockJamClone.Level.Editor
{
    public class LevelBlockEditor : ILevelObjectEditor
    {
        private readonly LevelGridModel _grid;

        public LevelObjectType ObjectType => LevelObjectType.Block;

        public LevelBlockEditor(LevelGridModel grid)
        {
            _grid = grid;
        }

        public bool CanDrawPiece(int2 position, IReadOnlyCollection<int2> drawnPositions)
        {
            if(drawnPositions.Contains(position) || !IsFreeCell(position, null)) return false;

            return drawnPositions.Count == 0 || LevelGridQueries.IsNeighbourOfAny(position, drawnPositions);
        }

        public LevelBlockObjectModel Create(IReadOnlyCollection<int2> positions, LevelObjectColor color)
        {
            var block = new LevelBlockObjectModel();
            block.SetId(LevelGridQueries.GetNextId(_grid.Blocks));
            Initialize(block, positions, color);
            LevelGridRegistry.AddBlock(_grid, block);
            return block;
        }

        public IReadOnlyList<int2> GetPositions(LevelObjectModel levelObject)
        {
            return LevelGridQueries.GetBlockPositions((LevelBlockObjectModel)levelObject);
        }

        public LevelObjectColor GetColor(LevelObjectModel levelObject)
        {
            return ((LevelBlockObjectModel)levelObject).Color;
        }

        public void SetColor(LevelObjectModel levelObject, LevelObjectColor color)
        {
            var block = (LevelBlockObjectModel)levelObject;
            Rebuild(block, LevelGridQueries.GetBlockPositions(block), color);
        }

        public void Remove(LevelObjectModel levelObject)
        {
            LevelGridRegistry.RemoveBlock(_grid, (LevelBlockObjectModel)levelObject);
        }

        public bool CanRemovePiece(LevelObjectModel levelObject, int2 position)
        {
            return LevelGridQueries.CanRemoveKeepingConnected(position, LevelGridQueries.GetBlockPositions((LevelBlockObjectModel)levelObject));
        }

        public void RemovePiece(LevelObjectModel levelObject, int2 position)
        {
            var block = (LevelBlockObjectModel)levelObject;
            var positions = LevelGridQueries.GetBlockPositions(block);
            positions.Remove(position);

            if(positions.Count == 0)
            {
                Remove(block);
                return;
            }

            Rebuild(block, positions, block.Color);
        }

        public bool CanMove(LevelObjectModel levelObject, int2 targetOrigin)
        {
            var block = (LevelBlockObjectModel)levelObject;
            return block.BlocksLocalPositions.All(localPosition => IsFreeCell(targetOrigin + localPosition, block));
        }

        public void Move(LevelObjectModel levelObject, int2 targetOrigin)
        {
            var block = (LevelBlockObjectModel)levelObject;
            var positions = block.BlocksLocalPositions.Select(localPosition => targetOrigin + localPosition).ToList();
            Rebuild(block, positions, block.Color);
        }

        private bool IsFreeCell(int2 position, LevelBlockObjectModel ignoredBlock)
        {
            if(!_grid.Cells.Contains(position)) return false;

            return !_grid.Blocks.ObjectsByPosition.TryGetValue(position, out var block) || block == ignoredBlock;
        }

        private void Rebuild(LevelBlockObjectModel block, IReadOnlyCollection<int2> positions, LevelObjectColor color)
        {
            LevelGridRegistry.RemoveBlock(_grid, block);
            block.OnReturnedToPool();
            Initialize(block, positions, color);
            LevelGridRegistry.AddBlock(_grid, block);
        }

        private static void Initialize(LevelBlockObjectModel block, IReadOnlyCollection<int2> positions, LevelObjectColor color)
        {
            var origin = LevelGridQueries.GetMinCorner(positions);
            var localPositions = positions
                .Select(position => position - origin)
                .OrderBy(localPosition => localPosition.Y)
                .ThenBy(localPosition => localPosition.X)
                .ToList();

            block.Initialize(origin, color, localPositions);
        }
    }
}
