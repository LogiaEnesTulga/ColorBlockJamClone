using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using RollicGames.Math.Runtime.Model;
using RollicGames.Pooling.Runtime.View;
using UnityEngine;
using DG.Tweening;
using System.Threading;
using RollicGames.ColorBlockJamClone.Level.Runtime.Model;

namespace RollicGames.ColorBlockJamClone.Level.Runtime.View
{
    public class LevelBlockObjectView : MonoBehaviour
    {
        private const float MoveDuration = 0.05f;
        private const float AbsorbingDuration = 1f;
        private const float BlockedDragLimit = LevelViewHelper.CellLength * 0.05f;

        [SerializeField] private Material _defaultMaterial;
        [SerializeField] private Material _clippingMaterial;

        private Color _color;
        private Tween _moveTween;

        private Vector3 _targetPosition;
        private Vector3? _dragOrigin;

        private readonly List<Renderer> _blockRenderers = new();
        private readonly List<Transform> _colliderObjects = new();

        public void InitializeView(Color color, IReadOnlyList<int2> localPositions,
            IViewPool<Renderer> centerPartPool, IViewPool<Renderer> edgePartPool,
            IViewPool<Renderer> outerCornerPartPool, IViewPool<Renderer> innerCornerPartPool,
            IViewPool<Transform> colliderPool)
        {
            _color = color;

            for(var i = 0; i < localPositions.Count; i++)
            {
                var localPosition = localPositions[i];
                var createdMesh = colliderPool.Spawn(transform);
                createdMesh.transform.localPosition = new Vector3(localPosition.X, -localPosition.Y, 0f) * LevelViewHelper.CellLength;
                _colliderObjects.Add(createdMesh);

                InitializeCornerOfPoint(localPositions, i, int2.Left, int2.Up, 90f,
                centerPartPool, edgePartPool, outerCornerPartPool, innerCornerPartPool); // Left Up
                InitializeCornerOfPoint(localPositions, i, int2.Right, int2.Up, 0f,
                centerPartPool, edgePartPool, outerCornerPartPool, innerCornerPartPool); // Right Up
                InitializeCornerOfPoint(localPositions, i, int2.Left, int2.Down, 180f,
                centerPartPool, edgePartPool, outerCornerPartPool, innerCornerPartPool); // Left Down
                InitializeCornerOfPoint(localPositions, i, int2.Right, int2.Down, 270f,
                centerPartPool, edgePartPool, outerCornerPartPool, innerCornerPartPool); // Right Down
            }

            gameObject.SetActive(true);
        }

        private void InitializeCornerOfPoint(IReadOnlyList<int2> localPositions, int positionIndex, int2 horizontalCheck, int2 verticalCheck, float rotationAngle,
            IViewPool<Renderer> centerPartPool, IViewPool<Renderer> edgePartPool,
            IViewPool<Renderer> outerCornerPartPool, IViewPool<Renderer> innerCornerPartPool)
        {
            var localPosition = localPositions[positionIndex];
            var horizontalNeighbour = false;
            var secondNeighbour = false;
            var crossNeighbour = false;
            var crossCheck = new int2(horizontalCheck.X, verticalCheck.Y);
            var cellLength = LevelViewHelper.CellLength;

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
                _blockRenderers.Add(createdMesh);
                LevelViewHelper.ApplyColorProperty(createdMesh, _color);
                createdMesh.transform.localPosition = new Vector3(localPosition.X, -localPosition.Y, 0f) * cellLength;
                createdMesh.transform.localRotation = Quaternion.Euler(0f, 0f, rotationAngle);
            }
            else if(crossNeighbour)
            {
                return;
            }
            else if(horizontalNeighbour && secondNeighbour)
            {
                var createdMesh = innerCornerPartPool.Spawn(transform);
                _blockRenderers.Add(createdMesh);
                LevelViewHelper.ApplyColorProperty(createdMesh, _color);
                var additionalPosition = new Vector3(horizontalCheck.X, -verticalCheck.Y, 0f) * 0.5f;
                createdMesh.transform.localPosition = (new Vector3(localPosition.X, -localPosition.Y, 0f) + additionalPosition) * cellLength;
                createdMesh.transform.localRotation = Quaternion.Euler(0f, 0f, rotationAngle);
            }
            else if(horizontalNeighbour)
            {
                var topLeftOrBottomRight = crossCheck.X == crossCheck.Y;
                var additionalAngle = topLeftOrBottomRight ? -90f : 0f;
                var additionalPosition = topLeftOrBottomRight ? new Vector3(crossCheck.X * 0.5f, 0f, 0f) : Vector3.zero;

                var createdMesh = edgePartPool.Spawn(transform);
                _blockRenderers.Add(createdMesh);
                LevelViewHelper.ApplyColorProperty(createdMesh, _color);
                createdMesh.transform.localPosition = (new Vector3(localPosition.X, -localPosition.Y, 0f) + additionalPosition) * cellLength;
                createdMesh.transform.localRotation = Quaternion.Euler(0f, 0f, rotationAngle + additionalAngle);
            }
            else if(secondNeighbour)
            {
                var topRightOrBottomLeft = crossCheck.X != crossCheck.Y;
                var additionalAngle = topRightOrBottomLeft ? -90f : 0f;
                var additionalPosition = topRightOrBottomLeft ? new Vector3(0f, -crossCheck.Y * 0.5f, 0f) : Vector3.zero;

                var createdMesh = edgePartPool.Spawn(transform);
                _blockRenderers.Add(createdMesh);
                LevelViewHelper.ApplyColorProperty(createdMesh, _color);
                createdMesh.transform.localPosition = (new Vector3(localPosition.X, -localPosition.Y, 0f) + additionalPosition) * cellLength;
                createdMesh.transform.localRotation = Quaternion.Euler(0f, 0f, rotationAngle + additionalAngle);
            }
            else
            {
                var createdMesh = outerCornerPartPool.Spawn(transform);
                _blockRenderers.Add(createdMesh);
                LevelViewHelper.ApplyColorProperty(createdMesh, _color);
                createdMesh.transform.localPosition = new Vector3(localPosition.X, -localPosition.Y, 0f) * cellLength;
                createdMesh.transform.localRotation = Quaternion.Euler(0f, 0f, rotationAngle);
            }
        }

        public void ResetView()
        {
            _moveTween?.Kill();
            _moveTween = null;
            _dragOrigin = null;

            foreach(var renderer in _blockRenderers)
            {
                LevelViewHelper.ChangeMaterial(renderer, _defaultMaterial);
            }

            _blockRenderers.Clear();
            _colliderObjects.Clear();
        }

        public void MoveToPoint(int2 newPoint)
        {
            _targetPosition = new Vector3(newPoint.X, -newPoint.Y, 0f) * LevelViewHelper.CellLength;

            // While dragging, DragByWorldDelta positions the block every frame.
            if(_dragOrigin.HasValue) return;

            MoveToTarget();
        }

        public async UniTask StartAbsorbingAnimation(int2 lastPoint, LevelDirection absortDirection, float clippingLimit, CancellationTokenSource cancellationToken)
        {
            foreach(var colliderObject in _colliderObjects)
            {
                colliderObject.gameObject.SetActive(false);
            }

            await UniTask.WaitForSeconds(MoveDuration, cancellationToken: cancellationToken.Token); // wait for canceling dragging
            _moveTween?.Kill();

            if(cancellationToken.IsCancellationRequested) return;

            var directionVector = absortDirection.GetDirectionVector();
            foreach(var renderer in _blockRenderers)
            {
                LevelViewHelper.ChangeMaterial(renderer, _clippingMaterial);
                LevelViewHelper.ApplyClipProperties(renderer, new Vector2(directionVector.X, -directionVector.Y), clippingLimit);
            }

            _moveTween = transform.DOLocalMove(new Vector3(lastPoint.X - directionVector.X * 0.5f, -lastPoint.Y + directionVector.Y * 0.5f, 0f) * LevelViewHelper.CellLength, AbsorbingDuration);
            await _moveTween.AsyncWaitForCompletion();

            if(cancellationToken.IsCancellationRequested) return;
            
            gameObject.SetActive(false);
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
            var cellLength = LevelViewHelper.CellLength;
            offset.x = Mathf.Clamp(offset.x, canMoveLeft ? -cellLength : -BlockedDragLimit, canMoveRight ? cellLength : BlockedDragLimit);
            offset.y = Mathf.Clamp(offset.y, canMoveDown ? -cellLength : -BlockedDragLimit, canMoveUp ? cellLength : BlockedDragLimit);

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