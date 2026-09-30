using RollicGames.ColorBlockJamClone.Level.Runtime.Model;
using RollicGames.ColorBlockJamClone.Level.Runtime.View;
using RollicGames.ColorBlockJamClone.Level.Runtime.Controller;
using RollicGames.Pooling.Runtime.Controller;
using RollicGames.Pooling.Runtime.View;
using Zenject;
using UnityEngine;

namespace RollicGames.ColorBlockJamClone.Level.Runtime.Injection
{
    public class LevelInstaller : MonoInstaller
    {
        [SerializeField] private LevelView _levelView;
        [SerializeField] private LevelGridView _levelGridView;
        [SerializeField] private LevelInputView _levelInputView;

        [Header("View Pools")]
        [SerializeField] private LevelBlockObjectViewPool _blockViewPool;
        [SerializeField] private LevelDoorViewPool _doorViewPool;
        [SerializeField] private TransformViewPool _gridCellPool;
        [SerializeField] private TransformViewPool _centerPartPool;
        [SerializeField] private TransformViewPool _edgePartPool;
        [SerializeField] private TransformViewPool _outerCornerPartPool;
        [SerializeField] private TransformViewPool _innerCornerPartPool;
        [SerializeField] private TransformViewPool _colliderPool;

        public override void InstallBindings()
        {
            Container.Bind<LevelModel>().AsSingle().NonLazy();
            Container.Bind<LevelGridModel>().AsSingle().NonLazy();

            Container.Bind<IObjectPool<LevelBlockObjectModel>>().FromMethod(_ => new ObjectPool<LevelBlockObjectModel>(() => new LevelBlockObjectModel())).AsSingle();
            Container.Bind<IObjectPool<LevelDoorObjectModel>>().FromMethod(_ => new ObjectPool<LevelDoorObjectModel>(() => new LevelDoorObjectModel())).AsSingle();

            Container.Bind<IViewPool<LevelBlockObjectView>>().FromInstance(_blockViewPool).AsSingle();
            Container.Bind<IViewPool<LevelDoorView>>().FromInstance(_doorViewPool).AsSingle();
            Container.Bind<IViewPool<Transform>>().WithId(LevelPartPoolIds.GridCell).FromInstance(_gridCellPool);
            Container.Bind<IViewPool<Transform>>().WithId(LevelPartPoolIds.Center).FromInstance(_centerPartPool);
            Container.Bind<IViewPool<Transform>>().WithId(LevelPartPoolIds.Edge).FromInstance(_edgePartPool);
            Container.Bind<IViewPool<Transform>>().WithId(LevelPartPoolIds.OuterCorner).FromInstance(_outerCornerPartPool);
            Container.Bind<IViewPool<Transform>>().WithId(LevelPartPoolIds.InnerCorner).FromInstance(_innerCornerPartPool);
            Container.Bind<IViewPool<Transform>>().WithId(LevelPartPoolIds.Collider).FromInstance(_colliderPool);

            Container.BindInterfacesTo<LevelBlockIdProvider>().AsSingle();

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