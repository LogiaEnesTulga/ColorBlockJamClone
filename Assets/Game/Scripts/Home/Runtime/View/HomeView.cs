using System;
using Zenject;
using RollicGames.UI.Runtime.View;
using RollicGames.UI.Runtime.Presenter;
using RollicGames.ColorBlockJamClone.Home.Runtime.Model;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

namespace RollicGames.ColorBlockJamClone.Home.Runtime.View
{
    public interface IHomeView
    {
        public void InitializeView(HomeViewData viewData);
        public void NavigateTo(HomeNavigationType to, HomeNavigationType from = HomeNavigationType.None, bool animate = true);
    }

    public class HomeView : MonoBehaviour, IHomeView, IContextPopupHandler
    {
        [SerializeField] private TMP_Text _coinText;
        [SerializeField] private TMP_Text _levelText;

        [SerializeField] private Button _levelButton;
        [SerializeField] private Button _settingsButton;

        [SerializeField] private HomeNavigationView _navigationView;
        [SerializeField] private Transform _popupParent;

        [Inject] private readonly IPopupPresenter _popupPresenter;

        private Action _onLevelButtonClick;
        private Action _onSettingsButtonClick;
        private Action<HomeNavigationType> _onNavigationTabClick;

        public Transform PopupParent => _popupParent;

        public void InitializeView(HomeViewData viewData)
        {
            _popupPresenter.SetContextPopupHandler(this);

            _navigationView.InitializeView(OnNavigationClicked);

            SetCoinText(viewData.PlayerCoinAmount);
            _levelText.SetText($"Level {viewData.PlayerLevel}");

            _onLevelButtonClick += viewData.OnLevelButtonClick;
            _settingsButton.onClick.AddListener(OnSettingsButtonClicked);

            _onSettingsButtonClick += viewData.OnSettingsButtonClick;
            _levelButton.onClick.AddListener(OnLevelButtonClicked);

            _onNavigationTabClick += viewData.OnNavigationTabClick;

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

        private void OnLevelButtonClicked()
        {
            _onLevelButtonClick?.Invoke();
        }

        private void OnSettingsButtonClicked()
        {
            _onSettingsButtonClick?.Invoke();
        }

        private void OnNavigationClicked(HomeNavigationType clickedTab)
        {
            _onNavigationTabClick?.Invoke(clickedTab);
        }
    }
}