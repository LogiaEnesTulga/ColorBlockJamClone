using UnityEngine;
using UnityEngine.UIElements;

namespace RollicGames.ColorBlockJamClone.Level.Editor
{
    /// <summary>Draws a delete badge next to the pointer, tinted green or red depending on whether the delete is allowed.</summary>
    public class LevelCursorLayer : ILevelCanvasLayer
    {
        private const float BadgeRadius = 12f;
        private const float IconSize = 13f;
        private static readonly Vector2 BadgeOffset = new(18f, 18f);

        public void Draw(Painter2D painter, LevelCanvasContext context)
        {
            var hover = context.Hover;
            if(hover.Cursor != LevelEditorCursor.Delete || hover.Feedback == LevelEditorFeedback.None || !context.PointerPosition.HasValue) return;

            var center = context.PointerPosition.Value + BadgeOffset;
            var color = LevelEditorTheme.GetFeedbackColor(hover.Feedback);
            var iconColor = hover.Feedback == LevelEditorFeedback.Neutral ? LevelEditorTheme.Background : Color.white;

            LevelCanvasPainter.FillCircle(painter, center, BadgeRadius + 2f, LevelEditorTheme.Background);
            LevelCanvasPainter.FillCircle(painter, center, BadgeRadius, color);
            LevelEditorIcons.DrawDelete(painter, new Rect(center - Vector2.one * (IconSize * 0.5f), Vector2.one * IconSize), iconColor);
        }
    }
}
