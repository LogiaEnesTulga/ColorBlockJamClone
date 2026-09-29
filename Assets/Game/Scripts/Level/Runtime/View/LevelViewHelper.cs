using UnityEngine;

namespace RollicGames.ColorBlockJamClone.Level.Runtime.View
{
    public static class LevelViewHelper
    {
        private static readonly int ColorPropertyId = Shader.PropertyToID("_Color");
        private static MaterialPropertyBlock PropertyBlock;

        public static void ApplyColorProperty(GameObject obj, Color color)
        {
            var renderer = obj.GetComponent<Renderer>();
            if(renderer == null) return;

            ApplyColorProperty(renderer, color);
        }

        public static void ApplyColorProperty(Renderer renderer, Color color)
        {
            PropertyBlock ??= new();

            renderer.GetPropertyBlock(PropertyBlock);
            PropertyBlock.SetColor(ColorPropertyId, color);
            renderer.SetPropertyBlock(PropertyBlock);
        }
    }
}
