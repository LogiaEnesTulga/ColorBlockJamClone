using RollicGames.Math.Runtime.Model;
using RollicGames.ColorBlockJamClone.Level.Runtime.Model;

namespace RollicGames.ColorBlockJamClone.Level.Editor
{
    public class LevelCellEditor
    {
        private readonly LevelGridModel _grid;
        private readonly LevelBlockEditor _blockEditor;
        private readonly LevelDoorEditor _doorEditor;

        public LevelCellEditor(LevelGridModel grid, LevelBlockEditor blockEditor, LevelDoorEditor doorEditor)
        {
            _grid = grid;
            _blockEditor = blockEditor;
            _doorEditor = doorEditor;
        }

        public bool CanAdd(int2 position)
        {
            return _grid.IsInsideGrid(position)
                && !_grid.Cells.Contains(position)
                && !_grid.Doors.ObjectsByPosition.ContainsKey(position);
        }

        public void Add(int2 position)
        {
            _grid.Cells.Add(position);
        }

        public bool CanRemove(int2 position)
        {
            return _grid.Cells.Contains(position);
        }

        public void Remove(int2 position)
        {
            if(_grid.Blocks.ObjectsByPosition.TryGetValue(position, out var block))
            {
                _blockEditor.Remove(block);
            }

            _grid.Cells.Remove(position);
            _doorEditor.RemoveInvalidDoors();
        }
    }
}
