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
        [SerializeField] private List<Entry> _doorArrowColors = new();
        [SerializeField] private Color _gridColor = Color.white;
        [SerializeField] private Color _wallColor = Color.white;

        private readonly Dictionary<LevelObjectColor, Color> _colorsDictionary = new Dictionary<LevelObjectColor, Color>();
        private readonly Dictionary<LevelObjectColor, Color> _doorArrowColorsDictionary = new Dictionary<LevelObjectColor, Color>();

        public Color GridColor => _gridColor;
        public Color WallColor => _wallColor;

        private void OnEnable()
        {
            UpdateDictionary();
        }

        private void OnValidate()
        {
            UpdateDictionary();
        }

        private void UpdateDictionary()
        {
            _colorsDictionary.Clear();
            _doorArrowColorsDictionary.Clear();
            
            foreach(var colorEntry in _colors)
            {
                _colorsDictionary.Add(colorEntry.Color, colorEntry.Value);
            }

            foreach(var colorEntry in _doorArrowColors)
            {
                _doorArrowColorsDictionary.Add(colorEntry.Color, colorEntry.Value);
            }
        }

        public Color GetColor(LevelObjectColor color)
        {
            return _colorsDictionary.GetValueOrDefault(color, Color.white);
        }

        public Color GetDoorArrowColor(LevelObjectColor color)
        {
            return _doorArrowColorsDictionary.GetValueOrDefault(color, Color.white);
        }
    }
}
