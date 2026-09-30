using System.Collections.Generic;
using RollicGames.Math.Runtime.Model;
using RollicGames.Pooling.Runtime.Controller;
using RollicGames.ColorBlockJamClone.Level.Runtime.Model;
using RollicGames.ColorBlockJamClone.Level.Runtime.View;
using Zenject;

namespace RollicGames.ColorBlockJamClone.Level.Runtime.Controller
{
    public interface ILevelGridController
    {
        public int LevelWidth { get; }
        public int LevelHeight { get; }

        void InitializeGrid();
        void PrepareForReuse();
    }

    public class LevelGridController : ILevelGridController
    {
        [Inject] private readonly LevelGridModel _gridModel;
        [Inject] private readonly ILevelGridView _gridView;
        [Inject] private readonly IObjectPool<LevelBlockObjectModel> _blockPool;
        [Inject] private readonly IObjectPool<LevelDoorObjectModel> _doorPool;

        public int LevelWidth => _gridModel.Width;
        public int LevelHeight => _gridModel.Height;

        public void InitializeGrid()
        {
            CreateMockLevel();
            _gridView.InitializeView();
        }

        public void PrepareForReuse()
        {
            _gridView.PrepareForReuse();

            _blockPool.PoolAll();
            _doorPool.PoolAll();

            _gridModel.Cells.Clear();
            _gridModel.Blocks.Clear();
            _gridModel.Doors.Clear();
        }

        private void CreateMockLevel()
        {
            // TODO : Change here, its mocked Level 1 initializing.
            _gridModel.Width = 5;
            _gridModel.Height = 6;

            var blueDoor = _doorPool.Spawn();
            blueDoor.Initialize(new int2(2, -1), 3, LevelObjectColor.Blue, LevelDirection.Up);
            var purpleDoor = _doorPool.Spawn();
            purpleDoor.Initialize(new int2(0, 6), 3, LevelObjectColor.Purple, LevelDirection.Down);
            var greenDoor = _doorPool.Spawn();
            greenDoor.Initialize(new int2(5, 0), 3, LevelObjectColor.Green, LevelDirection.Right);
            var yellowDoor = _doorPool.Spawn();
            yellowDoor.Initialize(new int2(-1, 2), 3, LevelObjectColor.Yellow, LevelDirection.Left);

            var squareLocalPositions = new List<int2>()
            {
                new (0, 0), new (1, 0),
                new (0, 1), new (1, 1)
            };
            var blueBlock = _blockPool.Spawn();
            blueBlock.Initialize(new int2(2, 1), LevelObjectColor.Blue, squareLocalPositions);
            var purpleBlock = _blockPool.Spawn();
            purpleBlock.Initialize(new int2(0, 2), LevelObjectColor.Purple, squareLocalPositions);

            for(var x = 0; x < _gridModel.Width; x++)
            {
                for(var y = 0; y < _gridModel.Height; y++)
                {
                    if((x == 2 || x == 3 || x == 4) && (y == 3 || y == 4)) continue;
                    _gridModel.Cells.Add(new int2(x, y));
                }
            }

            AddBlockToGrid(blueBlock);
            AddBlockToGrid(purpleBlock);
            AddDoorToGrid(blueDoor);
            AddDoorToGrid(purpleDoor);
            AddDoorToGrid(greenDoor);
            AddDoorToGrid(yellowDoor);
        }

        private void AddDoorToGrid(LevelDoorObjectModel doorObject)
        {
            _gridModel.Doors.AddObjectWithId(doorObject);

            var isVertical = doorObject.AbsorbDirection is LevelDirection.Up or LevelDirection.Down;
            var direction = new int2(isVertical ? 1 : 0, isVertical ? 0 : 1);
            for(var i = 0; i < doorObject.Length; i++)
            {
                var activePosition = doorObject.GridPosition + (direction * i);
                _gridModel.Doors.ObjectsByPosition.Add(activePosition, doorObject);
            }
        }

        private void AddBlockToGrid(LevelBlockObjectModel blockObject)
        {
            _gridModel.Blocks.AddObjectWithId(blockObject);

            var localPositions = blockObject.BlocksLocalPositions;
            for(var i = 0; i < localPositions.Count; i++)
            {
                var activePosition = blockObject.GridPosition + localPositions[i];
                _gridModel.Blocks.ObjectsByPosition.Add(activePosition, blockObject);
            }
        }
    }
}