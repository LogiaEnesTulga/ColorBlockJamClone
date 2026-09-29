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

        private readonly LevelObjectColor _color;
        private readonly List<int2> _blocksLocalPositions;

        private bool _isActive = true;

        public LevelBlockObjectModel(int2 gridPosition, LevelObjectColor color, List<int2> blocksLocalPositions) : base(gridPosition)
        {
            _color = color;
            _blocksLocalPositions = blocksLocalPositions;
        }

        public void SetActive(bool active)
        {
            _isActive = active;
        }
    }
}