using System;
using RollicGames.ColorBlockJamClone.Home.Runtime.Model;
using RollicGames.UI.Runtime.View;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

namespace RollicGames.ColorBlockJamClone.Home.Runtime.View
{
    public interface IHomeView
    {
        public void InitializeView(HomeViewData viewData);
        public void NavigateTo(HomeNavigationType to, HomeNavigationType from = HomeNavigationType.None, bool animate = true);

        public event Action<HomeNavigationType> OnNavigationTabClick;
    }

    public class HomeView : MonoBehaviour, IHomeView, IContextPopupHandler
    {
        [SerializeField] private TMP_Text _coinText;
        [SerializeField] private TMP_Text _levelText;

        [SerializeField] private Button _settingsButton;

        [SerializeField] private HomeNavigationView _navigationView;
        [SerializeField] private Transform _popupParent;

        private Action _onSettingsButtonClick;
        public event Action<HomeNavigationType> OnNavigationTabClick;

        public Transform PopupParent => _popupParent;

        public void InitializeView(HomeViewData viewData)
        {
            _navigationView.InitializeView(OnNavigationClicked);

            SetCoinText(viewData.PlayerCoinAmount);
            _levelText.SetText($"Level {viewData.PlayerLevel}");

            _onSettingsButtonClick += viewData.OnSettingsButtonClick;
            _settingsButton.onClick.AddListener(OnSettingsButtonClicked);

            _navigationView.NavigateTo(viewData.StartingTab, animate: false);
        }

        private void SetCoinText(int amount)
        {
            _coinText.SetText(TextFormat.FormatCoinAmount(amount));
        }

        public void NavigateTo(HomeNavigationType to, HomeNavigationType from = HomeNavigationType.None, bool animate = true)
        {
            _navigationView.NavigateTo(to, from, animate);
        }

        private void OnNavigationClicked(HomeNavigationType clickedTab)
        {
            OnNavigationTabClick?.Invoke(clickedTab);
        }

        private void OnSettingsButtonClicked()
        {
            _onSettingsButtonClick?.Invoke();
        }
    }
}