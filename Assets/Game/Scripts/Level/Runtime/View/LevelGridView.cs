using System.Threading;
using Cysharp.Threading.Tasks;
using RollicGames.Math.Runtime.Model;
using RollicGames.Common.Runtime.View;
using RollicGames.Pooling.Runtime.View;
using RollicGames.ColorBlockJamClone.Level.Runtime.Model;
using Zenject;
using UnityEngine;

namespace RollicGames.ColorBlockJamClone.Level.Runtime.View
{
    public interface ILevelGridView
    {
        void InitializeView();
        void PrepareForReuse();
        void MoveBlock(int id, int2 newPoint);
        void BeginDragBlock(int id);
        void DragBlock(int id, float2 worldDelta, bool canMoveLeft, bool canMoveRight, bool canMoveUp, bool canMoveDown);
        void EndDragBlock(int id);
        UniTask StartAbsorbingBlockAnimation(int doorId, int blockId, int2 lastPosition, LevelDirection direction, CancellationTokenSource cancellationToken);
    }

    public class LevelGridView : MonoBehaviour, ILevelGridView
    {
        [SerializeField] private float _cornerLength = 2f;

        [SerializeField] private Transform _gridParent;
        [SerializeField] private Transform _blocksParent;
        [SerializeField] private Transform _doorsParent;

        [SerializeField] private LevelWallView _levelWallView;

        [SerializeField] private LevelColorsConfig _colorsConfig;

        [Inject] private readonly LevelGridModel _gridModel;
        [Inject] private readonly IIdViewProvider<LevelBlockObjectView> _blockIdProvider;
        [Inject] private readonly IIdViewRegistry<LevelBlockObjectView> _blockIdRegistry;
        [Inject] private readonly IIdViewProvider<LevelDoorView> _doorIdProvider;
        [Inject] private readonly IIdViewRegistry<LevelDoorView> _doorIdRegistry;

        [Inject] private readonly IViewPool<LevelBlockObjectView> _blockViewPool;
        [Inject] private readonly IViewPool<LevelDoorView> _doorViewPool;

        [Inject(Id = LevelPartPoolIds.GridCell)] private readonly IViewPool<Renderer> _gridCellPool;
        [Inject(Id = LevelPartPoolIds.Center)] private readonly IViewPool<Renderer> _centerPartPool;
        [Inject(Id = LevelPartPoolIds.Edge)] private readonly IViewPool<Renderer> _edgePartPool;
        [Inject(Id = LevelPartPoolIds.OuterCorner)] private readonly IViewPool<Renderer> _outerCornerPartPool;
        [Inject(Id = LevelPartPoolIds.InnerCorner)] private readonly IViewPool<Renderer> _innerCornerPartPool;
        [Inject(Id = LevelPartPoolIds.Collider)] private readonly IViewPool<Transform> _colliderPool;
        [Inject(Id = LevelPartPoolIds.WallEdge)] private readonly IViewPool<Renderer> _wallEdgePool;
        [Inject(Id = LevelPartPoolIds.WallOuterCorner)] private readonly IViewPool<Renderer> _wallOuterCornerPool;
        [Inject(Id = LevelPartPoolIds.WallInnerCorner)] private readonly IViewPool<Renderer> _wallInnerCornerPool;

        
        public void InitializeView()
        {
            PrepareGrids();
            PrepareBlocks();
            PrepareDoors();
            PrepareWalls();
        }

        public void PrepareForReuse()
        {
            foreach(var blockView in _blockIdProvider.Views)
            {
                blockView.ResetView();
            }

            _centerPartPool.PoolAll();
            _edgePartPool.PoolAll();
            _colliderPool.PoolAll();
            _innerCornerPartPool.PoolAll();
            _outerCornerPartPool.PoolAll();
            _blockViewPool.PoolAll();

            _blockIdRegistry.Clear();
            _gridCellPool.PoolAll();

            _doorViewPool.PoolAll();
            _doorIdRegistry.Clear();

            _wallEdgePool.PoolAll();
            _wallOuterCornerPool.PoolAll();
            _wallInnerCornerPool.PoolAll();
        }

        private void PrepareGrids()
        {
            var gridColor = _colorsConfig.GridColor;

            foreach(var cell in _gridModel.Cells)
            {
                var createdMesh = _gridCellPool.Spawn(_gridParent);
                LevelViewHelper.ApplyColorProperty(createdMesh, gridColor);
                createdMesh.transform.rotation = Quaternion.Euler(-90f, 0f, 0f);
                createdMesh.transform.localPosition = new Vector3(cell.X, -cell.Y, 0f) * _cornerLength;
            }
        }

        private void PrepareBlocks()
        {
            foreach(var block in _gridModel.Blocks.ObjectsById.Values)
            {
                var color = _colorsConfig.GetColor(block.Color);
                var blockView = _blockViewPool.Spawn(_blocksParent);
                blockView.transform.localPosition = new Vector3(block.GridPosition.X, -block.GridPosition.Y, 0f) * _cornerLength;
                blockView.InitializeView(color, block.BlocksLocalPositions,
                    _centerPartPool, _edgePartPool, _outerCornerPartPool, _innerCornerPartPool, _colliderPool);

                _blockIdRegistry.Register(block.Id, blockView);
            }
        }

        private void PrepareDoors()
        {
            foreach(var door in _gridModel.Doors.ObjectsById.Values)
            {
                var doorColor = _colorsConfig.GetColor(door.Color);
                var doorArrowColor = _colorsConfig.GetDoorArrowColor(door.Color);
                var doorView = _doorViewPool.Spawn(_doorsParent);
                doorView.InitializeView(door.GridPosition, door.AbsorbDirection, door.Length, doorColor, doorArrowColor);

                _doorIdRegistry.Register(door.Id, doorView);
            }
        }

        private void PrepareWalls()
        {
            _levelWallView.GenerateWalls(_gridModel.Cells, _gridModel.Doors.ObjectsByPosition,
            _wallEdgePool, _wallOuterCornerPool, _wallInnerCornerPool, _colorsConfig.WallColor);
        }

        public void MoveBlock(int id, int2 newPoint)
        {
            if(!_blockIdProvider.TryGetView(id, out var blockView)) return;

            blockView.MoveToPoint(newPoint);
        }

        public void BeginDragBlock(int id)
        {
            if(_blockIdProvider.TryGetView(id, out var blockView)) blockView.BeginDrag();
        }

        public void DragBlock(int id, float2 worldDelta, bool canMoveLeft, bool canMoveRight, bool canMoveUp, bool canMoveDown)
        {
            if(_blockIdProvider.TryGetView(id, out var blockView)) blockView.DragByWorldDelta(new Vector3(worldDelta.X, worldDelta.Y, 0f), canMoveLeft, canMoveRight, canMoveUp, canMoveDown);
        }

        public void EndDragBlock(int id)
        {
            if(_blockIdProvider.TryGetView(id, out var blockView)) blockView.EndDrag();
        }

        public async UniTask StartAbsorbingBlockAnimation(int doorId, int blockId, int2 lastPosition, LevelDirection direction, CancellationTokenSource cancellationToken)
        {
            if(!_doorIdProvider.TryGetView(doorId, out var doorView)) return;
            if(!_blockIdProvider.TryGetView(blockId, out var blockView)) return;

            doorView.SetDoorOpen();

            var clippingLimit = direction is LevelDirection.Up or LevelDirection.Down ? doorView.transform.position.y : doorView.transform.position.x;
            await blockView.StartAbsorbingAnimation(lastPosition, direction, clippingLimit, cancellationToken);
            if(cancellationToken.IsCancellationRequested) return;

            await doorView.StartClosingAnimation();
        }

    }
}