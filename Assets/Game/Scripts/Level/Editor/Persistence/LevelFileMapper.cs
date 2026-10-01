using System.Linq;
using RollicGames.Math.Runtime.Model;
using RollicGames.ColorBlockJamClone.Level.Runtime.Model;

namespace RollicGames.ColorBlockJamClone.Level.Editor
{
    public static class LevelFileMapper
    {
        public static LevelFileData ToFileData(LevelEditorSession session)
        {
            var grid = session.Grid;
            var data = new LevelFileData
            {
                LevelId = session.LevelId,
                Duration = session.Duration,
            };

            data.Grid.Width = grid.Width;
            data.Grid.Height = grid.Height;
            data.Grid.Cells = grid.Cells
                .OrderBy(cell => cell.X)
                .ThenBy(cell => cell.Y)
                .Select(ToPositionData)
                .ToList();

            var blockId = 1;
            foreach(var block in grid.Blocks.ObjectsById.Values.OrderBy(block => block.Id))
            {
                data.Blocks.Add(new LevelBlockFileData
                {
                    Id = blockId++,
                    Color = block.Color,
                    Position = ToPositionData(block.GridPosition),
                    LocalPositions = block.BlocksLocalPositions.Select(ToPositionData).ToList(),
                });
            }

            var doorId = 1;
            foreach(var door in grid.Doors.ObjectsById.Values.OrderBy(door => door.Id))
            {
                data.Doors.Add(new LevelDoorFileData
                {
                    Id = doorId++,
                    Color = door.Color,
                    Position = ToPositionData(door.GridPosition),
                    Length = door.Length,
                    AbsorbDirection = door.AbsorbDirection,
                });
            }

            return data;
        }

        public static void FillGrid(LevelGridModel grid, LevelFileData data)
        {
            LevelGridRegistry.Clear(grid);
            grid.Width = data.Grid.Width;
            grid.Height = data.Grid.Height;

            foreach(var cell in data.Grid.Cells)
            {
                grid.Cells.Add(ToModel(cell));
            }

            foreach(var blockData in data.Blocks)
            {
                var block = new LevelBlockObjectModel();
                block.SetId(blockData.Id);
                block.Initialize(ToModel(blockData.Position), blockData.Color, blockData.LocalPositions.Select(ToModel).ToList());
                LevelGridRegistry.AddBlock(grid, block);
            }

            foreach(var doorData in data.Doors)
            {
                var door = new LevelDoorObjectModel();
                door.SetId(doorData.Id);
                door.Initialize(ToModel(doorData.Position), doorData.Length, doorData.Color, doorData.AbsorbDirection);
                LevelGridRegistry.AddDoor(grid, door);
            }
        }

        private static LevelPositionFileData ToPositionData(int2 position)
        {
            return new LevelPositionFileData { X = position.X, Y = position.Y };
        }

        private static int2 ToModel(LevelPositionFileData position)
        {
            return new int2(position.X, position.Y);
        }
    }
}
