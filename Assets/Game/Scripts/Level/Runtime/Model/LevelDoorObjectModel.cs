using RollicGames.Math.Runtime.Model;

namespace RollicGames.ColorBlockJamClone.Level.Runtime.Model
{
    public class LevelDoorObjectModel : LevelObjectModel
    {
        public override LevelObjectType ObjectType => LevelObjectType.Door;

        public int Length => _doorLength;
        public LevelObjectColor Color => _doorColor;
        public LevelDirection AbsorbDirection => _doorAbsorbDirection;

        private readonly int _doorLength;
        private readonly LevelObjectColor _doorColor;
        private readonly LevelDirection _doorAbsorbDirection;

        public LevelDoorObjectModel(int2 gridPosition, int doorLength,
        LevelObjectColor doorColor, LevelDirection doorAbsorbDirection) : base(gridPosition)
        {
            _doorLength = doorLength;
            _doorColor = doorColor;
            _doorAbsorbDirection = doorAbsorbDirection;
        }
    }
}