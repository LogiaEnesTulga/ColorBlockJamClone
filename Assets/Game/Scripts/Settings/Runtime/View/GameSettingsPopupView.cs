using System;
using RollicGames.UI.Runtime.View;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

namespace RollicGames.ColorBlockJamClone.Settings.Runtime.View
{
    public interface IGameSettingsPopupView : IPopupView
    {
        void InitializeView(GameSettingsPopupViewData viewData);
        void PlayIntroAnimation();
        void UpdateAudio(bool enabled);
        void UpdateMusic(bool enabled);
        void UpdateHaptics(bool enabled);
    }
    
    public class GameSettingsPopupView : PopupView, IGameSettingsPopupView
    {
        [SerializeField] private ToggleButton _audioToggleButton;
        [SerializeField] private ToggleButton _musicToggleButton;
        [SerializeField] private ToggleButton _hapticsToggleButton;

        [SerializeField] private Button _homeButton;
        [SerializeField] private Button _dismissButton;

        [SerializeField] private Transform _homeButtonParent;
        [SerializeField] private Transform _defaultContentParent;

        private Action _onHomeButtonClicked;
        private Action _onDismissButtonClicked;

        public void InitializeView(GameSettingsPopupViewData viewData)
        {
            _audioToggleButton.Switch(viewData.IsAudioOn);
            _musicToggleButton.Switch(viewData.IsMusicOn);
            _hapticsToggleButton.Switch(viewData.IsHapticsOn);

            _audioToggleButton.SubscribeListener(viewData.OnAudioToggleClicked);
            _musicToggleButton.SubscribeListener(viewData.OnMusicToggleClicked);
            _hapticsToggleButton.SubscribeListener(viewData.OnHapticsToggleClicked);

            _homeButton.onClick.AddListener(OnHomeButtonClicked);
            _onHomeButtonClicked += viewData.OnHomeButtonClicked;

            _dismissButton.onClick.AddListener(OnDismissButtonClick);
            _onDismissButtonClicked += viewData.OnDismissButtonClicked;

            _homeButtonParent.gameObject.SetActive(viewData.IsOpenedFromGameplay);
            _defaultContentParent.gameObject.SetActive(!viewData.IsOpenedFromGameplay);
        }

        public void PlayIntroAnimation()
        {
            transform.localScale = Vector3.zero;
            transform.DOScale(Vector3.one, 0.6f).SetEase(Ease.OutBack);
        }

        public void UpdateAudio(bool enabled)
        {
            _audioToggleButton.Switch(enabled);
        }

        public void UpdateMusic(bool enabled)
        {
            _musicToggleButton.Switch(enabled);
        }

        public void UpdateHaptics(bool enabled)
        {
            _hapticsToggleButton.Switch(enabled);
        }

        private void OnHomeButtonClicked()
        {
            _onHomeButtonClicked?.Invoke();
        }

        private void OnDismissButtonClick()
        {
            _onDismissButtonClicked?.Invoke();
        }
    }
}