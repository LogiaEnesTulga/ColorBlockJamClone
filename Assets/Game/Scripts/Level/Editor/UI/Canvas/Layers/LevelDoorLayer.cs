using UnityEngine.UIElements;

namespace RollicGames.ColorBlockJamClone.Level.Editor
{
    public class LevelDoorLayer : ILevelCanvasLayer
    {
        public void Draw(Painter2D painter, LevelCanvasContext context)
        {
            foreach(var door in context.Grid.Doors.ObjectsById.Values)
            {
                LevelCanvasPainter.DrawDoor(painter, context.Layout, LevelGridQueries.GetDoorPositions(door), door.AbsorbDirection,
                    context.Colors.GetObjectColor(door.Color), context.Colors.GetArrowColor(door.Color));
            }
        }
    }
}
