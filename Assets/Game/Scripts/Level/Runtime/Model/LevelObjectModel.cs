using RollicGames.Math.Runtime.Model;
using RollicGames.Pooling.Runtime.Model;

namespace RollicGames.ColorBlockJamClone.Level.Runtime.Model
{
    public class LevelObjectModel : IPoolObject
    {
        public static readonly LevelObjectModel EmptySpace = new();

        public virtual LevelObjectType ObjectType => LevelObjectType.None;

        public int PoolId => _poolId;

        public int2 GridPosition => _gridPosition;


        private int _poolId;
        protected int2 _gridPosition;

        public void SetPosition(int2 gridPosition)
        {
            _gridPosition = gridPosition;
        }

        public void SetPoolId(int poolId)
        {
            _poolId = poolId;
        }

        public virtual void OnReturnedToPool() {}
    }
}