using System;

namespace RollicGames.ColorBlockJamClone.Level.Runtime.View
{
    public struct LevelFailPopupViewData
    {
        public readonly int Level;

        public readonly Action OnDismissButtonClicked;
        public readonly Action OnRetryButtonClicked;

        public LevelFailPopupViewData(int level, Action dismissAction, Action retryAction)
        {
            Level = level;

            OnDismissButtonClicked = dismissAction;
            OnRetryButtonClicked = retryAction;
        }
    }
}
