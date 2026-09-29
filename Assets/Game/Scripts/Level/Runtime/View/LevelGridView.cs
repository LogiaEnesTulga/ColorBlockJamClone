using RollicGames.ColorBlockJamClone.Level.Runtime.Model;
using Zenject;
using UnityEngine;

namespace RollicGames.ColorBlockJamClone.Level.Runtime.View
{
    public interface ILevelGridView
    {
        void InitializeView();
    }

    public class LevelGridView : MonoBehaviour, ILevelGridView
    {
        [SerializeField] private float _cornerLength = 2f;

        [SerializeField] private Transform _gridParent;
        [SerializeField] private Transform _blocksParent;

        [SerializeField] private GameObject _gridPrefab;
        [SerializeField] private LevelBlockObjectView _blockPrefab;
        [SerializeField] private LevelColorsConfig _colorsConfig;

        [Inject] private readonly LevelGridModel _gridModel;
        
        public void InitializeView()
        {
            PrepareGrids();
            PrepareBlocks();
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
            foreach(var block in _gridModel.Blocks)
            {
                var color = _colorsConfig.GetColor(block.Color);
                var blockView = Instantiate(_blockPrefab, _blocksParent);
                blockView.transform.localPosition = new Vector3(block.GridPosition.X, -block.GridPosition.Y, 0f) * _cornerLength;
                blockView.InitializeView(block.BlocksLocalPositions, color);
            }
        }
    }
}