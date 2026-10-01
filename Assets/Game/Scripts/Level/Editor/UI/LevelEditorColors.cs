using RollicGames.ColorBlockJamClone.Level.Runtime.Model;
using RollicGames.ColorBlockJamClone.Level.Runtime.View;
using UnityEditor;
using UnityEngine;

namespace RollicGames.ColorBlockJamClone.Level.Editor
{
    /// <summary>Resolves level object colors from the game's <see cref="LevelColorsConfig"/> so the editor matches the game.</summary>
    public class LevelEditorColors
    {
        private static readonly Color FallbackCellColor = new(0.566f, 0.566f, 0.566f, 1f);

        private readonly LevelColorsConfig _config;

        public Color CellColor => _config != null ? _config.GridColor : FallbackCellColor;

        private LevelEditorColors(LevelColorsConfig config)
        {
            _config = config;
        }

        public static LevelEditorColors Load()
        {
            var guids = AssetDatabase.FindAssets($"t:{nameof(LevelColorsConfig)}");
            var config = guids.Length > 0 ? AssetDatabase.LoadAssetAtPath<LevelColorsConfig>(AssetDatabase.GUIDToAssetPath(guids[0])) : null;
            if(config == null)
            {
                Debug.LogWarning($"[Level Editor] No {nameof(LevelColorsConfig)} asset found. Using fallback colors.");
            }

            return new LevelEditorColors(config);
        }

        public Color GetObjectColor(LevelObjectColor color)
        {
            return _config != null ? _config.GetColor(color) : Color.white;
        }

        public Color GetArrowColor(LevelObjectColor color)
        {
            return _config != null ? _config.GetDoorArrowColor(color) : Color.white;
        }
    }
}
