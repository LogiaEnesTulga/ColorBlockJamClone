using RollicGames.Math.Runtime.Model;

namespace RollicGames.ColorBlockJamClone.Level.Runtime.Model
{
    public class LevelWallObjectModel : LevelObjectModel
    {
        public override LevelObjectType ObjectType => LevelObjectType.Block;

        public LevelObjectColor UpDoorColor => _upDoorColor;
        public LevelObjectColor DownDoorColor => _downDoorColor;
        public LevelObjectColor RightDoorColor => _rightDoorColor;
        public LevelObjectColor LeftDoorColor => _leftDoorColor;

        private readonly LevelObjectColor _upDoorColor;
        private readonly LevelObjectColor _downDoorColor;
        private readonly LevelObjectColor _rightDoorColor;
        private readonly LevelObjectColor _leftDoorColor;

        public LevelWallObjectModel(int2 gridPosition, 
        LevelObjectColor upDoorColor = LevelObjectColor.None,
        LevelObjectColor downDoorColor = LevelObjectColor.None,
        LevelObjectColor rightDoorColor = LevelObjectColor.None,
        LevelObjectColor leftDoorColor = LevelObjectColor.None) : base(gridPosition)
        {
            _upDoorColor = upDoorColor;
            _downDoorColor = downDoorColor;
            _rightDoorColor = rightDoorColor;
            _leftDoorColor = leftDoorColor;
        }
    }
}