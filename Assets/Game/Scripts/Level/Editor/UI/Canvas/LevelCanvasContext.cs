using RollicGames.ColorBlockJamClone.Level.Runtime.Model;
using UnityEngine;
using UnityEngine.UIElements;

namespace RollicGames.ColorBlockJamClone.Level.Editor
{
    public class LevelCanvasContext
    {
        public readonly LevelGridModel Grid;
        public readonly LevelEditorColors Colors;
        public readonly LevelCanvasLayout Layout;
        public readonly LevelObjectEditors Editors;
        public readonly LevelObjectModel Selected;
        public readonly ILevelEditorTool Tool;
        public readonly LevelEditorHover Hover;
        public readonly Vector2? PointerPosition;

        public LevelCanvasContext(LevelGridModel grid, LevelEditorColors colors, LevelCanvasLayout layout,
            LevelObjectEditors editors = null, LevelObjectModel selected = null, ILevelEditorTool tool = null,
            LevelEditorHover hover = default, Vector2? pointerPosition = null)
        {
            Grid = grid;
            Colors = colors;
            Layout = layout;
            Editors = editors;
            Selected = selected;
            Tool = tool;
            Hover = hover;
            PointerPosition = pointerPosition;
        }
    }

    public interface ILevelCanvasLayer
    {
        void Draw(Painter2D painter, LevelCanvasContext context);
    }
}
