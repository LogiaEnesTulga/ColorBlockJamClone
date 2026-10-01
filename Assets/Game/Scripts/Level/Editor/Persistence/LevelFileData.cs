using System.Collections.Generic;
using RollicGames.ColorBlockJamClone.Level.Runtime.Model;

namespace RollicGames.ColorBlockJamClone.Level.Editor
{
    public class LevelFileData
    {
        public const int CurrentSchemaVersion = 1;

        public int SchemaVersion = CurrentSchemaVersion;
        public int LevelId;
        public float Duration;
        public LevelGridFileData Grid = new();
        public List<LevelBlockFileData> Blocks = new();
        public List<LevelDoorFileData> Doors = new();
    }

    public class LevelGridFileData
    {
        public int Width;
        public int Height;
        public List<LevelPositionFileData> Cells = new();
    }

    public class LevelPositionFileData
    {
        public int X;
        public int Y;
    }

    public class LevelBlockFileData
    {
        public int Id;
        public LevelObjectType Type = LevelObjectType.Block;
        public LevelObjectColor Color;
        public LevelPositionFileData Position;
        public List<LevelPositionFileData> LocalPositions = new();
        public Dictionary<string, object> Properties = new();
    }

    public class LevelDoorFileData
    {
        public int Id;
        public LevelObjectType Type = LevelObjectType.Door;
        public LevelObjectColor Color;
        public LevelPositionFileData Position;
        public int Length;
        public LevelDirection AbsorbDirection;
        public Dictionary<string, object> Properties = new();
    }
}
