using System.Collections.Generic;
using RollicGames.Math.Runtime.Model;
using UnityEngine;
using UnityEngine.UIElements;

namespace RollicGames.ColorBlockJamClone.Level.Editor
{
    /// <summary>Draws the object a tool is currently building, before it is finished.</summary>
    public class LevelDraftLayer : ILevelCanvasLayer
    {
        public void Draw(Painter2D painter, LevelCanvasContext context)
        {
            if(context.Tool is not ILevelEditorDraftProvider draftProvider || draftProvider.DraftPositions.Count == 0) return;

            var positions = new HashSet<int2>(draftProvider.DraftPositions);
            var color = context.Colors.GetObjectColor(draftProvider.DraftColor);

            LevelCanvasPainter.DrawBlockShape(painter, context.Layout, positions, LevelCanvasPainter.WithAlpha(color, 0.6f), Vector2.zero);
            LevelCanvasPainter.StrokeOutline(painter, context.Layout, positions, LevelEditorTheme.Accent, 2f);
        }
    }
}
