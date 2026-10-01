using RollicGames.Math.Runtime.Model;
using UnityEngine;
using UnityEngine.UIElements;

namespace RollicGames.ColorBlockJamClone.Level.Editor
{
    public class LevelSelectTool : LevelEditorToolBase
    {
        private readonly LevelObjectEditors _editors;

        public override string DisplayName => "Select";
        public override string Hint => "Click a block or door to select it. Click an empty spot to clear the selection.";

        public LevelSelectTool(LevelEditorSession session, LevelObjectEditors editors) : base(session)
        {
            _editors = editors;
        }

        public override void DrawIcon(Painter2D painter, Rect rect, Color color)
        {
            LevelEditorIcons.DrawSelect(painter, rect, color);
        }

        public override LevelEditorHover GetHover(int2 position)
        {
            var target = Grid.GetObjectAt(position);
            if(target == null) return LevelEditorHover.None;

            return new LevelEditorHover(_editors.For(target).GetPositions(target), LevelEditorFeedback.Neutral);
        }

        public override void OnPointerDown(int2 position)
        {
            Session.Select(Grid.GetObjectAt(position));
        }
    }
}
