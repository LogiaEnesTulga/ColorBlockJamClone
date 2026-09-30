using System.Collections.Generic;
using RollicGames.Math.Runtime.Model;
using RollicGames.Pooling.Runtime.View;
using UnityEngine;
using DG.Tweening;

namespace RollicGames.ColorBlockJamClone.Level.Runtime.View
{
    public class LevelBlockObjectView : MonoBehaviour
    {
        private const float MoveDuration = 0.05f;
        private const float BlockedDragLimit = 0.15f;

        [SerializeField] private float _cornerLength = 2f;

        private Color _color;
        private Tween _moveTween;

        private Vector3 _targetPosition;
        private Vector3? _dragOrigin;

        public void InitializeView(Color color, IReadOnlyList<int2> localPositions,
            IViewPool<Transform> centerPartPool, IViewPool<Transform> edgePartPool,
            IViewPool<Transform> outerCornerPartPool, IViewPool<Transform> innerCornerPartPool,
            IViewPool<Transform> colliderPool)
        {
            _color = color;

            for(var i = 0; i < localPositions.Count; i++)
            {
                var localPosition = localPositions[i];
                var createdMesh = colliderPool.Spawn(transform);
                createdMesh.transform.localPosition = new Vector3(localPosition.X, -localPosition.Y, 0f) * _cornerLength;

                InitializeCornerOfPoint(localPositions, i, int2.Left, int2.Up, 90f,
                centerPartPool, edgePartPool, outerCornerPartPool, innerCornerPartPool); // Left Up
                InitializeCornerOfPoint(localPositions, i, int2.Right, int2.Up, 0f,
                centerPartPool, edgePartPool, outerCornerPartPool, innerCornerPartPool); // Right Up
                InitializeCornerOfPoint(localPositions, i, int2.Left, int2.Down, 180f,
                centerPartPool, edgePartPool, outerCornerPartPool, innerCornerPartPool); // Left Down
                InitializeCornerOfPoint(localPositions, i, int2.Right, int2.Down, 270f,
                centerPartPool, edgePartPool, outerCornerPartPool, innerCornerPartPool); // Right Down
            }
        }

        private void InitializeCornerOfPoint(IReadOnlyList<int2> localPositions, int positionIndex, int2 horizontalCheck, int2 verticalCheck, float rotationAngle,
            IViewPool<Transform> centerPartPool, IViewPool<Transform> edgePartPool,
            IViewPool<Transform> outerCornerPartPool, IViewPool<Transform> innerCornerPartPool)
        {
            var localPosition = localPositions[positionIndex];
            var horizontalNeighbour = false;
            var secondNeighbour = false;
            var crossNeighbour = false;
            var crossCheck = new int2(horizontalCheck.X, verticalCheck.Y);

            for(var i = 0; i < localPositions.Count; i++)
            {
                if(i == positionIndex) continue;
                if(horizontalNeighbour && secondNeighbour && crossNeighbour) break;

                var checkingPosition = localPositions[i];

                if(localPosition + horizontalCheck == checkingPosition)
                {
                    horizontalNeighbour = true;
                    continue;
                }

                if(localPosition + verticalCheck == checkingPosition)
                {
                    secondNeighbour = true;
                    continue;
                }

                if(localPosition + crossCheck == checkingPosition)
                {
                    crossNeighbour = true;
                    continue;
                }
            }
            
            if(horizontalNeighbour && secondNeighbour && crossNeighbour)
            {
                var createdMesh = centerPartPool.Spawn(transform);
                LevelViewHelper.ApplyColorProperty(createdMesh.gameObject, _color);
                createdMesh.transform.localPosition = new Vector3(localPosition.X, -localPosition.Y, 0f) * _cornerLength;
                createdMesh.transform.localRotation = Quaternion.Euler(0f, 0f, rotationAngle);
            }
            else if(crossNeighbour)
            {
                return;
            }
            else if(horizontalNeighbour && secondNeighbour)
            {
                var createdMesh = innerCornerPartPool.Spawn(transform);
                LevelViewHelper.ApplyColorProperty(createdMesh.gameObject, _color);
                createdMesh.transform.localPosition = new Vector3(localPosition.X, -localPosition.Y, 0f) * _cornerLength + new Vector3(horizontalCheck.X, -verticalCheck.Y, 0f);
                createdMesh.transform.localRotation = Quaternion.Euler(0f, 0f, rotationAngle);
            }
            else if(horizontalNeighbour)
            {
                var additionalAngle = 0f;
                var additionalPosition = int2.Zero;
                if(crossCheck.X == crossCheck.Y)
                {
                    additionalAngle = -90f;
                    additionalPosition = new int2(crossCheck.X, 0);
                }

                var createdMesh = edgePartPool.Spawn(transform);
                LevelViewHelper.ApplyColorProperty(createdMesh.gameObject, _color);
                createdMesh.transform.localPosition = new Vector3(localPosition.X, -localPosition.Y, 0f) * _cornerLength + new Vector3(additionalPosition.X, additionalPosition.Y, 0f);
                createdMesh.transform.localRotation = Quaternion.Euler(0f, 0f, rotationAngle + additionalAngle);
            }
            else if(secondNeighbour)
            {
                var additionalAngle = 0f;
                var additionalPosition = int2.Zero;
                if(crossCheck.X != crossCheck.Y)
                {
                    additionalAngle = -90f;
                    additionalPosition = new int2(0, -crossCheck.Y);
                }

                var createdMesh = edgePartPool.Spawn(transform);
                LevelViewHelper.ApplyColorProperty(createdMesh.gameObject, _color);
                createdMesh.transform.localPosition = new Vector3(localPosition.X, -localPosition.Y, 0f) * _cornerLength + new Vector3(additionalPosition.X, additionalPosition.Y, 0f);
                createdMesh.transform.localRotation = Quaternion.Euler(0f, 0f, rotationAngle + additionalAngle);
            }
            else
            {
                var createdMesh = outerCornerPartPool.Spawn(transform);
                LevelViewHelper.ApplyColorProperty(createdMesh.gameObject, _color);
                createdMesh.transform.localPosition = new Vector3(localPosition.X, -localPosition.Y, 0f) * _cornerLength;
                createdMesh.transform.localRotation = Quaternion.Euler(0f, 0f, rotationAngle);
            }
        }

        public void ResetView()
        {
            _moveTween?.Kill();
            _moveTween = null;
            _dragOrigin = null;
        }

        public void MoveToPoint(int2 newPoint)
        {
            _targetPosition = new Vector3(newPoint.X, -newPoint.Y, 0f) * _cornerLength;

            // While dragging, DragByWorldDelta positions the block every frame.
            if(_dragOrigin.HasValue) return;

            MoveToTarget();
        }

        public void BeginDrag()
        {
            _moveTween?.Kill();
            _dragOrigin = transform.localPosition;
            _targetPosition = _dragOrigin.Value;
        }

        public void DragByWorldDelta(Vector3 worldDelta, bool canMoveLeft, bool canMoveRight, bool canMoveUp, bool canMoveDown)
        {
            if(!_dragOrigin.HasValue) return;

            var desired = _dragOrigin.Value + transform.parent.InverseTransformVector(worldDelta);
            var offset = desired - _targetPosition;
            offset.x = Mathf.Clamp(offset.x, canMoveLeft ? -_cornerLength : -BlockedDragLimit, canMoveRight ? _cornerLength : BlockedDragLimit);
            offset.y = Mathf.Clamp(offset.y, canMoveDown ? -_cornerLength : -BlockedDragLimit, canMoveUp ? _cornerLength : BlockedDragLimit);

            transform.localPosition = _targetPosition + new Vector3(offset.x, offset.y, 0f);
        }

        public void EndDrag()
        {
            if(!_dragOrigin.HasValue) return;

            _dragOrigin = null;
            MoveToTarget();
        }

        private void MoveToTarget()
        {
            _moveTween?.Kill();
            _moveTween = transform.DOLocalMove(_targetPosition, MoveDuration);
        }
    }
}