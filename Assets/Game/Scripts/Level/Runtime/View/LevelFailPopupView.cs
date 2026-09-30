using System;
using RollicGames.UI.Runtime.View;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using TMPro;

namespace RollicGames.ColorBlockJamClone.Level.Runtime.View
{
    public interface ILevelFailPopupView : IPopupView
    {
        void InitializeView(LevelFailPopupViewData viewData);
        void PlayIntroAnimation();
    }

    public class LevelFailPopupView : PopupView, ILevelFailPopupView
    {
        [SerializeField] private TMP_Text _levelText;

        [SerializeField] private Button _dismissButton;
        [SerializeField] private Button _retryButton;

        private Action _onDismissButtonClicked;
        private Action _onRetryButtonClicked;

        public void InitializeView(LevelFailPopupViewData viewData)
        {
            _levelText.SetText($"Level {viewData.Level}");

            _dismissButton.onClick.AddListener(OnDismissButtonClick);
            _onDismissButtonClicked += viewData.OnDismissButtonClicked;

            _retryButton.onClick.AddListener(OnRetryButtonClick);
            _onRetryButtonClicked += viewData.OnRetryButtonClicked;
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
            _onRetryButtonClicked?.Invoke();
        }
    }
}
