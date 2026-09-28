using RollicGames.Math.Runtime.Model;

namespace RollicGames.ColorBlockJamClone.Level.Runtime.Model
{
    public class LevelObjectModel
    {
        public static readonly LevelObjectModel EmptySpace = new(int2.Zero);
        
        public virtual LevelObjectType ObjectType => LevelObjectType.None;

        protected readonly int2 _gridPosition;
        
        public LevelObjectModel(int2 gridPosition)
        {
            _gridPosition = gridPosition;
        }
    }
}