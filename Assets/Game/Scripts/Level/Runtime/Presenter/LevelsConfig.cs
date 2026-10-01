using System.Collections.Generic;
using UnityEngine;

namespace RollicGames.ColorBlockJamClone.Level.Runtime.Presenter
{
    public interface ILevelsProvider
    {
        int LevelCount { get; }

        string GetLevel(int levelIndex);
    }

    [CreateAssetMenu(fileName = "LevelsConfig", menuName = "Game/Configs/Levels Config")]
    public class LevelsConfig : ScriptableObject, ILevelsProvider
    {
        [SerializeField] private List<TextAsset> _levels = new();

        public int LevelCount => _levels.Count;

        public string GetLevel(int levelIndex)
        {
            return _levels[levelIndex].text;
        }
    }
}
