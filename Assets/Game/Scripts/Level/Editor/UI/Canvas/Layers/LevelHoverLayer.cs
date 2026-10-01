using System.Collections.Generic;
using RollicGames.Math.Runtime.Model;
using UnityEngine.UIElements;

namespace RollicGames.ColorBlockJamClone.Level.Editor
{
    /// <summary>Tints the spots the active tool would affect: green when allowed, red when not, white for neutral hovers.</summary>
    public class LevelHoverLayer : ILevelCanvasLayer
    {
        public void Draw(Painter2D painter, LevelCanvasContext context)
        {
            var hover = context.Hover;
            if(hover.Feedback == LevelEditorFeedback.None || hover.Positions == null || hover.Positions.Count == 0) return;

            var layout = context.Layout;
            var color = LevelEditorTheme.GetFeedbackColor(hover.Feedback);
            var fillColor = LevelCanvasPainter.WithAlpha(color, hover.Feedback == LevelEditorFeedback.Neutral ? 0.12f : 0.3f);

            foreach(var position in hover.Positions)
            {
                var rect = LevelCanvasPainter.Inset(layout.GetCellRect(position), 2f);
                LevelCanvasPainter.FillRoundedRect(painter, rect, layout.CellSize * 0.14f, fillColor);
            }

            LevelCanvasPainter.StrokeOutline(painter, layout, new HashSet<int2>(hover.Positions), color, 2f);
        }
    }
}
