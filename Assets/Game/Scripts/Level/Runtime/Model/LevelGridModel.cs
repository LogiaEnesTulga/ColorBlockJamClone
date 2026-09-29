using System.Collections.Generic;
using RollicGames.Math.Runtime.Model;

namespace RollicGames.ColorBlockJamClone.Level.Runtime.Model
{
    public class LevelGridModel
    {
        public int Width;
        public int Height;

        public readonly HashSet<int2> Cells = new HashSet<int2>();
        public readonly LevelObjectContainer<LevelBlockObjectModel> Blocks = new LevelObjectContainer<LevelBlockObjectModel>();
        public readonly LevelObjectContainer<LevelDoorObjectModel> Doors = new LevelObjectContainer<LevelDoorObjectModel>();
    }
}