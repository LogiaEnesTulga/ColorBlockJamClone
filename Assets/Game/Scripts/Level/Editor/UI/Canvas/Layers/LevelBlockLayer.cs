using System.Collections.Generic;
using RollicGames.Math.Runtime.Model;
using UnityEngine;
using UnityEngine.UIElements;

namespace RollicGames.ColorBlockJamClone.Level.Editor
{
    public class LevelBlockLayer : ILevelCanvasLayer
    {
        public void Draw(Painter2D painter, LevelCanvasContext context)
        {
            var layout = context.Layout;
            var depth = new Vector2(0f, layout.CellSize * 0.07f);

            foreach(var block in context.Grid.Blocks.ObjectsById.Values)
            {
                var positions = new HashSet<int2>(LevelGridQueries.GetBlockPositions(block));
                var color = context.Colors.GetObjectColor(block.Color);
                var studColor = Color.Lerp(color, Color.white, 0.25f);

                LevelCanvasPainter.DrawBlockShape(painter, layout, positions, LevelCanvasPainter.Shade(color, 0.68f), depth);
                LevelCanvasPainter.DrawBlockShape(painter, layout, positions, color, Vector2.zero);

                foreach(var position in positions)
                {
                    LevelCanvasPainter.FillCircle(painter, layout.GetCellRect(position).center, layout.CellSize * 0.12f, studColor);
                }
            }
        }
    }
}
