using System;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;

namespace RollicGames.AddressableLoading.Runtime.View
{
    public interface IAddressableLoader
    {
        UniTask<GameObject> LoadPrefab(string addressableName, Transform parent);
        void Release(GameObject instance);
    }

    public class AddressableLoader : IAddressableLoader, IDisposable
    {
        private readonly Dictionary<GameObject, AsyncOperationHandle<GameObject>> _loadedInstances = new();

        public async UniTask<GameObject> LoadPrefab(string addressableName, Transform parent)
        {
            var handle = Addressables.InstantiateAsync(addressableName, parent: parent);
            var instance = await handle.ToUniTask();

            _loadedInstances[instance] = handle;

            return instance;
        }

        public void Release(GameObject instance)
        {
            if (instance == null || !_loadedInstances.Remove(instance, out var handle))
            {
                return;
            }

            Addressables.ReleaseInstance(handle);
        }

        public void Dispose()
        {
            foreach (var handle in _loadedInstances.Values)
            {
                Addressables.ReleaseInstance(handle);
            }

            _loadedInstances.Clear();
        }
    }
}
