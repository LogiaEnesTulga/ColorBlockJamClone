using System;
using System.Collections.Generic;
using RollicGames.Pooling.Runtime.Model;

namespace RollicGames.Pooling.Runtime.Controller
{
    public interface IObjectPool<T> where T : class, IPoolObject
    {
        T Spawn();
        void Pool(T poolObject);
        void PoolAll();
    }

    public class ObjectPool<T> : IObjectPool<T> where T : class, IPoolObject
    {
        public const int DefaultCount = 50;

        private readonly Func<T> _factory;
        private readonly int _expandCount;

        private readonly Stack<T> _availableObjects = new();

        private readonly List<T> _allObjects = new();
        private readonly List<bool> _isAvailableById = new();

        public ObjectPool(Func<T> factory, int defaultCount = DefaultCount)
        {
            if(defaultCount <= 0) throw new ArgumentOutOfRangeException(nameof(defaultCount));

            _factory = factory;
            _expandCount = defaultCount;

            Expand();
        }

        public T Spawn()
        {
            if(_availableObjects.Count == 0)
            {
                Expand();
            }

            var poolObject = _availableObjects.Pop();
            _isAvailableById[poolObject.PoolId] = false;
            return poolObject;
        }

        public void Pool(T poolObject)
        {
            var id = poolObject.PoolId;
            if(id < 0 || id >= _allObjects.Count || !ReferenceEquals(_allObjects[id], poolObject))
            {
                throw new InvalidOperationException("Returned object does not belong to this pool.");
            }

            if(_isAvailableById[id])
            {
                throw new InvalidOperationException($"Object with pool id {id} is already returned to the pool.");
            }

            poolObject.OnReturnedToPool();
            _isAvailableById[id] = true;
            _availableObjects.Push(poolObject);
        }

        public void PoolAll()
        {
            for(var i = 0; i < _isAvailableById.Count; i++)
            {
                if(_isAvailableById[i]) continue;

                var obj = _allObjects[i];
                obj.OnReturnedToPool();
                _isAvailableById[i] = true;
                _availableObjects.Push(obj);
            }
        }

        private void Expand()
        {
            for(var i = 0; i < _expandCount; i++)
            {
                var poolObject = _factory();
                poolObject.SetPoolId(_allObjects.Count);

                _allObjects.Add(poolObject);
                _isAvailableById.Add(true);
                _availableObjects.Push(poolObject);
            }
        }
    }
}
