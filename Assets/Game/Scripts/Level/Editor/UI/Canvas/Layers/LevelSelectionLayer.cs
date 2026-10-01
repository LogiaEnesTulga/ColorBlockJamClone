using System.Collections.Generic;
using RollicGames.Math.Runtime.Model;
using UnityEngine.UIElements;

namespace RollicGames.ColorBlockJamClone.Level.Editor
{
    public class LevelSelectionLayer : ILevelCanvasLayer
    {
        public void Draw(Painter2D painter, LevelCanvasContext context)
        {
            var selected = context.Selected;
            if(selected == null) return;

            var positions = new HashSet<int2>(context.Editors.For(selected).GetPositions(selected));
            LevelCanvasPainter.StrokeOutline(painter, context.Layout, positions, LevelCanvasPainter.WithAlpha(LevelEditorTheme.Accent, 0.25f), 9f);
            LevelCanvasPainter.StrokeOutline(painter, context.Layout, positions, LevelEditorTheme.Accent, 2.5f);
        }
    }
}
