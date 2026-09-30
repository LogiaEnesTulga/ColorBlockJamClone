using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using UnityEngine.SceneManagement;

namespace RollicGames.ColorBlockJamClone.SceneLoad.Runtime.Presenter
{
    public interface ISceneHandler
    {
        string SceneName { get; }

        void OnSceneReactivated();
    }

    public interface ISceneLoader
    {
        void RegisterHandler(ISceneHandler handler);
        UniTask Load(string sceneName);
    }

    public class SceneLoadPresenter : ISceneLoader
    {
        private readonly Dictionary<string, ISceneHandler> _handlers = new();

        private string _activeSceneName;

        public void RegisterHandler(ISceneHandler handler)
        {
            _handlers[handler.SceneName] = handler;
        }

        public async UniTask Load(string sceneName)
        {
            if(_activeSceneName == sceneName) return;

            if(_activeSceneName != null)
            {
                SetSceneObjectsActive(_activeSceneName, false);
                _activeSceneName = null;
            }

            var scene = SceneManager.GetSceneByName(sceneName);
            if(scene.isLoaded)
            {
                SetSceneObjectsActive(sceneName, true);
                SceneManager.SetActiveScene(scene);
                _activeSceneName = sceneName;

                if(_handlers.TryGetValue(sceneName, out var handler))
                {
                    handler.OnSceneReactivated();
                }

                return;
            }

            await SceneManager.LoadSceneAsync(sceneName, LoadSceneMode.Additive);
            _activeSceneName = sceneName;

            SceneManager.SetActiveScene(SceneManager.GetSceneByName(sceneName));
        }

        private static void SetSceneObjectsActive(string sceneName, bool isActive)
        {
            var scene = SceneManager.GetSceneByName(sceneName);
            if(!scene.isLoaded) return;

            scene.GetRootGameObjects()[0].SetActive(isActive);
        }
    }
}
