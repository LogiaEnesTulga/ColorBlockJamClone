using System;
using Zenject;
using RollicGames.UI.Runtime.View;
using RollicGames.UI.Runtime.Presenter;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

namespace RollicGames.ColorBlockJamClone.Level.Runtime.View
{
    public interface ILevelView
    {
        void InitializeView(LevelViewData viewData);
        void PrepareForReuse();
    }

    public class LevelView : MonoBehaviour, ILevelView, IContextPopupHandler
    {
        [SerializeField] private TMP_Text _coinText;
        [SerializeField] private TMP_Text _levelText;
        [SerializeField] private TMP_Text _timerText;

        [SerializeField] private Button _pauseButton;

        [SerializeField] private Camera _levelCamera;

        [SerializeField] private Transform _popupParent;

        [Inject] private readonly IPopupPresenter _popupPresenter;

        private Action _onPauseButtonClick;

        public Transform PopupParent => _popupParent;

        public void InitializeView(LevelViewData viewData)
        {
            PrepareCamera(viewData.LevelWidth, viewData.LevelHeight);
            
            _popupPresenter.SetContextPopupHandler(this);
            
            SetLevelText(viewData.PlayerLevel);
            SetCoinText(viewData.PlayerCoinAmount);

            SubscribeListeners(viewData);
        }

        public void PrepareForReuse()
        {
            RemoveListeners();
        }

        private void SubscribeListeners(LevelViewData viewData)
        {
            _onPauseButtonClick += viewData.OnPauseButtonClick;
            _pauseButton.onClick.AddListener(OnPauseButtonClicked);
        }

        private void RemoveListeners()
        {
            _onPauseButtonClick = null;
            _pauseButton.onClick.RemoveAllListeners();
        }

        private void PrepareCamera(int width, int height)
        {
            var biggerEdge = width > height ? width : height;

            _levelCamera.transform.position = new Vector3(width - 1f, -2f * height, -4.6f * biggerEdge);
        }

        private void SetLevelText(int level)
        {
            _levelText.SetText($"Level<br><size=100>{level}");
        }

        private void SetCoinText(int amount)
        {
            _coinText.SetText(TextFormat.FormatCoinAmount(amount));
        }

        private void OnPauseButtonClicked()
        {
            _onPauseButtonClick?.Invoke();
        }
    }
}