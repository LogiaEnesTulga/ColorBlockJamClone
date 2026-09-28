using System.Collections.Generic;

namespace RollicGames.ColorBlockJamClone.Level.Runtime.Model
{
    public class LevelGridModel
    {
        public int Width;
        public int Height;

        public readonly List<List<LevelObjectModel>> Grid;
    }
}