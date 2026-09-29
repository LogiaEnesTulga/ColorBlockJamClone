using RollicGames.Math.Runtime.Model;

namespace RollicGames.ColorBlockJamClone.Level.Runtime.Model
{
    public static class LevelExtensions
    {
        public static int2 GetDirectionVector(this LevelDirection direction)
        {
            switch(direction)
            {
                case LevelDirection.Up:
                    return int2.Up;
                case LevelDirection.Down:
                    return int2.Down;
                case LevelDirection.Left:
                    return int2.Left;
                case LevelDirection.Right:
                    return int2.Right;
                default:
                    return int2.Left;
            }
        }
    }
}