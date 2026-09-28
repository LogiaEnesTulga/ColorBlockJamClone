using Zenject;
using Cysharp.Threading.Tasks;
using RollicGames.UI.Runtime.View;
using RollicGames.AddressableLoading.Runtime.View;
using UnityEngine;

namespace RollicGames.UI.Runtime.Presenter
{
    public interface IPopupPresenter
    {
        UniTask<T> LoadPopup<T>(string name) where T : class, IPopupView;
        void DismissPopup(IPopupView popupView);

        void SetContextPopupHandler(IContextPopupHandler contextPopupHandler);
    }

    public class PopupPresenter : IPopupPresenter
    {
        [Inject] private readonly IAddressableLoader _addressableLoader;

        private IContextPopupHandler _contextPopupHandler;

        public async UniTask<T> LoadPopup<T>(string name) where T : class, IPopupView
        {
            if(_contextPopupHandler == null) return null;

            var popupParent = _contextPopupHandler.PopupParent;
            popupParent.gameObject.SetActive(true);

            var gameObject = await _addressableLoader.LoadPrefab(name, popupParent);
            var popupView = gameObject?.GetComponent<PopupView>() as T;
            if(popupView == null)
            {
                _addressableLoader.Release(gameObject);
                popupParent.gameObject.SetActive(false);
                return null;
            }

            return popupView;
        }

        public void DismissPopup(IPopupView popupView)
        {
            if(popupView is not MonoBehaviour obj) return;
            
            _addressableLoader.Release(obj.gameObject);

            _contextPopupHandler?.PopupParent.gameObject.SetActive(false);
        }

        public void SetContextPopupHandler(IContextPopupHandler contextPopupHandler)
        {
            _contextPopupHandler = contextPopupHandler;
        }
    }
}