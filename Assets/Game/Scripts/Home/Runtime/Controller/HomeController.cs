using System;
using RollicGames.ColorBlockJamClone.Home.Runtime.Model;
using RollicGames.ColorBlockJamClone.Home.Runtime.View;
using Zenject;

namespace RollicGames.ColorBlockJamClone.Home.Runtime.Controller
{
    public class HomeController : IInitializable, IDisposable
    {
        [Inject] private readonly HomeModel _homeModel;
        [Inject] private readonly IHomeView _homeView;

        public void Initialize()
        {
            SubscribeListeners();
            InitializeHome();
        }

        private void InitializeHome()
        {
            _homeModel.ActiveTab = HomeNavigationType.Home;

            // TODO Get Level and Coin from player inventory!
            var viewData = new HomeViewData()
            {
                PlayerLevel = 15,
                PlayerCoinAmount = 5000,

                StartingTab = HomeNavigationType.Home,
            };

            _homeView.InitializeView(viewData);
        }

        private void NavigateTo(HomeNavigationType to)
        {
            var previousTab = _homeModel.ActiveTab;
            if(to == previousTab) return;

            _homeModel.ActiveTab = to;
            _homeView.NavigateTo(to, previousTab);
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