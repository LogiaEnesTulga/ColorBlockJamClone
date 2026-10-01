using Cysharp.Threading.Tasks;
using RollicGames.UI.Runtime.Presenter;
using RollicGames.ColorBlockJamClone.Level.Runtime.Model;
using RollicGames.ColorBlockJamClone.Level.Runtime.View;
using Zenject;

namespace RollicGames.ColorBlockJamClone.Level.Runtime.Controller
{
    public interface ILevelCompletePopupController
    {
        UniTask<LevelCompletePopupResult> OpenLevelCompletePopupAndWait(int level, int coinAmount);
    }

    public class LevelCompletePopupController : ILevelCompletePopupController
    {
        private const string PopupName = "LevelCompletePopup";

        [Inject] private readonly IPopupPresenter _popupPresenter;

        private UniTaskCompletionSource<LevelCompletePopupResult> _resultTcs;
        private ILevelCompletePopupView _activePopupView;

        public async UniTask<LevelCompletePopupResult> OpenLevelCompletePopupAndWait(int level, int coinAmount)
        {
            _activePopupView = await _popupPresenter.LoadPopup<ILevelCompletePopupView>(PopupName);
            if(_activePopupView == null) return LevelCompletePopupResult.None;

            _activePopupView.InitializeView(new LevelCompletePopupViewData
            (
                level,
                coinAmount,
                OnDismissButtonClicked,
                OnNextLevelClicked
            ));

            _activePopupView.PlayIntroAnimation();

            _resultTcs = new UniTaskCompletionSource<LevelCompletePopupResult>();

            return await _resultTcs.Task;
        }

        private void OnDismissButtonClicked()
        {
            DismissPopup();
            _resultTcs?.TrySetResult(LevelCompletePopupResult.ReturnHome);
        }

        private void OnNextLevelClicked()
        {
            DismissPopup();
            _resultTcs?.TrySetResult(LevelCompletePopupResult.NextLevel);
        }

        private void DismissPopup()
        {
            if(_activePopupView == null) return;

            _popupPresenter.DismissPopup(_activePopupView);
            _activePopupView = null;
        }
    }
}
