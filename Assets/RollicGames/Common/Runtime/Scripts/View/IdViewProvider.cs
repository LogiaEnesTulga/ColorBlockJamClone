using System.Collections.Generic;

namespace RollicGames.Common.Runtime.View
{
    public interface IIdViewProvider<T> where T : class
    {
        IEnumerable<T> Views { get; }

        bool TryGetId(T view, out int id);
        bool TryGetView(int id, out T view);
    }

    public interface IIdViewRegistry<T> where T : class
    {
        void Register(int id, T view);
        void Clear();
    }

    public class IdViewProvider<T> : IIdViewProvider<T>, IIdViewRegistry<T> where T : class
    {
        private readonly Dictionary<int, T> _viewsById = new();
        private readonly Dictionary<T, int> _idsByView = new();

        public IEnumerable<T> Views => _viewsById.Values;

        public void Register(int id, T view)
        {
            _viewsById.Add(id, view);
            _idsByView.Add(view, id);
        }

        public void Clear()
        {
            _viewsById.Clear();
            _idsByView.Clear();
        }

        public bool TryGetId(T view, out int id)
        {
            return _idsByView.TryGetValue(view, out id);
        }

        public bool TryGetView(int id, out T view)
        {
            return _viewsById.TryGetValue(id, out view);
        }
    }
}
