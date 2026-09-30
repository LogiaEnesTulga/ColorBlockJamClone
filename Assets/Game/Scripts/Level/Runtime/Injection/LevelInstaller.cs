using RollicGames.ColorBlockJamClone.Level.Runtime.Model;
using RollicGames.ColorBlockJamClone.Level.Runtime.View;
using RollicGames.ColorBlockJamClone.Level.Runtime.Controller;
using RollicGames.Pooling.Runtime.Controller;
using Zenject;
using UnityEngine;

namespace RollicGames.ColorBlockJamClone.Level.Runtime.Injection
{
    public class LevelInstaller : MonoInstaller
    {
        [SerializeField] private LevelView _levelView;
        [SerializeField] private LevelGridView _levelGridView;
        [SerializeField] private LevelInputView _levelInputView;

        public override void InstallBindings()
        {
            Container.Bind<LevelModel>().AsSingle().NonLazy();
            Container.Bind<LevelGridModel>().AsSingle().NonLazy();

            Container.Bind<IObjectPool<LevelBlockObjectModel>>().FromMethod(_ => new ObjectPool<LevelBlockObjectModel>(() => new LevelBlockObjectModel())).AsSingle();
            Container.Bind<IObjectPool<LevelDoorObjectModel>>().FromMethod(_ => new ObjectPool<LevelDoorObjectModel>(() => new LevelDoorObjectModel())).AsSingle();

            Container.BindInterfacesTo<LevelView>().FromInstance(_levelView).AsSingle().NonLazy();
            Container.BindInterfacesTo<LevelGridView>().FromInstance(_levelGridView).AsSingle().NonLazy();
            Container.BindInterfacesTo<LevelInputView>().FromInstance(_levelInputView).AsSingle().NonLazy();

            Container.BindInterfacesAndSelfTo<LevelController>().AsSingle().NonLazy();
            Container.BindInterfacesTo<LevelGridController>().AsSingle().NonLazy();
            Container.BindInterfacesTo<LevelMoveController>().AsSingle().NonLazy();
            Container.BindInterfacesAndSelfTo<LevelFlowController>().AsSingle().NonLazy();
        }
    }
}