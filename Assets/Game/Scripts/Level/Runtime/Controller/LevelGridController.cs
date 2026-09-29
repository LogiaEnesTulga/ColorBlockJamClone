using RollicGames.Math.Runtime.Model;
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
    }

    public class LevelGridController : ILevelGridController
    {
        [Inject] private readonly LevelGridModel _gridModel;
        [Inject] private readonly ILevelGridView _gridView;

        public int LevelWidth => _gridModel.Width;
        public int LevelHeight => _gridModel.Height;

        public void InitializeGrid()
        {
            CreateMockLevel();
            _gridView.InitializeView();
        }

        private void CreateMockLevel()
        {
            // TODO : Change here, its mocked Level 1 initializing.
            _gridModel.Width = 4;
            _gridModel.Height = 5;

            _gridModel.Cells.Clear();
            _gridModel.Blocks.Clear();
            _gridModel.Doors.Clear();

            var blueDoor = new LevelDoorObjectModel(new int2(1, 0), 3, LevelObjectColor.Blue, LevelDirection.Up);
            var purpleDoor = new LevelDoorObjectModel(new int2(0, 4), 3, LevelObjectColor.Purple, LevelDirection.Down);
            var blueBlock = new LevelBlockObjectModel(new int2(2, 1), LevelObjectColor.Blue, new()
            {
                new (0, 0), new (1, 0),
                new (0, 1), new (1, 1)
            });
            var purpleBlock = new LevelBlockObjectModel(new int2(0, 2), LevelObjectColor.Purple, new()
            {
                new (0, 0), new (1, 0),
                new (0, 1), new (1, 1)
            });

            for(var x = 0; x < _gridModel.Width; x++)
            {
                for(var y = 0; y < _gridModel.Height; y++)
                {
                    _gridModel.Cells.Add(new int2(x, y));
                }
            }

            AddBlockToGrid(blueBlock);
            AddBlockToGrid(purpleBlock);
            AddDoorToGrid(blueDoor);
            AddDoorToGrid(purpleDoor);
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