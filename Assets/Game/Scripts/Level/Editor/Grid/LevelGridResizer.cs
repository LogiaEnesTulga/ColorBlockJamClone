using System.Linq;
using RollicGames.Math.Runtime.Model;
using RollicGames.ColorBlockJamClone.Level.Runtime.Model;

namespace RollicGames.ColorBlockJamClone.Level.Editor
{
    public class LevelGridResizer
    {
        private readonly LevelGridModel _grid;
        private readonly LevelCellEditor _cellEditor;
        private readonly LevelDoorEditor _doorEditor;

        public LevelGridResizer(LevelGridModel grid, LevelCellEditor cellEditor, LevelDoorEditor doorEditor)
        {
            _grid = grid;
            _cellEditor = cellEditor;
            _doorEditor = doorEditor;
        }

        public bool IsShrinking(int width, int height)
        {
            return width < _grid.Width || height < _grid.Height;
        }

        public void Resize(int width, int height)
        {
            var oldWidth = _grid.Width;
            var oldHeight = _grid.Height;
            _grid.Width = width;
            _grid.Height = height;

            foreach(var cell in _grid.Cells.Where(cell => !_grid.IsInsideGrid(cell)).ToList())
            {
                _cellEditor.Remove(cell);
            }

            for(var x = 0; x < width; x++)
            {
                for(var y = 0; y < height; y++)
                {
                    var position = new int2(x, y);
                    var isNewArea = x >= oldWidth || y >= oldHeight;
                    if(isNewArea && _cellEditor.CanAdd(position))
                    {
                        _cellEditor.Add(position);
                    }
                }
            }

            _doorEditor.RemoveInvalidDoors();
        }
    }
}
