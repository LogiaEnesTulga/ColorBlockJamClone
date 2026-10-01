using RollicGames.ColorBlockJamClone.Level.Runtime.Model;
using RollicGames.ColorBlockJamClone.Level.Runtime.View;
using RollicGames.ColorBlockJamClone.Level.Runtime.Controller;
using RollicGames.ColorBlockJamClone.Level.Runtime.Presenter;
using RollicGames.Common.Runtime.View;
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
        [SerializeField] private RendererViewPool _gridCellPool;
        [SerializeField] private RendererViewPool _centerPartPool;
        [SerializeField] private RendererViewPool _edgePartPool;
        [SerializeField] private RendererViewPool _outerCornerPartPool;
        [SerializeField] private RendererViewPool _innerCornerPartPool;
        [SerializeField] private TransformViewPool _colliderPool;
        [SerializeField] private RendererViewPool _wallEdgePool;
        [SerializeField] private RendererViewPool _wallOuterCornerPool;
        [SerializeField] private RendererViewPool _wallInnerCornerPool;

        public override void InstallBindings()
        {
            Container.Bind<LevelModel>().AsSingle().NonLazy();
            Container.Bind<LevelGridModel>().AsSingle().NonLazy();
            Container.Bind<LevelGoalModel>().AsSingle().NonLazy();

            Container.Bind<IObjectPool<LevelBlockObjectModel>>().FromMethod(_ => new ObjectPool<LevelBlockObjectModel>(() => new LevelBlockObjectModel())).AsSingle();
            Container.Bind<IObjectPool<LevelDoorObjectModel>>().FromMethod(_ => new ObjectPool<LevelDoorObjectModel>(() => new LevelDoorObjectModel())).AsSingle();

            Container.Bind<IViewPool<LevelBlockObjectView>>().FromInstance(_blockViewPool).AsSingle();
            Container.Bind<IViewPool<LevelDoorView>>().FromInstance(_doorViewPool).AsSingle();
            Container.Bind<IViewPool<Renderer>>().WithId(LevelPartPoolIds.GridCell).FromInstance(_gridCellPool);
            Container.Bind<IViewPool<Renderer>>().WithId(LevelPartPoolIds.Center).FromInstance(_centerPartPool);
            Container.Bind<IViewPool<Renderer>>().WithId(LevelPartPoolIds.Edge).FromInstance(_edgePartPool);
            Container.Bind<IViewPool<Renderer>>().WithId(LevelPartPoolIds.OuterCorner).FromInstance(_outerCornerPartPool);
            Container.Bind<IViewPool<Renderer>>().WithId(LevelPartPoolIds.InnerCorner).FromInstance(_innerCornerPartPool);
            Container.Bind<IViewPool<Transform>>().WithId(LevelPartPoolIds.Collider).FromInstance(_colliderPool);
            Container.Bind<IViewPool<Renderer>>().WithId(LevelPartPoolIds.WallEdge).FromInstance(_wallEdgePool);
            Container.Bind<IViewPool<Renderer>>().WithId(LevelPartPoolIds.WallOuterCorner).FromInstance(_wallOuterCornerPool);
            Container.Bind<IViewPool<Renderer>>().WithId(LevelPartPoolIds.WallInnerCorner).FromInstance(_wallInnerCornerPool);

            Container.BindInterfacesTo<IdViewProvider<LevelBlockObjectView>>().AsSingle();
            Container.BindInterfacesTo<IdViewProvider<LevelDoorView>>().AsSingle();

            Container.BindInterfacesTo<LevelView>().FromInstance(_levelView).AsSingle().NonLazy();
            Container.BindInterfacesTo<LevelGridView>().FromInstance(_levelGridView).AsSingle().NonLazy();
            Container.BindInterfacesTo<LevelInputView>().FromInstance(_levelInputView).AsSingle().NonLazy();

            Container.BindInterfacesAndSelfTo<LevelController>().AsSingle().NonLazy();
            Container.BindInterfacesTo<LevelGridController>().AsSingle().NonLazy();
            Container.BindInterfacesTo<LevelFailPopupController>().AsSingle().NonLazy();
            Container.BindInterfacesTo<LevelCompletePopupController>().AsSingle().NonLazy();
            Container.BindInterfacesTo<LevelMoveController>().AsSingle().NonLazy();
            Container.BindInterfacesTo<LevelDoorsController>().AsSingle().NonLazy();
            Container.BindInterfacesTo<LevelGoalController>().AsSingle().NonLazy();
            Container.BindInterfacesTo<LevelTimerController>().AsSingle().NonLazy();
            Container.BindInterfacesTo<LevelTickPresenter>().AsSingle().NonLazy();
            Container.BindInterfacesAndSelfTo<LevelFlowController>().AsSingle().NonLazy();
        }
    }
}