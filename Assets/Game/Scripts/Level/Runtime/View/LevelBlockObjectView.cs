using System.Collections.Generic;
using RollicGames.ColorBlockJamClone.Level.Runtime.Model;
using RollicGames.Math.Runtime.Model;
using UnityEngine;

namespace RollicGames.ColorBlockJamClone.Level.Runtime.View
{
    public class LevelBlockObjectView : MonoBehaviour
    {
        private static readonly int2 Left = new (-1, 0);
        private static readonly int2 Right = new (1, 0);
        private static readonly int2 Up = new (0, -1);
        private static readonly int2 Down = new (0, 1);
        private static readonly int2 LeftUp = new (-1, -1);
        private static readonly int2 LeftDown = new (-1, 1);
        private static readonly int2 RightUp = new (1, -1);
        private static readonly int2 RightDown = new (1, 1);

        [SerializeField] private GameObject _centerPart;
        [SerializeField] private GameObject _edgePart;
        [SerializeField] private GameObject _outerCornerPart;
        [SerializeField] private GameObject _innertCornerPart;
        [SerializeField] private GameObject _colliderObject;

        [SerializeField] private float _cornerLength = 2f;

        [SerializeField] private LevelColorsConfig _colorsConfig;

        [SerializeField] private List<Vector2Int> _testLocalPositions;
        [SerializeField] private LevelObjectColor _testColor;

        private static readonly int ColorPropertyId = Shader.PropertyToID("_Color");

        private MaterialPropertyBlock _propertyBlock;
        private Color _color;

        [ContextMenu("TestShape")]
        public void TestShape()
        {
            var children = new List<Transform>();
            for(var i = 0; i < transform.childCount; i++)
            {
                children.Add(transform.GetChild(i));
            }

            foreach(var child in children)
            {
                DestroyImmediate(child.gameObject);
            }

            var positions = new List<int2>();
            foreach(var position in _testLocalPositions)
            {
                positions.Add(new int2(position.x, position.y));
            }
            InitializeView(positions, _testColor);
        }


        public void InitializeView(IReadOnlyList<int2> localPositions, LevelObjectColor levelColor)
        {
            _color = _colorsConfig.GetColor(levelColor);

            for(var i = 0; i < localPositions.Count; i++)
            {
                var localPosition = localPositions[i];
                var createdMesh = Instantiate(_colliderObject, transform);
                createdMesh.transform.localPosition = new Vector3(localPosition.X, -localPosition.Y, 0f) * _cornerLength;

                
                InitializeCornerOfPoint(localPositions, i, Left, Up, LeftUp, LeftDown, 90f); // Left Up
                InitializeCornerOfPoint(localPositions, i, Right, Up, RightUp, RightDown, 0f); // Right Up
                InitializeCornerOfPoint(localPositions, i, Left, Down, LeftDown, LeftUp, 180f); // Left Down
                InitializeCornerOfPoint(localPositions, i, Right, Down, RightDown, RightUp, 270f); // Right Down
            }
        }

        private void ApplyColor(GameObject part)
        {
            var renderer = part.GetComponent<Renderer>();
            if(renderer == null) return;

            _propertyBlock ??= new();
            
            renderer.GetPropertyBlock(_propertyBlock);
            _propertyBlock.SetColor(ColorPropertyId, _color);
            renderer.SetPropertyBlock(_propertyBlock);
        }

        private void InitializeCornerOfPoint(IReadOnlyList<int2> localPositions, int positionIndex, int2 firstCheck, int2 secondCheck, int2 crossCheck, int2 meshScaleMultiplier, float rotationAngle)
        {
            var localPosition = localPositions[positionIndex];
            var firstNeighbour = false;
            var secondNeighbour = false;
            var crossNeighbour = false;

            for(var i = 0; i < localPositions.Count; i++)
            {
                if(i == positionIndex) continue;
                if(firstNeighbour && secondNeighbour && crossNeighbour) break;

                var checkingPosition = localPositions[i];

                if(localPosition + firstCheck == checkingPosition)
                {
                    firstNeighbour = true;
                    continue;
                }

                if(localPosition + secondCheck == checkingPosition)
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
            
            if(firstNeighbour && secondNeighbour && crossNeighbour)
            {
                var createdMesh = Instantiate(_centerPart, transform);
                ApplyColor(createdMesh);
                createdMesh.transform.localPosition = new Vector3(localPosition.X, -localPosition.Y, 0f) * _cornerLength;
                createdMesh.transform.localRotation = Quaternion.Euler(0f, 0f, rotationAngle);
            }
            else if(crossNeighbour)
            {
                return;
            }
            else if(firstNeighbour && secondNeighbour)
            {
                var createdMesh = Instantiate(_innertCornerPart, transform);
                ApplyColor(createdMesh);
                createdMesh.transform.localPosition = new Vector3(localPosition.X, -localPosition.Y, 0f) * _cornerLength + new Vector3(firstCheck.X, -secondCheck.Y, 0f);
                createdMesh.transform.localRotation = Quaternion.Euler(0f, 0f, rotationAngle);
            }
            else if(firstNeighbour)
            {
                var additionalAngle = 0f;
                var additionalPosition = int2.Zero;
                if(crossCheck.X == crossCheck.Y)
                {
                    additionalAngle = -90f;
                    additionalPosition = new int2(crossCheck.X, 0);
                }

                var createdMesh = Instantiate(_edgePart, transform);
                ApplyColor(createdMesh);
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

                var createdMesh = Instantiate(_edgePart, transform);
                ApplyColor(createdMesh);
                createdMesh.transform.localPosition = new Vector3(localPosition.X, -localPosition.Y, 0f) * _cornerLength + new Vector3(additionalPosition.X, additionalPosition.Y, 0f);
                createdMesh.transform.localRotation = Quaternion.Euler(0f, 0f, rotationAngle + additionalAngle);
            }
            else
            {
                var createdMesh = Instantiate(_outerCornerPart, transform);
                ApplyColor(createdMesh);
                createdMesh.transform.localPosition = new Vector3(localPosition.X, -localPosition.Y, 0f) * _cornerLength;
                createdMesh.transform.localRotation = Quaternion.Euler(0f, 0f, rotationAngle);
            }
        }
    }
}