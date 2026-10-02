using RollicGames.Math.Runtime.Model;
using RollicGames.ColorBlockJamClone.Level.Runtime.Model;
using UnityEngine;
using DG.Tweening;
using Cysharp.Threading.Tasks;

namespace RollicGames.ColorBlockJamClone.Level.Runtime.View
{
    public class LevelDoorView : MonoBehaviour
    {
        private const float ClosingDuration = 0.5f;

        [SerializeField] private MeshRenderer _doorRenderer;
        [SerializeField] private MeshRenderer _arrowRenderer;

        private Tween closingAnimation;

        public void InitializeView(int2 gridPosition, LevelDirection direction, int length, Color color, Color arrowColor)
        {
            ResetAnimation();
            LevelViewHelper.ApplyColorProperty(_doorRenderer, color);
            LevelViewHelper.ApplyColorProperty(_arrowRenderer, arrowColor);

            var cellLength = LevelViewHelper.CellLength;

            _doorRenderer.transform.localScale = new Vector3(length * cellLength * 0.5f, 0.5f, 1f);
            switch(direction)
            {
                case LevelDirection.Up:
                    transform.localPosition = new Vector3(gridPosition.X + (length -1f) * 0.5f, -gridPosition.Y - 0.25f, 0f) * cellLength;
                    transform.localRotation = Quaternion.Euler(0f, 0f , 180f);
                    break;

                case LevelDirection.Down:
                    transform.localPosition = new Vector3(gridPosition.X + (length -1f) * 0.5f, -gridPosition.Y+ 0.25f, 0f) * cellLength;
                    transform.localRotation = Quaternion.Euler(0f, 0f , 0f);
                    break;
                
                case LevelDirection.Left:
                    transform.localPosition = new Vector3(gridPosition.X + 0.25f, -gridPosition.Y + (1 - length) * 0.5f, 0f) * cellLength;
                    transform.localRotation = Quaternion.Euler(0f, 0f , 270f);
                    break;
                
                case LevelDirection.Right:
                    transform.localPosition = new Vector3(gridPosition.X - 0.25f, -gridPosition.Y + (1 - length) * 0.5f, 0f) * cellLength;
                    transform.localRotation = Quaternion.Euler(0f, 0f , 90f);
                    break;
            }
        }

        public void SetDoorOpen()
        {
            ResetAnimation();

            var currentPosition = transform.localPosition;
            currentPosition.z = 2f;
            transform.localPosition = currentPosition;
        }

        public async UniTask StartClosingAnimation()
        {
            ResetAnimation();
            var currentPosition = transform.localPosition;
            currentPosition.z = 0f;
            closingAnimation = transform.DOLocalMove(currentPosition, ClosingDuration);

            await closingAnimation.AsyncWaitForCompletion();
        }

        private void ResetAnimation()
        {
            closingAnimation?.Kill();
            closingAnimation = null;
        }
    }
}