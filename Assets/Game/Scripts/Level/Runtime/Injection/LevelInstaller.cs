using RollicGames.ColorBlockJamClone.Level.Runtime.Model;
using RollicGames.ColorBlockJamClone.Level.Runtime.View;
using RollicGames.ColorBlockJamClone.Level.Runtime.Controller;
using Zenject;
using UnityEngine;

namespace RollicGames.ColorBlockJamClone.Level.Runtime.Injection
{
    public class LevelInstaller : MonoInstaller
    {
        [SerializeField] private LevelView _levelView;

        public override void InstallBindings()
        {
            Container.Bind<LevelModel>().AsSingle().NonLazy();
            Container.BindInterfacesTo<LevelView>().FromInstance(_levelView).AsSingle().NonLazy();
            Container.BindInterfacesAndSelfTo<LevelController>().AsSingle().NonLazy();
        }
    }
}