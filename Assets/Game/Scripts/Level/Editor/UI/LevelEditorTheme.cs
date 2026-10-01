using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

namespace RollicGames.ColorBlockJamClone.Level.Editor
{
    public static class LevelEditorTheme
    {
        public static readonly Color Background = FromHex("#16171B");
        public static readonly Color Board = FromHex("#24262D");
        public static readonly Color BoardRing = FromHex("#1D1E23");
        public static readonly Color SlotDot = new(1f, 1f, 1f, 0.09f);
        public static readonly Color Accent = FromHex("#5B8CFF");
        public static readonly Color Valid = FromHex("#3DDC84");
        public static readonly Color Invalid = FromHex("#FF5A5F");
        public static readonly Color Neutral = FromHex("#E8EAF0");

        private const string StyleSheetName = "LevelEditorWindow";

        public static Color GetFeedbackColor(LevelEditorFeedback feedback)
        {
            return feedback switch
            {
                LevelEditorFeedback.Valid => Valid,
                LevelEditorFeedback.Invalid => Invalid,
                _ => Neutral,
            };
        }

        public static StyleSheet LoadStyleSheet()
        {
            var guids = AssetDatabase.FindAssets($"{StyleSheetName} t:StyleSheet");
            if(guids.Length == 0)
            {
                Debug.LogWarning($"[Level Editor] {StyleSheetName}.uss not found.");
                return null;
            }

            return AssetDatabase.LoadAssetAtPath<StyleSheet>(AssetDatabase.GUIDToAssetPath(guids[0]));
        }

        private static Color FromHex(string hex)
        {
            ColorUtility.TryParseHtmlString(hex, out var color);
            return color;
        }
    }
}
