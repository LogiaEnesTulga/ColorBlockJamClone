using RollicGames.Math.Runtime.Model;
using UnityEngine;
using UnityEngine.UIElements;

namespace RollicGames.ColorBlockJamClone.Level.Editor
{
    /// <summary>Draws the board, the outer door ring and a faint dot on every empty spot.</summary>
    public class LevelBoardLayer : ILevelCanvasLayer
    {
        private readonly bool _drawSlotDots;

        public LevelBoardLayer(bool drawSlotDots)
        {
            _drawSlotDots = drawSlotDots;
        }

        public void Draw(Painter2D painter, LevelCanvasContext context)
        {
            var grid = context.Grid;
            if(grid.Width <= 0 || grid.Height <= 0) return;

            var layout = context.Layout;
            var cellSize = layout.CellSize;
            var doorArea = layout.GetRect(new int2(-1, -1), new int2(grid.Width, grid.Height));
            LevelCanvasPainter.FillRoundedRect(painter, doorArea, cellSize * 0.3f, LevelEditorTheme.BoardRing);

            var gridArea = layout.GetRect(int2.Zero, new int2(grid.Width - 1, grid.Height - 1));
            LevelCanvasPainter.FillRoundedRect(painter, LevelCanvasPainter.Inset(gridArea, -cellSize * 0.06f), cellSize * 0.2f, LevelEditorTheme.Board);

            if(!_drawSlotDots) return;

            var dotRadius = Mathf.Max(1.5f, cellSize * 0.045f);
            for(var x = -1; x <= grid.Width; x++)
            {
                for(var y = -1; y <= grid.Height; y++)
                {
                    var position = new int2(x, y);
                    if(grid.Cells.Contains(position) || grid.Doors.ObjectsByPosition.ContainsKey(position)) continue;

                    LevelCanvasPainter.FillCircle(painter, layout.GetCellRect(position).center, dotRadius, LevelEditorTheme.SlotDot);
                }
            }
        }
    }
}
