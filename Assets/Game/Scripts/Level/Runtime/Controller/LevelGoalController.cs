using RollicGames.ColorBlockJamClone.Level.Runtime.Model;
using Zenject;

namespace RollicGames.ColorBlockJamClone.Level.Runtime.Controller
{
    public interface ILevelGoalController
    {
        void AddBlockGoal(int blockCount);
        bool DecreaseBlockGoal(int blockCount);
        void PrepareForReuse();
    }

    public class LevelGoalController : ILevelGoalController
    {
        [Inject] private readonly LevelGoalModel _goalModel;

        public void AddBlockGoal(int blockCount)
        {
            _goalModel.BlockGoal += blockCount;
        }

        public bool DecreaseBlockGoal(int blockCount)
        {
            _goalModel.BlockGoal -= blockCount;

            return _goalModel.BlockGoal <= 0;
        }

        public void PrepareForReuse()
        {
            _goalModel.BlockGoal = 0;
        }
    }
}
