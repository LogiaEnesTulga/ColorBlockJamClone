using System;
using RollicGames.ColorBlockJamClone.Home.Runtime.Model;

namespace RollicGames.ColorBlockJamClone.Home.Runtime.View
{
    public struct HomeViewData
    {
        public int PlayerLevel;
        public int PlayerCoinAmount;

        public HomeNavigationType StartingTab;

        public Action OnLevelButtonClick;
        public Action OnSettingsButtonClick;
        public Action<HomeNavigationType> OnNavigationTabClick;
    }    
}
