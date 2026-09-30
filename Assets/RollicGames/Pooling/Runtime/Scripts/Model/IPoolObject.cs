namespace RollicGames.Pooling.Runtime.Model
{
    public interface IPoolObject
    {
        int PoolId { get; }

        void SetPoolId(int poolId);
        void OnReturnedToPool();
    }
}
