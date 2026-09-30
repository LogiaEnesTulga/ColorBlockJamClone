using System;
using System.Collections.Generic;
using UnityEngine;

namespace RollicGames.Pooling.Runtime.View
{
    public interface IViewPool<T> where T : Component
    {
        T Spawn(Transform parent);
        void Pool(T poolObject);
        void PoolAll();
    }

    public abstract class ViewPool<T> : MonoBehaviour, IViewPool<T> where T : Component
    {
        public const int DefaultCount = 50;

        [SerializeField] private T _prefab;
        [SerializeField] private int _defaultCount = DefaultCount;

        private readonly Stack<T> _availableObjects = new();
        private readonly HashSet<T> _availableSet = new();
        private readonly HashSet<T> _allObjects = new();

        private void Awake()
        {
            if(_defaultCount <= 0) throw new InvalidOperationException("Default count of a view pool must be positive.");

            Expand();
        }

        // If the parent is null, the object stays under the pool in the hierarchy.
        public T Spawn(Transform parent)
        {
            if(_availableObjects.Count == 0)
            {
                Expand();
            }

            var poolObject = _availableObjects.Pop();
            _availableSet.Remove(poolObject);

            poolObject.transform.SetParent(parent != null ? parent : transform, false);
            poolObject.gameObject.SetActive(true);
            return poolObject;
        }

        public void Pool(T poolObject)
        {
            if(!_allObjects.Contains(poolObject))
            {
                throw new InvalidOperationException("Pooled object does not belong to this pool.");
            }

            if(!_availableSet.Add(poolObject))
            {
                throw new InvalidOperationException($"{poolObject.name} is already pooled.");
            }

            Deactivate(poolObject);
            _availableObjects.Push(poolObject);
        }

        public void PoolAll()
        {
            foreach(var poolObject in _allObjects)
            {
                if(!_availableSet.Add(poolObject)) continue;

                Deactivate(poolObject);
                _availableObjects.Push(poolObject);
            }
        }

        private void Deactivate(T poolObject)
        {
            var objectTransform = poolObject.transform;
            objectTransform.SetParent(transform, false);
            objectTransform.localPosition = Vector3.zero;
            objectTransform.localRotation = Quaternion.identity;
            poolObject.gameObject.SetActive(false);
        }

        private void Expand()
        {
            for(var i = 0; i < _defaultCount; i++)
            {
                var poolObject = Instantiate(_prefab, transform);
                poolObject.gameObject.SetActive(false);

                _allObjects.Add(poolObject);
                _availableSet.Add(poolObject);
                _availableObjects.Push(poolObject);
            }
        }
    }
}
