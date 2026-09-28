using RollicGames.Math.Runtime.Model;

namespace RollicGames.ColorBlockJamClone.Level.Runtime.Model
{
    public class LevelWallObjectModel : LevelObjectModel
    {
        public override LevelObjectType ObjectType => LevelObjectType.Wall;

        public LevelWallObjectModel(int2 gridPosition) : base(gridPosition) {}
    }
}