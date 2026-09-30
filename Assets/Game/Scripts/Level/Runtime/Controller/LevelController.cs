using System;
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
    public class LevelController : IInitializable, IDisposable, ISceneHandler
    {
        [Inject] private readonly LevelModel _levelModel;
        [Inject] private readonly ILevelView _levelView;
        [Inject] private readonly ILevelGridController _gridController;
        [Inject] private readonly ILevelTimerController _timerController;
        [Inject] private readonly ILevelFlowController _flowController;
        [Inject] private readonly ILevelFailPopupController _failPopupController;
        [Inject] private readonly IPlayerController _playerController;
        [Inject] private readonly IGameSettingsController _gameSettingsController;
        [Inject] private readonly ISceneLoader _sceneLoader;

        public string SceneName => SceneNameConstants.Level;

        public void Initialize()
        {
            _sceneLoader.RegisterHandler(this);
            _flowController.OnLevelFailed += OnLevelFailed;
            InitializeLevel();
        }

        public void OnSceneReactivated()
        {
            PrepareForReuse();
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
                OnRetryButtonClick = RestartLevel,
            };
            _levelView.InitializeView(viewData);

            _levelModel.RemainingDuration = 10f;
            _timerController.InitializeTimer();
        }

        private void PrepareForReuse()
        {
            _levelModel.IsLevelStarted = false;
            _levelModel.IsLevelPaused = false;

            _flowController.PrepareForReuse();
            _timerController.PrepareForReuse();

            _levelView.PrepareForReuse();

            _gridController.PrepareForReuse();
        }

        private void RestartLevel()
        {
            PrepareForReuse();
            InitializeLevel();
        }

        private async void OnLevelFailed()
        {
            _levelModel.IsLevelPaused = true;

            var result = await _failPopupController.OpenLevelFailPopupAndWait(_playerController.GetLevel());

            if(result == LevelFailPopupResult.ReturnHome)
            {
                await _sceneLoader.Load(SceneNameConstants.Home);
                return;
            }

            RestartLevel();
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

        public void Dispose()
        {
            _flowController.OnLevelFailed -= OnLevelFailed;
        }
    }
}