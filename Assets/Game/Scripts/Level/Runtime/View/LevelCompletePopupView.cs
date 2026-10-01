using System;
using RollicGames.UI.Runtime.View;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using TMPro;

namespace RollicGames.ColorBlockJamClone.Level.Runtime.View
{
    public interface ILevelCompletePopupView : IPopupView
    {
        void InitializeView(LevelCompletePopupViewData viewData);
        void PlayIntroAnimation();
    }

    public class LevelCompletePopupView : PopupView, ILevelCompletePopupView
    {
        [SerializeField] private TMP_Text _levelText;
        [SerializeField] private TMP_Text _coinText;

        [SerializeField] private Button _dismissButton;
        [SerializeField] private Button _nextLevelButton;

        private Action _onDismissButtonClicked;
        private Action _onNextLevelButtonClicked;

        public void InitializeView(LevelCompletePopupViewData viewData)
        {
            _levelText.SetText($"Level {viewData.Level}");
            _coinText.SetText($"+ {TextFormat.FormatCoinAmount(viewData.CoinAmount)}");

            _dismissButton.onClick.AddListener(OnDismissButtonClick);
            _onDismissButtonClicked += viewData.OnDismissButtonClicked;

            _nextLevelButton.onClick.AddListener(OnRetryButtonClick);
            _onNextLevelButtonClicked += viewData.OnNextLevelButtonClicked;
        }

        public void PlayIntroAnimation()
        {
            transform.localScale = Vector3.zero;
            transform.DOScale(Vector3.one, 0.6f).SetEase(Ease.OutBack);
        }

        private void OnDismissButtonClick()
        {
            _onDismissButtonClicked?.Invoke();
        }

        private void OnRetryButtonClick()
        {
            _onNextLevelButtonClicked?.Invoke();
        }
    }
}
