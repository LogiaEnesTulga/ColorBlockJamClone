using RollicGames.Math.Runtime.Model;
using UnityEngine;

namespace RollicGames.ColorBlockJamClone.Level.Editor
{
    /// <summary>Maps grid positions to canvas pixels. The area covers the grid plus a one cell ring for doors.</summary>
    public readonly struct LevelCanvasLayout
    {
        public readonly Vector2 Origin;
        public readonly float CellSize;

        private LevelCanvasLayout(Vector2 origin, float cellSize)
        {
            Origin = origin;
            CellSize = cellSize;
        }

        public static LevelCanvasLayout Create(Rect area, int gridWidth, int gridHeight, float padding, float maxCellSize)
        {
            var columns = gridWidth + 2;
            var rows = gridHeight + 2;
            var fitSize = Mathf.Min((area.width - padding * 2f) / columns, (area.height - padding * 2f) / rows);
            var cellSize = Mathf.Clamp(fitSize, 4f, maxCellSize);
            var origin = area.center - new Vector2(columns * cellSize, rows * cellSize) * 0.5f;
            return new LevelCanvasLayout(new Vector2(Mathf.Round(origin.x), Mathf.Round(origin.y)), cellSize);
        }

        public Rect GetCellRect(int2 position)
        {
            return new Rect(Origin.x + (position.X + 1) * CellSize, Origin.y + (position.Y + 1) * CellSize, CellSize, CellSize);
        }

        public Rect GetRect(int2 min, int2 max)
        {
            return Rect.MinMaxRect(GetCellRect(min).xMin, GetCellRect(min).yMin, GetCellRect(max).xMax, GetCellRect(max).yMax);
        }

        public int2 ToGridPosition(Vector2 localPosition)
        {
            var x = Mathf.FloorToInt((localPosition.x - Origin.x) / CellSize) - 1;
            var y = Mathf.FloorToInt((localPosition.y - Origin.y) / CellSize) - 1;
            return new int2(x, y);
        }
    }
}
