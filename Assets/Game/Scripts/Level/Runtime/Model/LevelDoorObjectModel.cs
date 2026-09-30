using RollicGames.Math.Runtime.Model;

namespace RollicGames.ColorBlockJamClone.Level.Runtime.Model
{
    public class LevelDoorObjectModel : LevelObjectModel
    {
        public override LevelObjectType ObjectType => LevelObjectType.Door;

        public int Length => _doorLength;
        public LevelObjectColor Color => _doorColor;
        public LevelDirection AbsorbDirection => _doorAbsorbDirection;

        private int _doorLength;
        private LevelObjectColor _doorColor;
        private LevelDirection _doorAbsorbDirection;

        public void Initialize(int2 gridPosition, int doorLength, LevelObjectColor doorColor, LevelDirection doorAbsorbDirection)
        {
            _gridPosition = gridPosition;
            _doorLength = doorLength;
            _doorColor = doorColor;
            _doorAbsorbDirection = doorAbsorbDirection;
        }

        public override void OnReturnedToPool()
        {
            _gridPosition = int2.Zero;
            _doorLength = 0;
            _doorColor = default;
            _doorAbsorbDirection = default;
        }
    }
}
