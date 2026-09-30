using System.Collections.Generic;
using RollicGames.Math.Runtime.Model;

namespace RollicGames.ColorBlockJamClone.Level.Runtime.Model
{
    public class LevelObjectContainer<T> where T : LevelObjectModel
    {
        public readonly Dictionary<int, T> ObjectsById = new();
        public readonly Dictionary<int2, T> ObjectsByPosition = new();

        public void AddObjectWithId(T obj)
        {
            ObjectsById.Add(obj.Id, obj);
        }

        public void Clear()
        {
            ObjectsById.Clear();
            ObjectsByPosition.Clear();
        }
    }
}