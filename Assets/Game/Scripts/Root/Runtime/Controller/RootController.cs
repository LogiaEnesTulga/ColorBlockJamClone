using RollicGames.ColorBlockJamClone.Player.Runtime.Controller;
using RollicGames.ColorBlockJamClone.SceneLoad.Runtime.Model;
using RollicGames.ColorBlockJamClone.SceneLoad.Runtime.Presenter;
using Zenject;

namespace RollicGames.ColorBlockJamClone.Root.Runtime.Controller
{
    public class RootController : IInitializable
    {
        [Inject] private readonly IPlayerController _playerController;
        [Inject] private readonly ISceneLoader _sceneLoader;

        public async void Initialize()
        {
            _playerController.InitializePlayer();

            await _sceneLoader.Load(SceneNameConstants.Home);
        }
    }
}
