using Cysharp.Threading.Tasks;
using UnityEngine.SceneManagement;

namespace RollicGames.ColorBlockJamClone.SceneLoad.Runtime.Presenter
{
    public interface ISceneLoader
    {
        UniTask Load(string sceneName);
    }

    public class SceneLoadPresenter : ISceneLoader
    {
        private string _loadedSceneName;

        public async UniTask Load(string sceneName)
        {
            if (_loadedSceneName != null)
            {
                await SceneManager.UnloadSceneAsync(_loadedSceneName);
                _loadedSceneName = null;
            }

            await SceneManager.LoadSceneAsync(sceneName, LoadSceneMode.Additive);
            _loadedSceneName = sceneName;

            SceneManager.SetActiveScene(SceneManager.GetSceneByName(sceneName));
        }
    }
}
