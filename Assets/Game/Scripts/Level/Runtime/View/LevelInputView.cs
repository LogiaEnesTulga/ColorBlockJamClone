using System;
using RollicGames.Math.Runtime.Model;
using UnityEngine;

namespace RollicGames.ColorBlockJamClone.Level.Runtime.View
{
    public interface ILevelInputView
    {
        public event Action<int, int2> OnBlockClickStarted;
        public event Action<int2> OnTouchMoved;
        public event Action OnTouchEnded;
    }

    public class LevelInputView : MonoBehaviour, ILevelInputView
    {
        [SerializeField] private Camera _levelCamera;

        private bool _isTouched = false;
        private int _lastTouchId;

        private Vector2 _touchPosition;

        public event Action<int, int2> OnBlockClickStarted;
        public event Action<int2> OnTouchMoved;
        public event Action OnTouchEnded;

        private void Update()
        {
            if(_isTouched)
            {
                if(Input.GetMouseButton(0))
                {
                    _touchPosition = Input.mousePosition;
                    FireTouchGridPosition();
                }
                else if (Input.GetMouseButtonUp(0))
                {
                    _isTouched = false;
                    _touchPosition = Input.mousePosition;
                    FireTouchGridPosition();
                    OnTouchEnded?.Invoke();
                }
                else
                {
                    for (int i = 0; i < Input.touchCount; i++)
                    {
                        Touch touch = Input.GetTouch(i);

                        if (touch.fingerId == _lastTouchId)
                        {
                            _touchPosition = touch.position;

                            if(touch.phase == TouchPhase.Ended)
                            {
                                _isTouched = false;
                                FireTouchGridPosition();
                                OnTouchEnded?.Invoke();
                                return;
                            }
                            
                            FireTouchGridPosition();
                            break;
                        }
                    }

                    _isTouched = false;
                    OnTouchEnded?.Invoke();
                }

                return;
            }
            
            if (Input.GetMouseButtonDown(0))
            {
                _isTouched = true;
                _touchPosition = Input.mousePosition;
            }
            else if (Input.touchCount > 0)
            {
                var touch = Input.GetTouch(0);
                if(touch.phase == TouchPhase.Began)
                {
                    _isTouched = true;
                    _lastTouchId = touch.fingerId;
                    _touchPosition = touch.position;
                }
            }
            else return;
            if(!_isTouched) return;

            Ray ray = _levelCamera.ScreenPointToRay(_touchPosition);
            if (Physics.Raycast(ray, out RaycastHit hit))
            {
                var parent = hit.transform.parent;
                if(parent == null) return;

                var clickedBlock = parent.GetComponent<LevelBlockObjectView>();
                if(clickedBlock == null) return;

                OnBlockClickStarted?.Invoke(clickedBlock.Id, GetGridPositionOfTouch());
            }
        }

        private void FireTouchGridPosition()
        {
            OnTouchMoved?.Invoke(GetGridPositionOfTouch());
        }

        private int2 GetGridPositionOfTouch()
        {
            Ray ray = _levelCamera.ScreenPointToRay(_touchPosition);
            var distance = (-0.84f - ray.origin.z) / ray.direction.z;
            var onGridPoint = ray.GetPoint(distance);
            return new int2((int)System.Math.Floor((onGridPoint.x + 1f) * 0.5d), -(int)System.Math.Floor((onGridPoint.y + 1f) * 0.5d));
        }
    }
}