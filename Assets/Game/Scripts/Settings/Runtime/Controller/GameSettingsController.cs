using Cysharp.Threading.Tasks;
using Zenject;
using RollicGames.UI.Runtime.Presenter;
using RollicGames.ColorBlockJamClone.Settings.Runtime.Model;
using RollicGames.ColorBlockJamClone.Settings.Runtime.View;

namespace RollicGames.ColorBlockJamClone.Settings.Runtime.Controller
{
    public interface IGameSettingsController
    {
        UniTask<SettingsPopupResult> OpenGameSettingsPopupAndWait(bool fromGameplay);
    }

    public class GameSettingsController : IGameSettingsController
    {
        private const string PopupName = "SettingsPopup";

        [Inject] private readonly SettingsModel _settingsModel;
        [Inject] private readonly IPopupPresenter _popupPresenter;

        private UniTaskCompletionSource<SettingsPopupResult> _dismissTcs;
        private IGameSettingsPopupView _activePopupView;

        public async UniTask<SettingsPopupResult> OpenGameSettingsPopupAndWait(bool fromGameplay)
        {
            _activePopupView = await _popupPresenter.LoadPopup<IGameSettingsPopupView>(PopupName);
            if(_activePopupView == null) return SettingsPopupResult.None;

            _activePopupView.InitializeView(new GameSettingsPopupViewData
            (
                _settingsModel.IsAudioOn,
                _settingsModel.IsMusicOn,
                _settingsModel.IsHapticsOn,
                fromGameplay,
                OnAudioClicked,
                OnMusicClicked,
                OnHapticsClicked,
                OnDismissButtonClicked,
                OnHomeButtonClicked
            ));

            _activePopupView.PlayIntroAnimation();

            _dismissTcs = new UniTaskCompletionSource<SettingsPopupResult>();

            return await _dismissTcs.Task;
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
            DismissPopup();
            _dismissTcs?.TrySetResult(SettingsPopupResult.None);
        }

        private void OnHomeButtonClicked()
        {
            DismissPopup();
            _dismissTcs?.TrySetResult(SettingsPopupResult.ReturnHome);
        }

        private void DismissPopup()
        {
            if(_activePopupView != null)
            {
                _popupPresenter.DismissPopup(_activePopupView);
                _activePopupView = null;
            }
        }
    }
}