using RollicGames.Math.Runtime.Model;

namespace RollicGames.ColorBlockJamClone.Level.Runtime.Model
{
    public class LevelObjectModel
    {
        public static readonly LevelObjectModel EmptySpace = new(int2.Zero);

        public virtual LevelObjectType ObjectType => LevelObjectType.None;

        public int Id => _id;
        public int2 GridPosition => _gridPosition;

        protected int _id;
        protected int2 _gridPosition;
        
        public LevelObjectModel(int2 gridPosition)
        {
            _gridPosition = gridPosition;
        }

        public void SetId(int id)
        {
            _id = id;
        }

        public void SetPosition(int2 gridPosition)
        {
            _gridPosition = gridPosition;
        }
    }
}