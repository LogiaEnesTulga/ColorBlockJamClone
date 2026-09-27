using System;
using UnityEngine;
using UnityEngine.UI;

namespace RollicGames.UI.Runtime.View
{
    public class ToggleButton : MonoBehaviour
    {
        [SerializeField] private Image _background;
        [SerializeField] private Image _toggleIcon;

        [SerializeField] private Button _button;

        [SerializeField] private Transform _activeTogglePoint;
        [SerializeField] private Transform _disabledTogglePoint;

        [SerializeField] private Sprite _activeToggleSprite;
        [SerializeField] private Sprite _disabledToggleSprite;

        [SerializeField] private Color _activeBackgroundColor;
        [SerializeField] private Color _disabledBackgroundColor;

        private event Action _onToggleClicked;

        private void Awake()
        {
            _button.onClick.AddListener(OnToggleClicked);
        }

        public void Switch(bool enable)
        {
            _background.color = enable ? _activeBackgroundColor : _disabledBackgroundColor;

            _toggleIcon.sprite = enable ? _activeToggleSprite : _disabledToggleSprite;
            _toggleIcon.transform.position = enable ? _activeTogglePoint.position : _disabledTogglePoint.position;
        }

        public void SubscribeListener(Action onToggleClicked)
        {
            _onToggleClicked += onToggleClicked;
        }

        private void OnToggleClicked()
        {
            _onToggleClicked?.Invoke();
        }
    }
}