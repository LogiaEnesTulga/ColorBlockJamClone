using RollicGames.ColorBlockJamClone.SceneLoad.Runtime.Model;
using RollicGames.ColorBlockJamClone.SceneLoad.Runtime.Presenter;
using RollicGames.ColorBlockJamClone.Home.Runtime.Model;
using RollicGames.ColorBlockJamClone.Home.Runtime.View;
using RollicGames.ColorBlockJamClone.Player.Runtime.Controller;
using RollicGames.ColorBlockJamClone.Settings.Runtime.Controller;
using Zenject;

namespace RollicGames.ColorBlockJamClone.Home.Runtime.Controller
{
    public class HomeController : IInitializable, ISceneHandler
    {
        [Inject] private readonly HomeModel _homeModel;
        [Inject] private readonly IHomeView _homeView;
        [Inject] private readonly IPlayerController _playerController;
        [Inject] private readonly IGameSettingsController _gameSettingsController;
        [Inject] private readonly ISceneLoader _sceneLoader;

        public string SceneName => SceneNameConstants.Home;

        public void Initialize()
        {
            _sceneLoader.RegisterHandler(this);
            _homeModel.ActiveTab = HomeNavigationType.Home;
            _homeView.InitializeView(GetViewData());
        }

        public void OnSceneReactivated()
        {
            _homeModel.IsHomeInteractable = true;
            _homeView.PrepareForReuse();

            Initialize();
        }

        private HomeViewData GetViewData()
        {
            return new HomeViewData()
            {
                PlayerLevel = _playerController.GetLevel(),
                PlayerCoinAmount = _playerController.GetCoin(),

                StartingTab = HomeNavigationType.Home,

                OnLevelButtonClick = OnLevelButtonClicked,
                OnSettingsButtonClick = OnSettingsButtonClicked,
                OnNavigationTabClick = NavigateTo,
            };
        }

        private void NavigateTo(HomeNavigationType to)
        {
            if(!_homeModel.IsHomeInteractable) return;
            
            var previousTab = _homeModel.ActiveTab;
            if(to == previousTab) return;

            _homeModel.ActiveTab = to;
            _homeView.NavigateTo(to, previousTab);
        }

        private async void OnSettingsButtonClicked()
        {
            if(!_homeModel.IsHomeInteractable) return;

            _homeModel.IsHomeInteractable = false;

            await _gameSettingsController.OpenGameSettingsPopupAndWait(false);

            _homeModel.IsHomeInteractable = true;
        }

        private async void OnLevelButtonClicked()
        {
            if(!_homeModel.IsHomeInteractable) return;
            
            _homeModel.IsHomeInteractable = false;

            await _sceneLoader.Load(SceneNameConstants.Level);
        }
    }
}