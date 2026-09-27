using RollicGames.UI.Runtime.View;
using UnityEngine;
using UnityEngine.UI;

namespace RollicGames.ColorBlockJamClone.Settings.Runtime.View
{
    public class GameSettingsPopupView : MonoBehaviour
    {
        [SerializeField] private ToggleButton _audioToggleButton;
        [SerializeField] private ToggleButton _musicToggleButton;
        [SerializeField] private ToggleButton _hapticsToggleButton;

        [SerializeField] private Button _homeButton;
        [SerializeField] private Button _dismissButton;

        [SerializeField] private Transform _homeButtonParent;
        [SerializeField] private Transform _defaultContentParent;

        public void InitializeView(GameSettingsPopupViewData viewData)
        {
            _audioToggleButton.Switch(viewData.IsAudioOn);
            _musicToggleButton.Switch(viewData.IsMusicOn);
            _hapticsToggleButton.Switch(viewData.IsHapticsOn);

            _homeButtonParent.gameObject.SetActive(viewData.IsOpenedFromGameplay);
            _defaultContentParent.gameObject.SetActive(!viewData.IsOpenedFromGameplay);
        }
    }
}