using System;
using UnityEngine;
using Zenject;

namespace RollicGames.ColorBlockJamClone.Level.Runtime.Presenter
{
    public interface ILevelTickPresenter
    {
        event Action<float> OnTick;
    }

    public class LevelTickPresenter : ILevelTickPresenter, ITickable
    {
        public event Action<float> OnTick;

        public void Tick()
        {
            OnTick?.Invoke(Time.deltaTime);
        }
    }
}
