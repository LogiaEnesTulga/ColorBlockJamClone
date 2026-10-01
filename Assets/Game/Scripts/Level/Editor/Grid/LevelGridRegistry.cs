using RollicGames.ColorBlockJamClone.Level.Runtime.Model;

namespace RollicGames.ColorBlockJamClone.Level.Editor
{
    public static class LevelGridRegistry
    {
        public static void AddBlock(LevelGridModel grid, LevelBlockObjectModel block)
        {
            grid.Blocks.AddObjectWithId(block);
            foreach(var position in LevelGridQueries.GetBlockPositions(block))
            {
                grid.Blocks.ObjectsByPosition.Add(position, block);
            }
        }

        public static void RemoveBlock(LevelGridModel grid, LevelBlockObjectModel block)
        {
            foreach(var position in LevelGridQueries.GetBlockPositions(block))
            {
                grid.Blocks.ObjectsByPosition.Remove(position);
            }
            grid.Blocks.ObjectsById.Remove(block.Id);
        }

        public static void AddDoor(LevelGridModel grid, LevelDoorObjectModel door)
        {
            grid.Doors.AddObjectWithId(door);
            foreach(var position in LevelGridQueries.GetDoorPositions(door))
            {
                grid.Doors.ObjectsByPosition.Add(position, door);
            }
        }

        public static void RemoveDoor(LevelGridModel grid, LevelDoorObjectModel door)
        {
            foreach(var position in LevelGridQueries.GetDoorPositions(door))
            {
                grid.Doors.ObjectsByPosition.Remove(position);
            }
            grid.Doors.ObjectsById.Remove(door.Id);
        }

        public static void Clear(LevelGridModel grid)
        {
            grid.Width = 0;
            grid.Height = 0;
            grid.Cells.Clear();
            grid.Blocks.Clear();
            grid.Doors.Clear();
        }
    }
}
