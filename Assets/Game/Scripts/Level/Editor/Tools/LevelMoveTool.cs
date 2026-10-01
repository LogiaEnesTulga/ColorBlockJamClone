using System.Linq;
using RollicGames.Math.Runtime.Model;
using RollicGames.ColorBlockJamClone.Level.Runtime.Model;
using UnityEngine;
using UnityEngine.UIElements;

namespace RollicGames.ColorBlockJamClone.Level.Editor
{
    public class LevelMoveTool : LevelEditorToolBase
    {
        private readonly LevelObjectEditors _editors;

        private LevelObjectModel _draggedObject;
        private int2 _grabOffset;

        public override string DisplayName => "Move";
        public override string Hint => _draggedObject == null
            ? "Drag a block or door to move it."
            : "Release on green to move it here. Releasing on red cancels the move.";

        public LevelMoveTool(LevelEditorSession session, LevelObjectEditors editors) : base(session)
        {
            _editors = editors;
        }

        public override void DrawIcon(Painter2D painter, Rect rect, Color color)
        {
            LevelEditorIcons.DrawMove(painter, rect, color);
        }

        public override void Deactivate()
        {
            _draggedObject = null;
        }

        public override LevelEditorHover GetHover(int2 position)
        {
            if(_draggedObject != null)
            {
                var editor = _editors.For(_draggedObject);
                var targetOrigin = position - _grabOffset;
                var shift = targetOrigin - _draggedObject.GridPosition;
                var ghostPositions = editor.GetPositions(_draggedObject).Select(piece => piece + shift).ToList();
                var feedback = editor.CanMove(_draggedObject, targetOrigin) ? LevelEditorFeedback.Valid : LevelEditorFeedback.Invalid;
                return new LevelEditorHover(ghostPositions, feedback);
            }

            var target = Grid.GetObjectAt(position);
            if(target == null) return LevelEditorHover.None;

            return new LevelEditorHover(_editors.For(target).GetPositions(target), LevelEditorFeedback.Neutral);
        }

        public override void OnPointerDown(int2 position)
        {
            var target = Grid.GetObjectAt(position);
            if(target == null) return;

            Session.Select(target);
            _draggedObject = target;
            _grabOffset = position - target.GridPosition;
            NotifyStateChanged();
        }

        public override void OnPointerUp(int2 position)
        {
            if(_draggedObject == null) return;

            var editor = _editors.For(_draggedObject);
            var targetOrigin = position - _grabOffset;
            if(targetOrigin != _draggedObject.GridPosition && editor.CanMove(_draggedObject, targetOrigin))
            {
                editor.Move(_draggedObject, targetOrigin);
                Session.MarkChanged();
            }

            _draggedObject = null;
            NotifyStateChanged();
        }
    }
}
