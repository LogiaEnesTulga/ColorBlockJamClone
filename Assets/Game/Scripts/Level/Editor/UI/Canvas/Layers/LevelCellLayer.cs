using UnityEngine.UIElements;

namespace RollicGames.ColorBlockJamClone.Level.Editor
{
    public class LevelCellLayer : ILevelCanvasLayer
    {
        public void Draw(Painter2D painter, LevelCanvasContext context)
        {
            var layout = context.Layout;
            var cellSize = layout.CellSize;
            var cellColor = context.Colors.CellColor;
            var edgeColor = LevelCanvasPainter.Shade(cellColor, 0.8f);

            foreach(var cell in context.Grid.Cells)
            {
                var rect = LevelCanvasPainter.Inset(layout.GetCellRect(cell), cellSize * 0.04f);
                LevelCanvasPainter.FillRoundedRect(painter, rect, cellSize * 0.14f, edgeColor);
                LevelCanvasPainter.FillRoundedRect(painter, LevelCanvasPainter.Inset(rect, cellSize * 0.04f), cellSize * 0.11f, cellColor);
            }
        }
    }
}
