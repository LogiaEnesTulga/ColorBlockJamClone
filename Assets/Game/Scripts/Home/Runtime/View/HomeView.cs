using System;
using RollicGames.ColorBlockJamClone.Home.Runtime.Model;
using UnityEngine;
using TMPro;

namespace RollicGames.ColorBlockJamClone.Home.Runtime.View
{
    public interface IHomeView
    {
        public void InitializeView(HomeViewData viewData);
        public void NavigateTo(HomeNavigationType to, HomeNavigationType from = HomeNavigationType.None, bool animate = true);

        public event Action<HomeNavigationType> OnNavigationTabClick;
    }

    public class HomeView : MonoBehaviour, IHomeView
    {
        [SerializeField] private TMP_Text _coinText;
        [SerializeField] private TMP_Text _levelText;

        [SerializeField] private HomeNavigationView _navigationView;

        public event Action<HomeNavigationType> OnNavigationTabClick;

        public void InitializeView(HomeViewData viewData)
        {
            _navigationView.InitializeView(OnNavigationClicked);

            // TODO : Format Coin Text
            _coinText.SetText(viewData.PlayerCoinAmount.ToString());
            _levelText.SetText($"Level {viewData.PlayerLevel}");

            _navigationView.NavigateTo(viewData.StartingTab, animate: false);
        }

        public void NavigateTo(HomeNavigationType to, HomeNavigationType from = HomeNavigationType.None, bool animate = true)
        {
            _navigationView.NavigateTo(to, from, animate);
        }

        private void OnNavigationClicked(HomeNavigationType clickedTab)
        {
            OnNavigationTabClick?.Invoke(clickedTab);
        }
    }
}