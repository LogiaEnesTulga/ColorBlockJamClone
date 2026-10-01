using System;
using System.Collections.Generic;
using RollicGames.Math.Runtime.Model;
using UnityEngine;
using UnityEngine.UIElements;

namespace RollicGames.ColorBlockJamClone.Level.Editor
{
    /// <summary>
    /// Without a selection, deletes whole objects, or the cell under the cursor when it holds no object.
    /// With a selection, deletes the selected object piece by piece. Only pieces whose removal keeps the object connected can be deleted.
    /// Clicking anything other than the selected object clears the selection first.
    /// </summary>
    public class LevelDeleteTool : LevelEditorToolBase
    {
        private readonly LevelObjectEditors _editors;
        private readonly LevelCellEditor _cellEditor;

        public override string DisplayName => "Delete";
        public override string Hint => Session.Selected == null
            ? "Click a block or door to delete it. Click an empty cell to delete the cell."
            : "Deleting the selected object piece by piece. Green pieces can be deleted. Click elsewhere to clear the selection.";

        public LevelDeleteTool(LevelEditorSession session, LevelObjectEditors editors, LevelCellEditor cellEditor) : base(session)
        {
            _editors = editors;
            _cellEditor = cellEditor;
        }

        public override void DrawIcon(Painter2D painter, Rect rect, Color color)
        {
            LevelEditorIcons.DrawDelete(painter, rect, color);
        }

        public override LevelEditorHover GetHover(int2 position)
        {
            var action = ResolveAction(position);
            if(action.Feedback == LevelEditorFeedback.None) return LevelEditorHover.None;

            return new LevelEditorHover(action.Positions, action.Feedback, LevelEditorCursor.Delete);
        }

        public override void OnPointerDown(int2 position)
        {
            var action = ResolveAction(position);
            if(action.Feedback is LevelEditorFeedback.None or LevelEditorFeedback.Invalid) return;

            action.Execute();
            NotifyStateChanged();
        }

        private DeleteAction ResolveAction(int2 position)
        {
            var selected = Session.Selected;
            var target = Grid.GetObjectAt(position);

            if(target != null)
            {
                var editor = _editors.For(target);
                if(selected != null && selected != target) return CreateDeselectAction(editor.GetPositions(target));

                if(selected == target)
                {
                    var canRemovePiece = editor.CanRemovePiece(target, position);
                    return new DeleteAction(new[] { position }, canRemovePiece ? LevelEditorFeedback.Valid : LevelEditorFeedback.Invalid, () =>
                    {
                        editor.RemovePiece(target, position);
                        Session.MarkChanged();
                    });
                }

                return new DeleteAction(editor.GetPositions(target), LevelEditorFeedback.Valid, () =>
                {
                    editor.Remove(target);
                    Session.MarkChanged();
                });
            }

            if(!_cellEditor.CanRemove(position)) return DeleteAction.None;
            if(selected != null) return CreateDeselectAction(new[] { position });

            return new DeleteAction(new[] { position }, LevelEditorFeedback.Valid, () =>
            {
                _cellEditor.Remove(position);
                Session.MarkChanged();
            });
        }

        private DeleteAction CreateDeselectAction(IReadOnlyList<int2> positions)
        {
            return new DeleteAction(positions, LevelEditorFeedback.Neutral, () => Session.Select(null));
        }

        private readonly struct DeleteAction
        {
            public static readonly DeleteAction None = new(Array.Empty<int2>(), LevelEditorFeedback.None, null);

            public readonly IReadOnlyList<int2> Positions;
            public readonly LevelEditorFeedback Feedback;
            public readonly Action Execute;

            public DeleteAction(IReadOnlyList<int2> positions, LevelEditorFeedback feedback, Action execute)
            {
                Positions = positions;
                Feedback = feedback;
                Execute = execute;
            }
        }
    }
}
