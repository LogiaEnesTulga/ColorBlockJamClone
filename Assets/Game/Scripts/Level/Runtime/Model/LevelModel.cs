using System.Threading;

namespace RollicGames.ColorBlockJamClone.Level.Runtime.Model
{
    public class LevelModel
    {
        public bool IsLevelStarted = false;
        public bool IsLevelPaused = false;
        public bool IsLevelFinished = false;

        public float RemainingDuration;

        public CancellationTokenSource LevelCancellationToken = new();
    }
}