using System.Collections.Generic;
using RollicGames.Math.Runtime.Model;
using RollicGames.ColorBlockJamClone.Level.Runtime.Model;
using UnityEngine;
using UnityEngine.UIElements;

namespace RollicGames.ColorBlockJamClone.Level.Editor
{
    public class LevelDoorTool : LevelShapeDrawToolBase
    {
        private readonly LevelDoorEditor _doorEditor;

        public override string DisplayName => "Door";
        protected override string ObjectName => "Door";

        public override string Hint => DraftPositions.Count == 0
            ? "Click an empty spot next to a cell to start a door. Doors can't sit on cells."
            : "Extend the door in a straight line. Enter finishes the door, Esc cancels it.";

        public LevelDoorTool(LevelEditorSession session, LevelDoorEditor doorEditor) : base(session)
        {
            _doorEditor = doorEditor;
        }

        public override void DrawIcon(Painter2D painter, Rect rect, Color color)
        {
            LevelEditorIcons.DrawDoor(painter, rect, color);
        }

        protected override bool CanDraw(int2 position, IReadOnlyList<int2> draftPositions)
        {
            return _doorEditor.CanDrawSegment(position, draftPositions);
        }

        protected override LevelObjectModel CreateObject(IReadOnlyList<int2> draftPositions, LevelObjectColor color)
        {
            return _doorEditor.Create(draftPositions, color);
        }
    }
}
