using System.Collections.Generic;
using RollicGames.Math.Runtime.Model;
using RollicGames.ColorBlockJamClone.Level.Runtime.Model;
using Zenject;
using UnityEngine;

namespace RollicGames.ColorBlockJamClone.Level.Runtime.View
{
    public interface ILevelGridView
    {
        void InitializeView();
        void MoveBlock(int id, int2 newPoint);
        void BeginDragBlock(int id);
        void DragBlock(int id, float2 worldDelta, bool canMoveLeft, bool canMoveRight, bool canMoveUp, bool canMoveDown);
        void EndDragBlock(int id);
    }

    public class LevelGridView : MonoBehaviour, ILevelGridView
    {
        [SerializeField] private float _cornerLength = 2f;

        [SerializeField] private Transform _gridParent;
        [SerializeField] private Transform _blocksParent;
        [SerializeField] private Transform _doorsParent;

        [SerializeField] private GameObject _gridPrefab;
        [SerializeField] private LevelBlockObjectView _blockPrefab;
        [SerializeField] private LevelDoorView _doorPrefab;
        [SerializeField] private LevelColorsConfig _colorsConfig;

        [Inject] private readonly LevelGridModel _gridModel;

        private readonly Dictionary<int, LevelBlockObjectView> _blocksById = new();
        
        public void InitializeView()
        {
            ClearChildren(_gridParent);
            ClearChildren(_blocksParent);
            ClearChildren(_doorsParent);

            PrepareGrids();
            PrepareBlocks();
            PrepareDoors();
        }

        private static void ClearChildren(Transform parent)
        {
            for(var i = parent.childCount - 1; i >= 0; i--)
            {
                Destroy(parent.GetChild(i).gameObject);
            }
        }

        private void PrepareGrids()
        {
            var gridColor = _colorsConfig.GridColor;

            foreach(var cell in _gridModel.Cells)
            {
                var createdMesh = Instantiate(_gridPrefab, _gridParent);
                LevelViewHelper.ApplyColorProperty(createdMesh, gridColor);
                createdMesh.transform.localPosition = new Vector3(cell.X, -cell.Y, 0f) * _cornerLength;
            }
        }

        private void PrepareBlocks()
        {
            _blocksById.Clear();

            foreach(var block in _gridModel.Blocks.ObjectsById.Values)
            {
                var color = _colorsConfig.GetColor(block.Color);
                var blockView = Instantiate(_blockPrefab, _blocksParent);
                blockView.transform.localPosition = new Vector3(block.GridPosition.X, -block.GridPosition.Y, 0f) * _cornerLength;
                blockView.InitializeView(block.PoolId, color, block.BlocksLocalPositions);

                _blocksById.Add(block.PoolId, blockView);
            }
        }

        private void PrepareDoors()
        {
            foreach(var door in _gridModel.Doors.ObjectsById.Values)
            {
                var doorColor = _colorsConfig.GetColor(door.Color);
                var doorArrowColor = _colorsConfig.GetDoorArrowColor(door.Color);
                var doorView = Instantiate(_doorPrefab, _doorsParent);
                doorView.InitializeView(door.GridPosition, door.AbsorbDirection, door.Length, doorColor, doorArrowColor);
            }
        }

        public void MoveBlock(int id, int2 newPoint)
        {
            if(!_blocksById.TryGetValue(id, out var blockView)) return;

            blockView.MoveToPoint(newPoint);
        }

        public void BeginDragBlock(int id)
        {
            if(_blocksById.TryGetValue(id, out var blockView)) blockView.BeginDrag();
        }

        public void DragBlock(int id, float2 worldDelta, bool canMoveLeft, bool canMoveRight, bool canMoveUp, bool canMoveDown)
        {
            if(_blocksById.TryGetValue(id, out var blockView)) blockView.DragByWorldDelta(new Vector3(worldDelta.X, worldDelta.Y, 0f), canMoveLeft, canMoveRight, canMoveUp, canMoveDown);
        }

        public void EndDragBlock(int id)
        {
            if(_blocksById.TryGetValue(id, out var blockView)) blockView.EndDrag();
        }
    }
}