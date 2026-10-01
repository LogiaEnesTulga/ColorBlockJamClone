using System.Collections.Generic;
using RollicGames.ColorBlockJamClone.Level.Runtime.Model;
using UnityEngine.UIElements;

namespace RollicGames.ColorBlockJamClone.Level.Editor
{
    /// <summary>Non-interactive thumbnail of a level.</summary>
    public class LevelGridPreview : VisualElement
    {
        private const float Padding = 4f;
        private const float MaxCellSize = 16f;

        private readonly LevelGridModel _grid;
        private readonly LevelEditorColors _colors;
        private readonly IReadOnlyList<ILevelCanvasLayer> _layers;

        public LevelGridPreview(LevelGridModel grid, LevelEditorColors colors)
        {
            _grid = grid;
            _colors = colors;
            _layers = LevelCanvasLayers.CreatePreviewLayers();

            pickingMode = PickingMode.Ignore;
            generateVisualContent += OnGenerateVisualContent;
            RegisterCallback<GeometryChangedEvent>(_ => MarkDirtyRepaint());
        }

        private void OnGenerateVisualContent(MeshGenerationContext meshGenerationContext)
        {
            var layout = LevelCanvasLayout.Create(contentRect, _grid.Width, _grid.Height, Padding, MaxCellSize);
            var context = new LevelCanvasContext(_grid, _colors, layout);
            foreach(var layer in _layers)
            {
                layer.Draw(meshGenerationContext.painter2D, context);
            }
        }
    }
}
