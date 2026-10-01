using System.Collections.Generic;
using System.Linq;
using RollicGames.Math.Runtime.Model;
using RollicGames.ColorBlockJamClone.Level.Runtime.Model;

namespace RollicGames.ColorBlockJamClone.Level.Editor
{
    public class LevelDoorEditor : ILevelObjectEditor
    {
        private static readonly LevelDirection[] AllDirections = { LevelDirection.Up, LevelDirection.Down, LevelDirection.Left, LevelDirection.Right };
        private static readonly LevelDirection[] HorizontalDirections = { LevelDirection.Up, LevelDirection.Down };
        private static readonly LevelDirection[] VerticalDirections = { LevelDirection.Left, LevelDirection.Right };

        private readonly LevelGridModel _grid;

        public LevelObjectType ObjectType => LevelObjectType.Door;

        public LevelDoorEditor(LevelGridModel grid)
        {
            _grid = grid;
        }

        public bool CanHostSegment(int2 position, LevelDoorObjectModel ignoredDoor)
        {
            if(!_grid.IsInsideDoorArea(position) || _grid.Cells.Contains(position)) return false;
            if(_grid.Doors.ObjectsByPosition.TryGetValue(position, out var door) && door != ignoredDoor) return false;

            return _grid.IsNextToCell(position);
        }

        public bool CanDrawSegment(int2 position, IReadOnlyList<int2> drawnPositions)
        {
            if(drawnPositions.Contains(position) || !CanHostSegment(position, null)) return false;
            if(drawnPositions.Count == 0) return true;

            if(drawnPositions.Count > 1)
            {
                var isHorizontal = drawnPositions[0].Y == drawnPositions[1].Y;
                var isOnLine = isHorizontal ? position.Y == drawnPositions[0].Y : position.X == drawnPositions[0].X;
                if(!isOnLine) return false;
            }

            return LevelGridQueries.IsNeighbourOfAny(position, drawnPositions);
        }

        public LevelDoorObjectModel Create(IReadOnlyList<int2> positions, LevelObjectColor color)
        {
            var isHorizontal = positions.Count > 1 && positions[0].Y == positions[1].Y;
            var direction = ResolveDefaultDirection(positions, GetAvailableDirections(positions.Count, isHorizontal));

            var door = new LevelDoorObjectModel();
            door.SetId(LevelGridQueries.GetNextId(_grid.Doors));
            Initialize(door, positions, color, direction);
            LevelGridRegistry.AddDoor(_grid, door);
            return door;
        }

        public IReadOnlyList<LevelDirection> GetAvailableDirections(LevelDoorObjectModel door)
        {
            return GetAvailableDirections(door.Length, LevelGridQueries.IsHorizontal(door.AbsorbDirection));
        }

        public void SetDirection(LevelDoorObjectModel door, LevelDirection direction)
        {
            Rebuild(door, LevelGridQueries.GetDoorPositions(door), door.Color, direction);
        }

        public IReadOnlyList<int2> GetPositions(LevelObjectModel levelObject)
        {
            return LevelGridQueries.GetDoorPositions((LevelDoorObjectModel)levelObject);
        }

        public LevelObjectColor GetColor(LevelObjectModel levelObject)
        {
            return ((LevelDoorObjectModel)levelObject).Color;
        }

        public void SetColor(LevelObjectModel levelObject, LevelObjectColor color)
        {
            var door = (LevelDoorObjectModel)levelObject;
            Rebuild(door, LevelGridQueries.GetDoorPositions(door), color, door.AbsorbDirection);
        }

        public void Remove(LevelObjectModel levelObject)
        {
            LevelGridRegistry.RemoveDoor(_grid, (LevelDoorObjectModel)levelObject);
        }

        public bool CanRemovePiece(LevelObjectModel levelObject, int2 position)
        {
            return LevelGridQueries.CanRemoveKeepingConnected(position, LevelGridQueries.GetDoorPositions((LevelDoorObjectModel)levelObject));
        }

        public void RemovePiece(LevelObjectModel levelObject, int2 position)
        {
            var door = (LevelDoorObjectModel)levelObject;
            var positions = LevelGridQueries.GetDoorPositions(door);
            positions.Remove(position);

            if(positions.Count == 0)
            {
                Remove(door);
                return;
            }

            Rebuild(door, positions, door.Color, door.AbsorbDirection);
        }

        public bool CanMove(LevelObjectModel levelObject, int2 targetOrigin)
        {
            var door = (LevelDoorObjectModel)levelObject;
            return GetMovedPositions(door, targetOrigin).All(position => CanHostSegment(position, door));
        }

        public void Move(LevelObjectModel levelObject, int2 targetOrigin)
        {
            var door = (LevelDoorObjectModel)levelObject;
            Rebuild(door, GetMovedPositions(door, targetOrigin), door.Color, door.AbsorbDirection);
        }

        public void RemoveInvalidDoors()
        {
            foreach(var door in _grid.Doors.ObjectsById.Values.ToList())
            {
                var isValid = LevelGridQueries.GetDoorPositions(door).All(position => CanHostSegment(position, door));
                if(!isValid)
                {
                    Remove(door);
                }
            }
        }

        private static IReadOnlyList<LevelDirection> GetAvailableDirections(int length, bool isHorizontal)
        {
            if(length <= 1) return AllDirections;

            return isHorizontal ? HorizontalDirections : VerticalDirections;
        }

        private LevelDirection ResolveDefaultDirection(IReadOnlyList<int2> positions, IReadOnlyList<LevelDirection> candidates)
        {
            var bestDirection = candidates[0];
            var bestScore = -1;
            foreach(var candidate in candidates)
            {
                // A door absorbs blocks coming from the opposite side of its absorb direction.
                var blockSide = candidate.GetDirectionVector();
                var score = positions.Count(position => _grid.Cells.Contains(position - blockSide));
                if(score <= bestScore) continue;

                bestScore = score;
                bestDirection = candidate;
            }

            return bestDirection;
        }

        private static List<int2> GetMovedPositions(LevelDoorObjectModel door, int2 targetOrigin)
        {
            return LevelGridQueries.GetDoorPositions(door).Select(position => position - door.GridPosition + targetOrigin).ToList();
        }

        private void Rebuild(LevelDoorObjectModel door, IReadOnlyCollection<int2> positions, LevelObjectColor color, LevelDirection direction)
        {
            LevelGridRegistry.RemoveDoor(_grid, door);
            door.OnReturnedToPool();
            Initialize(door, positions, color, direction);
            LevelGridRegistry.AddDoor(_grid, door);
        }

        private static void Initialize(LevelDoorObjectModel door, IReadOnlyCollection<int2> positions, LevelObjectColor color, LevelDirection direction)
        {
            door.Initialize(LevelGridQueries.GetMinCorner(positions), positions.Count, color, direction);
        }
    }
}
