using System;

namespace RollicGames.ColorBlockJamClone.Settings.Runtime.View
{
    public struct GameSettingsPopupViewData
    {
        public readonly bool IsAudioOn;
        public readonly bool IsMusicOn;
        public readonly bool IsHapticsOn;
        public readonly bool IsOpenedFromGameplay;

        public readonly Action OnAudioToggleClicked;
        public readonly Action OnMusicToggleClicked;
        public readonly Action OnHapticsToggleClicked;
        public readonly Action OnDismissButtonClicked;
        public readonly Action OnHomeButtonClicked;

        public GameSettingsPopupViewData(bool audio, bool music, bool haptics, bool fromGameplay,
         Action audioAction, Action musicAction, Action hapticsAction, Action dismissAction, Action homeAction)
        {
            IsAudioOn = audio;
            IsMusicOn = music;
            IsHapticsOn = haptics;
            IsOpenedFromGameplay = fromGameplay;

            OnAudioToggleClicked = audioAction;
            OnMusicToggleClicked = musicAction;
            OnHapticsToggleClicked = hapticsAction;
            OnDismissButtonClicked = dismissAction;
            OnHomeButtonClicked = homeAction;
        }
    }
}