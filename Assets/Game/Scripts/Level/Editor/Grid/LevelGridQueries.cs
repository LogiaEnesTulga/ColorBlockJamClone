using System.Collections.Generic;
using System.Linq;
using RollicGames.Math.Runtime.Model;
using RollicGames.ColorBlockJamClone.Level.Runtime.Model;

namespace RollicGames.ColorBlockJamClone.Level.Editor
{
    public static class LevelGridQueries
    {
        public static readonly int2[] NeighbourOffsets = { int2.Up, int2.Down, int2.Left, int2.Right };

        public static bool IsInsideGrid(this LevelGridModel grid, int2 position)
        {
            return position.X >= 0 && position.Y >= 0 && position.X < grid.Width && position.Y < grid.Height;
        }

        public static bool IsInsideDoorArea(this LevelGridModel grid, int2 position)
        {
            return position.X >= -1 && position.Y >= -1 && position.X <= grid.Width && position.Y <= grid.Height;
        }

        public static bool IsNextToCell(this LevelGridModel grid, int2 position)
        {
            foreach(var offset in NeighbourOffsets)
            {
                if(grid.Cells.Contains(position + offset)) return true;
            }

            return false;
        }

        public static LevelObjectModel GetObjectAt(this LevelGridModel grid, int2 position)
        {
            if(grid.Blocks.ObjectsByPosition.TryGetValue(position, out var block)) return block;
            if(grid.Doors.ObjectsByPosition.TryGetValue(position, out var door)) return door;

            return null;
        }

        public static bool ContainsObject(this LevelGridModel grid, LevelObjectModel levelObject)
        {
            return levelObject switch
            {
                LevelBlockObjectModel block => grid.Blocks.ObjectsById.TryGetValue(block.Id, out var found) && found == block,
                LevelDoorObjectModel door => grid.Doors.ObjectsById.TryGetValue(door.Id, out var found) && found == door,
                _ => false,
            };
        }

        public static bool IsNeighbour(int2 a, int2 b)
        {
            var delta = a - b;
            return System.Math.Abs(delta.X) + System.Math.Abs(delta.Y) == 1;
        }

        public static bool IsNeighbourOfAny(int2 position, IEnumerable<int2> positions)
        {
            return positions.Any(other => IsNeighbour(position, other));
        }

        /// <summary>True when every position can be reached from every other through 4-directional neighbours.</summary>
        public static bool IsConnected(ICollection<int2> positions)
        {
            if(positions.Count == 0) return true;

            var visited = new HashSet<int2>();
            var pending = new Stack<int2>();
            pending.Push(positions.First());
            while(pending.Count > 0)
            {
                var position = pending.Pop();
                if(!visited.Add(position)) continue;

                foreach(var offset in NeighbourOffsets)
                {
                    var neighbour = position + offset;
                    if(positions.Contains(neighbour) && !visited.Contains(neighbour))
                    {
                        pending.Push(neighbour);
                    }
                }
            }

            return visited.Count == positions.Count;
        }

        /// <summary>True when the position belongs to the shape and removing it leaves the rest connected.</summary>
        public static bool CanRemoveKeepingConnected(int2 position, IEnumerable<int2> positions)
        {
            var remaining = new HashSet<int2>(positions);
            return remaining.Remove(position) && IsConnected(remaining);
        }

        public static int2 GetMinCorner(IEnumerable<int2> positions)
        {
            var minX = int.MaxValue;
            var minY = int.MaxValue;
            foreach(var position in positions)
            {
                minX = System.Math.Min(minX, position.X);
                minY = System.Math.Min(minY, position.Y);
            }

            return new int2(minX, minY);
        }

        public static int GetNextId<T>(LevelObjectContainer<T> container) where T : LevelObjectModel
        {
            return container.ObjectsById.Count == 0 ? 1 : container.ObjectsById.Keys.Max() + 1;
        }

        public static bool IsHorizontal(LevelDirection absorbDirection)
        {
            return absorbDirection is LevelDirection.Up or LevelDirection.Down;
        }

        public static List<int2> GetBlockPositions(LevelBlockObjectModel block)
        {
            return block.BlocksLocalPositions.Select(localPosition => block.GridPosition + localPosition).ToList();
        }

        public static List<int2> GetDoorPositions(LevelDoorObjectModel door)
        {
            var axis = IsHorizontal(door.AbsorbDirection) ? int2.Right : int2.Down;
            var positions = new List<int2>(door.Length);
            for(var i = 0; i < door.Length; i++)
            {
                positions.Add(door.GridPosition + axis * i);
            }

            return positions;
        }
    }
}
