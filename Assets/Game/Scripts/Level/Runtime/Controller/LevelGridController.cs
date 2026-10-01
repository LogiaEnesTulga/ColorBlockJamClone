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

        public int LevelWidth => _gridModel.Width;
        public int LevelHeight => _gridModel.Height;

        public void InitializeGrid()
        {
            _gridView.InitializeView();
        }

        public void PrepareForReuse()
        {
            _gridView.PrepareForReuse();

            _gridModel.Cells.Clear();
            _gridModel.Blocks.Clear();
            _gridModel.Doors.Clear();
        }
    }
}