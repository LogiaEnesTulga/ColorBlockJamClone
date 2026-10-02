using System.Collections.Generic;
using RollicGames.Math.Runtime.Model;
using RollicGames.Pooling.Runtime.View;
using RollicGames.ColorBlockJamClone.Level.Runtime.Model;
using UnityEngine;
using System.Linq;

namespace RollicGames.ColorBlockJamClone.Level.Runtime.View
{
    public class LevelWallView : MonoBehaviour
    {
        [SerializeField] private float _cornerLength = 2f;

        public void GenerateWalls(HashSet<int2> cellPositions, IReadOnlyDictionary<int2, LevelDoorObjectModel> doors,
            IViewPool<Renderer> edgePartPool, IViewPool<Renderer> outerCornerPartPool,
            IViewPool<Renderer> innerCornerPartPool, Color color)
        {
            foreach(var cellPosition in cellPositions)
            {
                InitializeCornerOfPoint(cellPositions, doors, cellPosition, int2.Left, int2.Up, 180f, color,
                edgePartPool, outerCornerPartPool, innerCornerPartPool); // Left Up
                InitializeCornerOfPoint(cellPositions, doors, cellPosition, int2.Right, int2.Up, 90f, color,
                edgePartPool, outerCornerPartPool, innerCornerPartPool); // Right Up
                InitializeCornerOfPoint(cellPositions, doors, cellPosition, int2.Left, int2.Down, 270f, color,
                edgePartPool, outerCornerPartPool, innerCornerPartPool); // Left Down
                InitializeCornerOfPoint(cellPositions, doors, cellPosition, int2.Right, int2.Down, 0f, color,
                edgePartPool, outerCornerPartPool, innerCornerPartPool); // Right Down
            }
        }

        private void InitializeCornerOfPoint(IReadOnlyCollection<int2> cellPositions, IReadOnlyDictionary<int2, LevelDoorObjectModel> doors,
         int2 cellPosition, int2 horizontalCheck, int2 verticalCheck, float rotationAngle, Color color,
            IViewPool<Renderer> edgePartPool,IViewPool<Renderer> outerCornerPartPool,
            IViewPool<Renderer> innerCornerPartPool)
        {
            var crossCheck = new int2(horizontalCheck.X, verticalCheck.Y);

            var horizontalNeighbour = cellPositions.Contains(cellPosition + horizontalCheck);
            var verticalNeighbour = cellPositions.Contains(cellPosition + verticalCheck);
            var crossNeighbour = cellPositions.Contains(cellPosition + crossCheck);

            var horizontalDoor = doors.ContainsKey(cellPosition + horizontalCheck);
            var verticalDoor = doors.ContainsKey(cellPosition + verticalCheck);
            var crossDoor = doors.ContainsKey(cellPosition + crossCheck);

            if(!horizontalNeighbour && !verticalNeighbour && !crossNeighbour && !crossDoor)
            {
                if (verticalDoor != horizontalDoor)
                {
                    var otherPart = edgePartPool.Spawn(transform);
                    var noNeedToChangeDegree = (!horizontalDoor && horizontalCheck.X != verticalCheck.Y)
                                    || (!verticalDoor && horizontalCheck.X == verticalCheck.Y);
                    LevelViewHelper.ApplyColorProperty(otherPart, color);
                    otherPart.transform.localPosition = new Vector3(cellPosition.X, -cellPosition.Y, 0f) * _cornerLength + new Vector3((horizontalDoor ? 0.25f : 0.75f) * horizontalCheck.X, (verticalDoor ? 0.25f : 0.75f) * -verticalCheck.Y, 0f) * _cornerLength;
                    otherPart.transform.localEulerAngles = new Vector3((rotationAngle + (noNeedToChangeDegree ? 0f : 90f)) % 360f, -90f, -90f);
                }

                var createdMesh = (verticalDoor || horizontalDoor ? innerCornerPartPool : outerCornerPartPool).Spawn(transform);
                LevelViewHelper.ApplyColorProperty(createdMesh, color);
                createdMesh.transform.localPosition = new Vector3(cellPosition.X, -cellPosition.Y, 0f) * _cornerLength + new Vector3(horizontalCheck.X, -verticalCheck.Y, 0f) * _cornerLength * 0.75f;
                createdMesh.transform.localEulerAngles = new Vector3(rotationAngle, -90f, -90f);
                return;
            }

            if(horizontalNeighbour && verticalNeighbour && !crossNeighbour && !crossDoor)
            {
                var createdMesh = innerCornerPartPool.Spawn(transform);
                LevelViewHelper.ApplyColorProperty(createdMesh, color);
                createdMesh.transform.localPosition = new Vector3(cellPosition.X, -cellPosition.Y, 0f) * _cornerLength + new Vector3(horizontalCheck.X, -verticalCheck.Y, 0f) * _cornerLength * 0.75f;
                createdMesh.transform.localEulerAngles = new Vector3((rotationAngle + 180f) % 360f, -90f, -90f);
                return;
            }

            if(!crossNeighbour && 
                (
                (horizontalNeighbour && !verticalNeighbour && (!verticalDoor || doors[cellPosition + verticalCheck].AbsorbDirection.GetDirectionVector().X == horizontalCheck.X)) ||
                (!horizontalNeighbour && verticalNeighbour && (!horizontalDoor || doors[cellPosition + horizontalCheck].AbsorbDirection.GetDirectionVector().Y == verticalCheck.Y))
                ))
            {
                var createdMesh = edgePartPool.Spawn(transform);
                var noNeedToChangeDegree = (!horizontalNeighbour && horizontalCheck.X != verticalCheck.Y)
                                || (!verticalNeighbour && horizontalCheck.X == verticalCheck.Y);
                LevelViewHelper.ApplyColorProperty(createdMesh, color);
                createdMesh.transform.localPosition = new Vector3(cellPosition.X, -cellPosition.Y, 0f) * _cornerLength + new Vector3((horizontalNeighbour ? 0.25f : 0.75f) * horizontalCheck.X, (verticalNeighbour ? 0.25f : 0.75f) * -verticalCheck.Y, 0f) * _cornerLength;
                createdMesh.transform.localEulerAngles = new Vector3((rotationAngle + (noNeedToChangeDegree ? 0f : 90f)) % 360f, -90f, -90f);
                return;
            }
        }
    }
}