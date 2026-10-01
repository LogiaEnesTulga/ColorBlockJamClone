using System.Collections.Generic;
using RollicGames.Math.Runtime.Model;
using RollicGames.ColorBlockJamClone.Level.Runtime.Model;
using UnityEngine;
using UnityEngine.UIElements;

namespace RollicGames.ColorBlockJamClone.Level.Editor
{
    public class LevelBlockTool : LevelShapeDrawToolBase
    {
        private readonly LevelBlockEditor _blockEditor;

        public override string DisplayName => "Block";
        protected override string ObjectName => "Block";

        public override string Hint => DraftPositions.Count == 0
            ? "Click an empty cell to start a block."
            : "Add pieces next to the ones already placed. Enter finishes the block, Esc cancels it.";

        public LevelBlockTool(LevelEditorSession session, LevelBlockEditor blockEditor) : base(session)
        {
            _blockEditor = blockEditor;
        }

        public override void DrawIcon(Painter2D painter, Rect rect, Color color)
        {
            LevelEditorIcons.DrawBlock(painter, rect, color);
        }

        protected override bool CanDraw(int2 position, IReadOnlyList<int2> draftPositions)
        {
            return _blockEditor.CanDrawPiece(position, draftPositions);
        }

        protected override LevelObjectModel CreateObject(IReadOnlyList<int2> draftPositions, LevelObjectColor color)
        {
            return _blockEditor.Create(draftPositions, color);
        }
    }
}
