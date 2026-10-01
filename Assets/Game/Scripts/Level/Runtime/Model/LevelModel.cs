using System.Threading;

namespace RollicGames.ColorBlockJamClone.Level.Runtime.Model
{
    public class LevelModel
    {
        public const int LevelCompleteCoinReward = 500;

        public bool IsLevelStarted = false;
        public bool IsLevelPaused = false;
        public bool IsLevelFinished = false;

        public float RemainingDuration;

        public CancellationTokenSource LevelCancellationToken = new();
    }
}