using Cysharp.Threading.Tasks;
using RollicGames.UI.Runtime.Presenter;
using RollicGames.ColorBlockJamClone.Level.Runtime.Model;
using RollicGames.ColorBlockJamClone.Level.Runtime.View;
using Zenject;

namespace RollicGames.ColorBlockJamClone.Level.Runtime.Controller
{
    public interface ILevelFailPopupController
    {
        UniTask<LevelFailPopupResult> OpenLevelFailPopupAndWait(int level);
    }

    public class LevelFailPopupController : ILevelFailPopupController
    {
        private const string PopupName = "LevelFailPopup";

        [Inject] private readonly IPopupPresenter _popupPresenter;

        private UniTaskCompletionSource<LevelFailPopupResult> _resultTcs;
        private ILevelFailPopupView _activePopupView;

        public async UniTask<LevelFailPopupResult> OpenLevelFailPopupAndWait(int level)
        {
            _activePopupView = await _popupPresenter.LoadPopup<ILevelFailPopupView>(PopupName);
            if(_activePopupView == null) return LevelFailPopupResult.None;

            _activePopupView.InitializeView(new LevelFailPopupViewData
            (
                level,
                OnDismissButtonClicked,
                OnRetryButtonClicked
            ));

            _activePopupView.PlayIntroAnimation();

            _resultTcs = new UniTaskCompletionSource<LevelFailPopupResult>();

            return await _resultTcs.Task;
        }

        private void OnDismissButtonClicked()
        {
            DismissPopup();
            _resultTcs?.TrySetResult(LevelFailPopupResult.ReturnHome);
        }

        private void OnRetryButtonClicked()
        {
            DismissPopup();
            _resultTcs?.TrySetResult(LevelFailPopupResult.Retry);
        }

        private void DismissPopup()
        {
            if(_activePopupView == null) return;

            _popupPresenter.DismissPopup(_activePopupView);
            _activePopupView = null;
        }
    }
}
