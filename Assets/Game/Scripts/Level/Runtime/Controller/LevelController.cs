using RollicGames.ColorBlockJamClone.Player.Runtime.Controller;
using RollicGames.ColorBlockJamClone.Settings.Runtime.Model;
using RollicGames.ColorBlockJamClone.Settings.Runtime.Controller;
using RollicGames.ColorBlockJamClone.SceneLoad.Runtime.Model;
using RollicGames.ColorBlockJamClone.SceneLoad.Runtime.Presenter;
using RollicGames.ColorBlockJamClone.Level.Runtime.Model;
using RollicGames.ColorBlockJamClone.Level.Runtime.View;
using Zenject;

namespace RollicGames.ColorBlockJamClone.Level.Runtime.Controller
{
    public class LevelController : IInitializable
    {
        [Inject] private readonly LevelModel _levelModel;
        [Inject] private readonly ILevelView _levelView;
        [Inject] private readonly ILevelGridController _gridController;
        [Inject] private readonly IPlayerController _playerController;
        [Inject] private readonly IGameSettingsController _gameSettingsController;
        [Inject] private readonly ISceneLoader _sceneLoader;

        public void Initialize()
        {
            InitializeLevel();
        }

        private void InitializeLevel()
        {
            _gridController.InitializeGrid();

            var viewData = new LevelViewData()
            {
                PlayerLevel = _playerController.GetLevel(),
                PlayerCoinAmount = _playerController.GetCoin(),

                LevelWidth = _gridController.LevelWidth,
                LevelHeight = _gridController.LevelHeight,

                OnPauseButtonClick = OnPauseButtonClicked,
            };
            _levelView.InitializeView(viewData);
        }

        private async void OnPauseButtonClicked()
        {
            _levelModel.IsLevelPaused = true;

            var result = await _gameSettingsController.OpenGameSettingsPopupAndWait(true);

            if(result == SettingsPopupResult.ReturnHome)
            {
                await _sceneLoader.Load(SceneNameConstants.Home);
                return;
            }

            _levelModel.IsLevelPaused = false;
        }
    }
}