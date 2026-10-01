using System;

namespace RollicGames.ColorBlockJamClone.Level.Runtime.View
{
    public struct LevelCompletePopupViewData
    {
        public readonly int Level;
        public readonly int CoinAmount;

        public readonly Action OnDismissButtonClicked;
        public readonly Action OnNextLevelButtonClicked;

        public LevelCompletePopupViewData(int level, int coinAmount, Action dismissAction, Action nextLevelAction)
        {
            Level = level;
            CoinAmount = coinAmount;

            OnDismissButtonClicked = dismissAction;
            OnNextLevelButtonClicked = nextLevelAction;
        }
    }
}
