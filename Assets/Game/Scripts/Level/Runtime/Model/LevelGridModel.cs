using System.Collections.Generic;
using RollicGames.Math.Runtime.Model;

namespace RollicGames.ColorBlockJamClone.Level.Runtime.Model
{
    public class LevelGridModel
    {
        public int Width;
        public int Height;

        public readonly HashSet<int2> Cells = new HashSet<int2>();
        public readonly HashSet<LevelBlockObjectModel> Blocks = new HashSet<LevelBlockObjectModel>();
        public readonly List<List<LevelObjectModel>> Grid = new List<List<LevelObjectModel>>();
    }
}