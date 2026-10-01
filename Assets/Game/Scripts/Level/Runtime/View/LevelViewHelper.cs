using UnityEngine;

namespace RollicGames.ColorBlockJamClone.Level.Runtime.View
{
    public static class LevelViewHelper
    {
        private static readonly int ColorPropertyId = Shader.PropertyToID("_Color");
        private static readonly int ClipDirectionPropertyId = Shader.PropertyToID("_ClipDirection");
        private static readonly int ClipLimitPropertyId = Shader.PropertyToID("_ClipLimit");
        private static MaterialPropertyBlock PropertyBlock;

        public static void ApplyColorProperty(Renderer renderer, Color color)
        {
            PropertyBlock ??= new();

            renderer.GetPropertyBlock(PropertyBlock);
            PropertyBlock.SetColor(ColorPropertyId, color);
            renderer.SetPropertyBlock(PropertyBlock);
        }

        public static void ApplyClipProperties(Renderer renderer, Vector2 direction, float limit)
        {
            PropertyBlock ??= new();

            renderer.GetPropertyBlock(PropertyBlock);
            PropertyBlock.SetVector(ClipDirectionPropertyId, direction);
            PropertyBlock.SetFloat(ClipLimitPropertyId, limit);
            renderer.SetPropertyBlock(PropertyBlock);
        }

        public static void ChangeMaterial(Renderer renderer, Material material)
        {
            if(renderer == null || material == null) return;

            renderer.sharedMaterial = material;
        }
    }
}
