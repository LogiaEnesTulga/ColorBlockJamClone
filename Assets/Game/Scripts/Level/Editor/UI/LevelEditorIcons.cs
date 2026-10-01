using UnityEngine;
using UnityEngine.UIElements;

namespace RollicGames.ColorBlockJamClone.Level.Editor
{
    /// <summary>Vector icons drawn with Painter2D, so they stay crisp at any size and can be tinted per state.</summary>
    public static class LevelEditorIcons
    {
        public static void DrawSelect(Painter2D painter, Rect rect, Color color)
        {
            LevelCanvasPainter.FillPolygon(painter, color,
                Point(rect, 0.24f, 0.08f), Point(rect, 0.24f, 0.84f), Point(rect, 0.43f, 0.66f),
                Point(rect, 0.56f, 0.94f), Point(rect, 0.69f, 0.88f), Point(rect, 0.56f, 0.61f), Point(rect, 0.82f, 0.61f));
        }

        public static void DrawCell(Painter2D painter, Rect rect, Color color)
        {
            var size = rect.width * 0.4f;
            var gap = rect.width * 0.1f;
            for(var x = 0; x < 2; x++)
            {
                for(var y = 0; y < 2; y++)
                {
                    var square = new Rect(rect.x + rect.width * 0.05f + x * (size + gap), rect.y + rect.height * 0.05f + y * (size + gap), size, size);
                    LevelCanvasPainter.FillRoundedRect(painter, square, size * 0.25f, color);
                }
            }
        }

        public static void DrawBlock(Painter2D painter, Rect rect, Color color)
        {
            var unit = rect.width * 0.3f;
            var gap = rect.width * 0.04f;
            var left = rect.x + (rect.width - unit * 2f - gap) * 0.5f;
            var top = rect.y + (rect.height - unit * 3f - gap * 2f) * 0.5f;
            DrawUnit(0, 0);
            DrawUnit(0, 1);
            DrawUnit(0, 2);
            DrawUnit(1, 2);

            void DrawUnit(int x, int y)
            {
                var square = new Rect(left + x * (unit + gap), top + y * (unit + gap), unit, unit);
                LevelCanvasPainter.FillRoundedRect(painter, square, unit * 0.25f, color);
            }
        }

        public static void DrawDoor(Painter2D painter, Rect rect, Color color)
        {
            LevelCanvasPainter.FillRoundedRect(painter, new Rect(Point(rect, 0.08f, 0.74f), new Vector2(rect.width * 0.84f, rect.height * 0.18f)), rect.height * 0.09f, color);
            LevelCanvasPainter.FillPolygon(painter, color, Point(rect, 0.5f, 0.06f), Point(rect, 0.2f, 0.4f), Point(rect, 0.8f, 0.4f));
            LevelCanvasPainter.FillRect(painter, Rect.MinMaxRect(rect.x + rect.width * 0.4f, rect.y + rect.height * 0.38f, rect.x + rect.width * 0.6f, rect.y + rect.height * 0.62f), color);
        }

        public static void DrawMove(Painter2D painter, Rect rect, Color color)
        {
            var thickness = rect.width * 0.12f;
            var center = rect.center;
            LevelCanvasPainter.FillRect(painter, new Rect(rect.x + rect.width * 0.2f, center.y - thickness * 0.5f, rect.width * 0.6f, thickness), color);
            LevelCanvasPainter.FillRect(painter, new Rect(center.x - thickness * 0.5f, rect.y + rect.height * 0.2f, thickness, rect.height * 0.6f), color);
            LevelCanvasPainter.FillPolygon(painter, color, Point(rect, 0.5f, 0.0f), Point(rect, 0.3f, 0.24f), Point(rect, 0.7f, 0.24f));
            LevelCanvasPainter.FillPolygon(painter, color, Point(rect, 0.5f, 1.0f), Point(rect, 0.3f, 0.76f), Point(rect, 0.7f, 0.76f));
            LevelCanvasPainter.FillPolygon(painter, color, Point(rect, 0.0f, 0.5f), Point(rect, 0.24f, 0.3f), Point(rect, 0.24f, 0.7f));
            LevelCanvasPainter.FillPolygon(painter, color, Point(rect, 1.0f, 0.5f), Point(rect, 0.76f, 0.3f), Point(rect, 0.76f, 0.7f));
        }

        public static void DrawDelete(Painter2D painter, Rect rect, Color color)
        {
            LevelCanvasPainter.FillRoundedRect(painter, new Rect(Point(rect, 0.38f, 0.04f), new Vector2(rect.width * 0.24f, rect.height * 0.12f)), rect.height * 0.04f, color);
            LevelCanvasPainter.FillRoundedRect(painter, new Rect(Point(rect, 0.12f, 0.14f), new Vector2(rect.width * 0.76f, rect.height * 0.12f)), rect.height * 0.05f, color);
            LevelCanvasPainter.FillPolygon(painter, color, Point(rect, 0.2f, 0.32f), Point(rect, 0.8f, 0.32f), Point(rect, 0.72f, 0.96f), Point(rect, 0.28f, 0.96f));
        }

        private static Vector2 Point(Rect rect, float x, float y)
        {
            return new Vector2(rect.x + rect.width * x, rect.y + rect.height * y);
        }
    }
}
