namespace RollicGames.Pooling.Runtime.Model
{
    public interface IPoolObject
    {
        int Id { get; }

        void SetId(int id);
        void OnReturnedToPool();
    }
}
