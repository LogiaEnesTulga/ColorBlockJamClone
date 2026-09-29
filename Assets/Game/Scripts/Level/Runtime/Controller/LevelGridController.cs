using System.Collections.Generic;
using Zenject;
using RollicGames.Math.Runtime.Model;
using RollicGames.ColorBlockJamClone.Level.Runtime.Model;

namespace RollicGames.ColorBlockJamClone.Level.Runtime.Controller
{
    public class LevelGridController
    {
        [Inject] private readonly LevelGridModel _gridModel;

        private void Initialize()
        {
            // TODO : Change here, its mocked Level 1 initializing.
            _gridModel.Width = 6;
            _gridModel.Height = 7;

            _gridModel.Grid.Clear();
            for(var rowIndex = 0; rowIndex < _gridModel.Height; rowIndex++)
            {
                _gridModel.Grid.Add(new List<LevelObjectModel>());
            }

            var blueDoor = new LevelDoorObjectModel(new int2(2, 0), 3, LevelObjectColor.Blue, LevelDirection.Up);
            var purpleDoor = new LevelDoorObjectModel(new int2(1, 6), 3, LevelObjectColor.Purple, LevelDirection.Down);
            var blueBlock = new LevelBlockObjectModel(new int2(3, 2), LevelObjectColor.Blue, new()
            {
                new (0, 0), new (1, 0),
                new (0, 1), new (1, 1)
            });
            var purpleBlock = new LevelBlockObjectModel(new int2(1, 3), LevelObjectColor.Purple, new()
            {
                new (0, 0), new (1, 0),
                new (0, 1), new (1, 1)
            });

            var wallList = new List<int2>()
            {
                new (0, 0),
                new (1, 0),
                new (5, 0),
                
                new (0, 1),
                new (5, 1),
                new (0, 2),
                new (5, 2),
                new (0, 3),
                new (5, 3),
                new (0, 4),
                new (5, 4),
                new (0, 5),
                new (5, 5),

                new (0, 6),
                new (4, 6),
                new (5, 6),
            };

            foreach(var wallPosition in wallList)
            {
                _gridModel.Grid[wallPosition.Y][wallPosition.X] = new LevelWallObjectModel(wallPosition);
            }

            AddBlockToGrid(blueBlock);
            AddBlockToGrid(purpleBlock);
            AddDoorToGrid(blueDoor);
            AddDoorToGrid(purpleDoor);
        }

        private void AddDoorToGrid(LevelDoorObjectModel doorObject)
        {
            var isVertical = doorObject.AbsorbDirection is LevelDirection.Up or LevelDirection.Down;
            var direction = new int2(isVertical ? 1 : 0, isVertical ? 0 : 1);
            for(var i = 0; i < doorObject.Length; i++)
            {
                var activePosition = doorObject.GridPosition + (direction * i);
                _gridModel.Grid[activePosition.Y][activePosition.X] = doorObject;
            }
        }

        private void AddBlockToGrid(LevelBlockObjectModel blockObject)
        {
            var localPositions = blockObject.BlocksLocalPositions;
            for(var i = 0; i < localPositions.Count; i++)
            {
                var activePosition = blockObject.GridPosition + localPositions[i];
                _gridModel.Grid[activePosition.Y][activePosition.X] = blockObject;
            }
        }
    }
}