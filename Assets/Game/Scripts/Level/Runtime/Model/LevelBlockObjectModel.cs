using System.Collections.Generic;
using RollicGames.Math.Runtime.Model;

namespace RollicGames.ColorBlockJamClone.Level.Runtime.Model
{
    public class LevelBlockObjectModel : LevelObjectModel
    {
        public override LevelObjectType ObjectType => LevelObjectType.Block;
        public LevelObjectColor Color => _color;
        public IReadOnlyList<int2> BlocksLocalPositions => _blocksLocalPositions;

        public bool IsActive => _isActive;

        private readonly List<int2> _blocksLocalPositions = new();

        private LevelObjectColor _color;
        private bool _isActive = true;

        public void Initialize(int2 gridPosition, LevelObjectColor color, IReadOnlyList<int2> blocksLocalPositions)
        {
            _gridPosition = gridPosition;
            _color = color;
            _blocksLocalPositions.AddRange(blocksLocalPositions);
        }

        public void SetActive(bool active)
        {
            _isActive = active;
        }

        public override void OnReturnedToPool()
        {
            _gridPosition = int2.Zero;
            _color = default;
            _isActive = true;
            _blocksLocalPositions.Clear();
        }
    }
}
