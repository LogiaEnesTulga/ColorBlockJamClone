using System.Collections.Generic;
using RollicGames.Math.Runtime.Model;
using UnityEngine;

namespace RollicGames.ColorBlockJamClone.Level.Runtime.View
{
    public class LevelBlockObjectView : MonoBehaviour
    {
        [SerializeField] private GameObject _centerPart;
        [SerializeField] private GameObject _edgePart;
        [SerializeField] private GameObject _outerCornerPart;
        [SerializeField] private GameObject _innertCornerPart;
        [SerializeField] private GameObject _colliderObject;

        [SerializeField] private float _cornerLength = 2f;

        private Color _color;

        public void InitializeView(IReadOnlyList<int2> localPositions, Color color)
        {
            _color = color;

            for(var i = 0; i < localPositions.Count; i++)
            {
                var localPosition = localPositions[i];
                var createdMesh = Instantiate(_colliderObject, transform);
                createdMesh.transform.localPosition = new Vector3(localPosition.X, -localPosition.Y, 0f) * _cornerLength;

                
                InitializeCornerOfPoint(localPositions, i, int2.Left, int2.Up, 90f); // Left Up
                InitializeCornerOfPoint(localPositions, i, int2.Right, int2.Up, 0f); // Right Up
                InitializeCornerOfPoint(localPositions, i, int2.Left, int2.Down, 180f); // Left Down
                InitializeCornerOfPoint(localPositions, i, int2.Right, int2.Down, 270f); // Right Down
            }
        }

        private void InitializeCornerOfPoint(IReadOnlyList<int2> localPositions, int positionIndex, int2 horizontalCheck, int2 verticalCheck, float rotationAngle)
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
                var createdMesh = Instantiate(_centerPart, transform);
                LevelViewHelper.ApplyColorProperty(createdMesh, _color);
                createdMesh.transform.localPosition = new Vector3(localPosition.X, -localPosition.Y, 0f) * _cornerLength;
                createdMesh.transform.localRotation = Quaternion.Euler(0f, 0f, rotationAngle);
            }
            else if(crossNeighbour)
            {
                return;
            }
            else if(horizontalNeighbour && secondNeighbour)
            {
                var createdMesh = Instantiate(_innertCornerPart, transform);
                LevelViewHelper.ApplyColorProperty(createdMesh, _color);
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

                var createdMesh = Instantiate(_edgePart, transform);
                LevelViewHelper.ApplyColorProperty(createdMesh, _color);
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
                LevelViewHelper.ApplyColorProperty(createdMesh, _color);
                createdMesh.transform.localPosition = new Vector3(localPosition.X, -localPosition.Y, 0f) * _cornerLength + new Vector3(additionalPosition.X, additionalPosition.Y, 0f);
                createdMesh.transform.localRotation = Quaternion.Euler(0f, 0f, rotationAngle + additionalAngle);
            }
            else
            {
                var createdMesh = Instantiate(_outerCornerPart, transform);
                LevelViewHelper.ApplyColorProperty(createdMesh, _color);
                createdMesh.transform.localPosition = new Vector3(localPosition.X, -localPosition.Y, 0f) * _cornerLength;
                createdMesh.transform.localRotation = Quaternion.Euler(0f, 0f, rotationAngle);
            }
        }
    }
}