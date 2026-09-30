using System;
using RollicGames.ColorBlockJamClone.Home.Runtime.Model;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

namespace RollicGames.ColorBlockJamClone.Home.Runtime.View
{
    public class HomeNavigationTabView : MonoBehaviour
    {
        private const float AnimationDuration = 0.33f;
        private static readonly Vector3 ActiveTabBackgroundAnimationStartingScale = new Vector3(0.8f, 0.8f, 0.8f);

        [SerializeField] private Transform _activeTabParent;
        [SerializeField] private Transform _activeIconReferenceTransform;
        [SerializeField] private Transform _deactiveIconReferenceTransform;
        [SerializeField] private Transform _iconTransform;
        [SerializeField] private Transform _tabTitleTransform;
        [SerializeField] private Transform _activeTabBackgroundTransform;

        [SerializeField] private Button _tabButton;

        [SerializeField] private HomeNavigationType _tabType;

        private Sequence _enablingAnimationSequence;
        private Action<HomeNavigationType> _tabClickListener;

        public HomeNavigationType TabType => _tabType;

        public void InitializeView(Action<HomeNavigationType> tabClickListener)
        {
            _tabButton.onClick.AddListener(OnTabClicked);
            _tabClickListener = tabClickListener;
        }

        public void PrepareForReuse()
        {
            SetTabDeactive();

            _tabButton.onClick.RemoveAllListeners();
            _tabClickListener = null;

            _enablingAnimationSequence?.Kill();
            _enablingAnimationSequence = null;
        }

        public void SetTabActive(bool animate)
        {
            _activeTabParent.gameObject.SetActive(true);

            if(!animate)
            {
                _iconTransform.position = _activeIconReferenceTransform.position;
                _iconTransform.localScale = _activeIconReferenceTransform.localScale;
                _tabTitleTransform.localScale = Vector3.one;
                _activeTabBackgroundTransform.localScale = Vector3.one;

                return;
            }

            _iconTransform.position = _deactiveIconReferenceTransform.position;
            _iconTransform.localScale = _deactiveIconReferenceTransform.localScale;
            _tabTitleTransform.localScale = Vector3.zero;
            _activeTabBackgroundTransform.localScale = ActiveTabBackgroundAnimationStartingScale;

            StartAnimation();
        }

        public void SetTabDeactive()
        {
            if(_enablingAnimationSequence != null)
            {
                _enablingAnimationSequence.Kill();
                _enablingAnimationSequence = null;
            }

            _iconTransform.position = _deactiveIconReferenceTransform.position;
            _iconTransform.localScale = _deactiveIconReferenceTransform.localScale;
            
            _activeTabParent.gameObject.SetActive(false);
        }

        private void StartAnimation()
        {
            _enablingAnimationSequence = DOTween.Sequence();
            _enablingAnimationSequence.Append(_iconTransform.DOMove(_activeIconReferenceTransform.position, AnimationDuration));
            _enablingAnimationSequence.Join(_iconTransform.DOScale(_activeIconReferenceTransform.localScale, AnimationDuration));
            _enablingAnimationSequence.Join(_tabTitleTransform.DOScale(Vector3.one, AnimationDuration));
            _enablingAnimationSequence.Join(_activeTabBackgroundTransform.DOScale(Vector3.one, AnimationDuration));
        }

        private void OnTabClicked()
        {
            _tabClickListener?.Invoke(_tabType);
        }
    }
}