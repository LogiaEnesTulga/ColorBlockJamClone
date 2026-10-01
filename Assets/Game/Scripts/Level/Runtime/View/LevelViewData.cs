using System;

namespace RollicGames.ColorBlockJamClone.Level.Runtime.View
{
    public struct LevelViewData
    {
        public int PlayerLevel;
        public int PlayerCoinAmount;

        public int LevelWidth;
        public int LevelHeight;

        public Action OnPauseButtonClick;
        public Action OnRetryButtonClick;
    }
}