using System;
using RollicGames.ColorBlockJamClone.Home.Runtime.Model;
using RollicGames.ColorBlockJamClone.Home.Runtime.View;
using RollicGames.ColorBlockJamClone.Player.Runtime.Controller;
using RollicGames.ColorBlockJamClone.Settings.Runtime.Controller;
using Zenject;

namespace RollicGames.ColorBlockJamClone.Home.Runtime.Controller
{
    public class HomeController : IInitializable, IDisposable
    {
        [Inject] private readonly HomeModel _homeModel;
        [Inject] private readonly IHomeView _homeView;
        [Inject] private readonly IPlayerController _playerController;
        [Inject] private readonly IGameSettingsController _gameSettingsController;

        public void Initialize()
        {
            SubscribeListeners();
            InitializeHome();
        }

        private void InitializeHome()
        {
            _homeModel.ActiveTab = HomeNavigationType.Home;

            var viewData = new HomeViewData()
            {
                PlayerLevel = _playerController.GetLevel(),
                PlayerCoinAmount = _playerController.GetCoin(),

                StartingTab = HomeNavigationType.Home,

                OnSettingsButtonClick = OnSettingsButtonClicked,
            };

            _homeView.InitializeView(viewData);
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
            _homeModel.IsHomeInteractable = false;

            await _gameSettingsController.OpenGameSettingsPopupAndWait(false);

            _homeModel.IsHomeInteractable = true;
        }

        private void SubscribeListeners()
        {
            _homeView.OnNavigationTabClick += NavigateTo;
        }

        private void UnsubscribeListeners()
        {
            _homeView.OnNavigationTabClick -= NavigateTo;
        }

        public void Dispose()
        {
            UnsubscribeListeners();
        }
    }
}