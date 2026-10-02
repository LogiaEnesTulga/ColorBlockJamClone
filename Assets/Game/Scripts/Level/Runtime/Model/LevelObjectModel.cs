using RollicGames.Math.Runtime.Model;
using RollicGames.Pooling.Runtime.Model;

namespace RollicGames.ColorBlockJamClone.Level.Runtime.Model
{
    public class LevelObjectModel : IPoolObject
    {
        public virtual LevelObjectType ObjectType => LevelObjectType.None;

        public int Id => _id;

        public int2 GridPosition => _gridPosition;


        private int _id;
        protected int2 _gridPosition;

        public void SetPosition(int2 gridPosition)
        {
            _gridPosition = gridPosition;
        }

        public void SetId(int id)
        {
            _id = id;
        }

        public virtual void OnReturnedToPool() {}
    }
}