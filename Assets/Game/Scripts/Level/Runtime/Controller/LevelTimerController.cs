using System;
using RollicGames.ColorBlockJamClone.Level.Runtime.Model;
using RollicGames.ColorBlockJamClone.Level.Runtime.Presenter;
using RollicGames.ColorBlockJamClone.Level.Runtime.View;
using Zenject;

namespace RollicGames.ColorBlockJamClone.Level.Runtime.Controller
{
    public interface ILevelTimerController
    {
        event Action OnLevelTimerExpired;

        void InitializeTimer();
        void PrepareForReuse();
    }

    public class LevelTimerController : ILevelTimerController
    {
        [Inject] private readonly LevelModel _levelModel;
        [Inject] private readonly ILevelView _levelView;
        [Inject] private readonly ILevelTickPresenter _tickPresenter;

        private int _displayedSeconds;

        public event Action OnLevelTimerExpired;

        public void InitializeTimer()
        {
            UpdateTimerView();

            _tickPresenter.OnTick += OnTick;
        }

        public void PrepareForReuse()
        {
            _tickPresenter.OnTick -= OnTick;
        }

        private void OnTick(float deltaTime)
        {
            if(!_levelModel.IsLevelStarted || _levelModel.IsLevelPaused 
            || _levelModel.RemainingDuration <= 0f) return;

            _levelModel.RemainingDuration = _levelModel.RemainingDuration - deltaTime;
            UpdateTimerView();

            if(_levelModel.RemainingDuration > 0f) return;

            OnLevelTimerExpired?.Invoke();
        }

        private void UpdateTimerView()
        {
            var remainingSeconds = (int)System.Math.Max(0f, System.Math.Ceiling(_levelModel.RemainingDuration));
            if(remainingSeconds == _displayedSeconds) return;

            _displayedSeconds = remainingSeconds;
            _levelView.SetTimerText(remainingSeconds);
        }
    }
}
