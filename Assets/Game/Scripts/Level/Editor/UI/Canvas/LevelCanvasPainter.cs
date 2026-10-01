using System.Collections.Generic;
using RollicGames.Math.Runtime.Model;
using RollicGames.ColorBlockJamClone.Level.Runtime.Model;
using UnityEngine;
using UnityEngine.UIElements;

namespace RollicGames.ColorBlockJamClone.Level.Editor
{
    public static class LevelCanvasPainter
    {
        public static void FillRect(Painter2D painter, Rect rect, Color color)
        {
            painter.fillColor = color;
            painter.BeginPath();
            painter.MoveTo(new Vector2(rect.xMin, rect.yMin));
            painter.LineTo(new Vector2(rect.xMax, rect.yMin));
            painter.LineTo(new Vector2(rect.xMax, rect.yMax));
            painter.LineTo(new Vector2(rect.xMin, rect.yMax));
            painter.ClosePath();
            painter.Fill();
        }

        public static void FillRoundedRect(Painter2D painter, Rect rect, float radius, Color color)
        {
            painter.fillColor = color;
            AddRoundedRectPath(painter, rect, radius);
            painter.Fill();
        }

        public static void StrokeRoundedRect(Painter2D painter, Rect rect, float radius, Color color, float width)
        {
            painter.strokeColor = color;
            painter.lineWidth = width;
            AddRoundedRectPath(painter, rect, radius);
            painter.Stroke();
        }

        public static void FillCircle(Painter2D painter, Vector2 center, float radius, Color color)
        {
            painter.fillColor = color;
            painter.BeginPath();
            painter.Arc(center, radius, Angle.Degrees(0f), Angle.Degrees(360f));
            painter.ClosePath();
            painter.Fill();
        }

        public static void FillPolygon(Painter2D painter, Color color, params Vector2[] points)
        {
            painter.fillColor = color;
            painter.BeginPath();
            painter.MoveTo(points[0]);
            for(var i = 1; i < points.Length; i++)
            {
                painter.LineTo(points[i]);
            }
            painter.ClosePath();
            painter.Fill();
        }

        /// <summary>Strokes only the outer edges of a set of grid positions, so a multi piece object gets a single outline.</summary>
        public static void StrokeOutline(Painter2D painter, LevelCanvasLayout layout, ICollection<int2> positions, Color color, float width)
        {
            painter.strokeColor = color;
            painter.lineWidth = width;
            painter.lineCap = LineCap.Round;
            painter.lineJoin = LineJoin.Round;
            painter.BeginPath();

            foreach(var position in positions)
            {
                var rect = layout.GetCellRect(position);
                if(!positions.Contains(position + int2.Up)) AddLine(painter, new Vector2(rect.xMin, rect.yMin), new Vector2(rect.xMax, rect.yMin));
                if(!positions.Contains(position + int2.Down)) AddLine(painter, new Vector2(rect.xMin, rect.yMax), new Vector2(rect.xMax, rect.yMax));
                if(!positions.Contains(position + int2.Left)) AddLine(painter, new Vector2(rect.xMin, rect.yMin), new Vector2(rect.xMin, rect.yMax));
                if(!positions.Contains(position + int2.Right)) AddLine(painter, new Vector2(rect.xMax, rect.yMin), new Vector2(rect.xMax, rect.yMax));
            }

            painter.Stroke();
        }

        /// <summary>Draws connected pieces as one rounded shape by bridging the gaps between neighbouring pieces.</summary>
        public static void DrawBlockShape(Painter2D painter, LevelCanvasLayout layout, ICollection<int2> positions, Color color, Vector2 offset)
        {
            var inset = layout.CellSize * 0.08f;
            var radius = layout.CellSize * 0.2f;

            foreach(var position in positions)
            {
                var rect = GetPieceRect(layout, position, inset, offset);
                FillRoundedRect(painter, rect, radius, color);

                var hasRight = positions.Contains(position + int2.Right);
                var hasDown = positions.Contains(position + int2.Down);
                if(hasRight)
                {
                    var right = GetPieceRect(layout, position + int2.Right, inset, offset);
                    FillRect(painter, Rect.MinMaxRect(rect.center.x, rect.yMin, right.center.x, rect.yMax), color);
                }

                if(hasDown)
                {
                    var down = GetPieceRect(layout, position + int2.Down, inset, offset);
                    FillRect(painter, Rect.MinMaxRect(rect.xMin, rect.center.y, rect.xMax, down.center.y), color);
                }

                if(hasRight && hasDown && positions.Contains(position + int2.RightDown))
                {
                    var diagonal = GetPieceRect(layout, position + int2.RightDown, inset, offset);
                    FillRect(painter, Rect.MinMaxRect(rect.center.x, rect.center.y, diagonal.center.x, diagonal.center.y), color);
                }
            }
        }

        public static void DrawDoor(Painter2D painter, LevelCanvasLayout layout, IReadOnlyList<int2> positions, LevelDirection direction, Color color, Color arrowColor)
        {
            var cellSize = layout.CellSize;
            var thickness = cellSize * 0.38f;
            var inset = cellSize * 0.08f;
            var bounds = layout.GetRect(LevelGridQueries.GetMinCorner(positions), GetMaxCorner(positions));
            var center = bounds.center;

            var bar = LevelGridQueries.IsHorizontal(direction)
                ? Rect.MinMaxRect(bounds.xMin + inset, center.y - thickness * 0.5f, bounds.xMax - inset, center.y + thickness * 0.5f)
                : Rect.MinMaxRect(center.x - thickness * 0.5f, bounds.yMin + inset, center.x + thickness * 0.5f, bounds.yMax - inset);

            FillRoundedRect(painter, Offset(bar, new Vector2(0f, cellSize * 0.06f)), thickness * 0.5f, Shade(color, 0.68f));
            FillRoundedRect(painter, bar, thickness * 0.5f, color);

            var directionVector = direction.GetDirectionVector();
            var forward = new Vector2(directionVector.X, directionVector.Y);
            var side = new Vector2(-forward.y, forward.x);
            var arrowSize = thickness * 0.34f;
            foreach(var position in positions)
            {
                var arrowCenter = layout.GetCellRect(position).center;
                FillPolygon(painter, arrowColor,
                    arrowCenter + forward * arrowSize,
                    arrowCenter - forward * (arrowSize * 0.6f) + side * arrowSize,
                    arrowCenter - forward * (arrowSize * 0.6f) - side * arrowSize);
            }
        }

        public static Rect Inset(Rect rect, float amount)
        {
            return new Rect(rect.x + amount, rect.y + amount, rect.width - amount * 2f, rect.height - amount * 2f);
        }

        public static Rect Offset(Rect rect, Vector2 offset)
        {
            return new Rect(rect.position + offset, rect.size);
        }

        public static Color Shade(Color color, float factor)
        {
            return new Color(color.r * factor, color.g * factor, color.b * factor, color.a);
        }

        public static Color WithAlpha(Color color, float alpha)
        {
            return new Color(color.r, color.g, color.b, alpha);
        }

        private static Rect GetPieceRect(LevelCanvasLayout layout, int2 position, float inset, Vector2 offset)
        {
            return Offset(Inset(layout.GetCellRect(position), inset), offset);
        }

        private static int2 GetMaxCorner(IEnumerable<int2> positions)
        {
            var maxX = int.MinValue;
            var maxY = int.MinValue;
            foreach(var position in positions)
            {
                maxX = Mathf.Max(maxX, position.X);
                maxY = Mathf.Max(maxY, position.Y);
            }

            return new int2(maxX, maxY);
        }

        private static void AddLine(Painter2D painter, Vector2 from, Vector2 to)
        {
            painter.MoveTo(from);
            painter.LineTo(to);
        }

        private static void AddRoundedRectPath(Painter2D painter, Rect rect, float radius)
        {
            radius = Mathf.Max(0.01f, Mathf.Min(radius, rect.width * 0.5f, rect.height * 0.5f));
            painter.BeginPath();
            painter.MoveTo(new Vector2(rect.xMin + radius, rect.yMin));
            painter.ArcTo(new Vector2(rect.xMax, rect.yMin), new Vector2(rect.xMax, rect.yMax), radius);
            painter.ArcTo(new Vector2(rect.xMax, rect.yMax), new Vector2(rect.xMin, rect.yMax), radius);
            painter.ArcTo(new Vector2(rect.xMin, rect.yMax), new Vector2(rect.xMin, rect.yMin), radius);
            painter.ArcTo(new Vector2(rect.xMin, rect.yMin), new Vector2(rect.xMax, rect.yMin), radius);
            painter.ClosePath();
        }
    }
}
