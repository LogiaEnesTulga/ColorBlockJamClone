using UnityEngine;

namespace RollicGames.ColorBlockJamClone.Level.Runtime.View
{
    public static class LevelViewHelper
    {
        private static readonly int ColorPropertyId = Shader.PropertyToID("_Color");
        private static MaterialPropertyBlock PropertyBlock;

        public static void ApplyColorProperty(GameObject obj, Color color)
        {
            PropertyBlock ??= new();

            var renderer = obj.GetComponent<Renderer>();
            if(renderer == null) return;

            renderer.GetPropertyBlock(PropertyBlock);
            PropertyBlock.SetColor(ColorPropertyId, color);
            renderer.SetPropertyBlock(PropertyBlock);
        }
    }
}
