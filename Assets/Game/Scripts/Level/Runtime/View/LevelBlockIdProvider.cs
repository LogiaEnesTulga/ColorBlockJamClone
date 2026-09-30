using System.Collections.Generic;

namespace RollicGames.ColorBlockJamClone.Level.Runtime.View
{
    public interface ILevelBlockIdProvider
    {
        IEnumerable<LevelBlockObjectView> BlockViews { get; }

        bool TryGetBlockId(LevelBlockObjectView blockView, out int id);
        bool TryGetBlockView(int id, out LevelBlockObjectView blockView);
    }

    public interface ILevelBlockIdRegistry
    {
        void Register(int id, LevelBlockObjectView blockView);
        void Clear();
    }

    public class LevelBlockIdProvider : ILevelBlockIdProvider, ILevelBlockIdRegistry
    {
        private readonly Dictionary<int, LevelBlockObjectView> _viewsById = new();
        private readonly Dictionary<LevelBlockObjectView, int> _idsByView = new();

        public IEnumerable<LevelBlockObjectView> BlockViews => _viewsById.Values;

        public void Register(int id, LevelBlockObjectView blockView)
        {
            _viewsById.Add(id, blockView);
            _idsByView.Add(blockView, id);
        }

        public void Clear()
        {
            _viewsById.Clear();
            _idsByView.Clear();
        }

        public bool TryGetBlockId(LevelBlockObjectView blockView, out int id)
        {
            return _idsByView.TryGetValue(blockView, out id);
        }

        public bool TryGetBlockView(int id, out LevelBlockObjectView blockView)
        {
            return _viewsById.TryGetValue(id, out blockView);
        }
    }
}
