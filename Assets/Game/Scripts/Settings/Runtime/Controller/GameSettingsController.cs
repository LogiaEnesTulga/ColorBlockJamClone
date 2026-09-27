using Cysharp.Threading.Tasks;
using Zenject;
using RollicGames.UI.Runtime.Presenter;
using RollicGames.ColorBlockJamClone.Settings.Runtime.Model;
using RollicGames.ColorBlockJamClone.Settings.Runtime.View;

namespace RollicGames.ColorBlockJamClone.Settings.Runtime.Controller
{
    public interface IGameSettingsController
    {
        UniTask OpenGameSettingsPopupAndWait(bool fromGameplay);
    }

    public class GameSettingsController : IGameSettingsController
    {
        private const string PopupName = "SettingsPopup";

        [Inject] private readonly SettingsModel _settingsModel;
        [Inject] private readonly IPopupPresenter _popupPresenter;

        private UniTaskCompletionSource _dismissTcs;
        private IGameSettingsPopupView _activePopupView;

        public async UniTask OpenGameSettingsPopupAndWait(bool fromGameplay)
        {
            _activePopupView = await _popupPresenter.LoadPopup<IGameSettingsPopupView>(PopupName);
            if(_activePopupView == null) return;

            _activePopupView.InitializeView(new GameSettingsPopupViewData
            (
                _settingsModel.IsAudioOn,
                _settingsModel.IsMusicOn,
                _settingsModel.IsHapticsOn,
                fromGameplay,
                OnAudioClicked,
                OnMusicClicked,
                OnHapticsClicked,
                OnDismissButtonClicked
            ));

            _activePopupView.PlayIntroAnimation();

            _dismissTcs?.TrySetResult();
            _dismissTcs = new UniTaskCompletionSource();

            await _dismissTcs.Task;
        }

        private void OnAudioClicked()
        {
            _settingsModel.IsAudioOn = !_settingsModel.IsAudioOn;
            _activePopupView.UpdateAudio(_settingsModel.IsAudioOn);
        }

        private void OnMusicClicked()
        {
            _settingsModel.IsMusicOn = !_settingsModel.IsMusicOn;
            _activePopupView.UpdateMusic(_settingsModel.IsMusicOn);
        }

        private void OnHapticsClicked()
        {
            _settingsModel.IsHapticsOn = !_settingsModel.IsHapticsOn;
            _activePopupView.UpdateHaptics(_settingsModel.IsHapticsOn);
        }

        private void OnDismissButtonClicked()
        {
            if(_activePopupView != null)
            {
                _popupPresenter.DismissPopup(_activePopupView);
                _activePopupView = null;
            }

            _dismissTcs?.TrySetResult();
        }
    }
}