using System;
using System.Collections.Generic;
using RollicGames.ColorBlockJamClone.Level.Runtime.Model;
using UnityEngine;

namespace RollicGames.ColorBlockJamClone.Level.Runtime.View
{
    [CreateAssetMenu(fileName = "LevelColorsConfig", menuName = "Game/Configs/Level Colors Config")]
    public class LevelColorsConfig : ScriptableObject
    {
        [Serializable]
        public struct Entry
        {
            public LevelObjectColor Color;
            public Color Value;
        }

        [SerializeField] private List<Entry> _colors = new();

        private readonly Dictionary<LevelObjectColor, Color> _colorsDictionary = new Dictionary<LevelObjectColor, Color>();

        private void OnEnable()
        {
            UpdateDictionary();
        }

        private void Oalidate()
        {
            UpdateDictionary();
        }

        private void UpdateDictionary()
        {
            _colorsDictionary.Clear();
            
            foreach(var colorEntry in _colors)
            {
                _colorsDictionary.Add(colorEntry.Color, colorEntry.Value);
            }
        }

        public Color GetColor(LevelObjectColor color)
        {
            return _colorsDictionary.GetValueOrDefault(color, Color.white);
        }
    }
}
