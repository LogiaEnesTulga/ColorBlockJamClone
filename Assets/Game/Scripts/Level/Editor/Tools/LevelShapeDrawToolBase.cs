using System.Collections.Generic;
using RollicGames.Math.Runtime.Model;
using RollicGames.ColorBlockJamClone.Level.Runtime.Model;
using UnityEngine;
using UnityEngine.UIElements;

namespace RollicGames.ColorBlockJamClone.Level.Editor
{
    /// <summary>Base for tools that draw an object piece by piece until "Finish Object" is pressed.</summary>
    public abstract class LevelShapeDrawToolBase : LevelEditorToolBase, ILevelEditorDraftProvider
    {
        private const LevelObjectColor DefaultColor = LevelObjectColor.Blue;

        private readonly List<int2> _draftPositions = new();

        private Label _statusLabel;
        private Button _finishButton;
        private Button _cancelButton;

        public IReadOnlyList<int2> DraftPositions => _draftPositions;
        public LevelObjectColor DraftColor => DefaultColor;

        protected abstract string ObjectName { get; }

        protected LevelShapeDrawToolBase(LevelEditorSession session) : base(session) {}

        protected abstract bool CanDraw(int2 position, IReadOnlyList<int2> draftPositions);
        protected abstract LevelObjectModel CreateObject(IReadOnlyList<int2> draftPositions, LevelObjectColor color);

        public override VisualElement CreateOptions()
        {
            var container = new VisualElement();
            container.AddToClassList("le-options__group");

            _statusLabel = new Label();
            _statusLabel.AddToClassList("le-options__status");

            _cancelButton = new Button(CancelDraft) { text = "Cancel", tooltip = "Discard the object being drawn (Esc)" };
            _cancelButton.AddToClassList("le-button");
            _cancelButton.AddToClassList("le-button--ghost");

            _finishButton = new Button(FinishDraft) { text = "Finish Object", tooltip = "Create the object (Enter)" };
            _finishButton.AddToClassList("le-button");
            _finishButton.AddToClassList("le-button--primary");

            container.Add(_statusLabel);
            container.Add(_cancelButton);
            container.Add(_finishButton);
            RefreshOptions();
            return container;
        }

        public override void Deactivate()
        {
            _draftPositions.Clear();
        }

        public override LevelEditorHover GetHover(int2 position)
        {
            if(!Grid.IsInsideDoorArea(position)) return LevelEditorHover.None;

            return LevelEditorHover.Single(position, CanDraw(position, _draftPositions));
        }

        public override void OnPointerDown(int2 position)
        {
            TryDraw(position);
        }

        public override void OnPointerDrag(int2 position)
        {
            TryDraw(position);
        }

        public override bool OnKeyDown(KeyCode keyCode)
        {
            switch(keyCode)
            {
                case KeyCode.Return:
                case KeyCode.KeypadEnter:
                    FinishDraft();
                    return true;

                case KeyCode.Escape:
                    CancelDraft();
                    return true;

                default:
                    return false;
            }
        }

        private void TryDraw(int2 position)
        {
            if(!CanDraw(position, _draftPositions)) return;

            _draftPositions.Add(position);
            OnDraftChanged();
        }

        private void FinishDraft()
        {
            if(_draftPositions.Count == 0) return;

            var createdObject = CreateObject(_draftPositions, DefaultColor);
            _draftPositions.Clear();
            Session.MarkChanged();
            Session.Select(createdObject);
            OnDraftChanged();
        }

        private void CancelDraft()
        {
            if(_draftPositions.Count == 0) return;

            _draftPositions.Clear();
            OnDraftChanged();
        }

        private void OnDraftChanged()
        {
            RefreshOptions();
            NotifyStateChanged();
        }

        private void RefreshOptions()
        {
            if(_statusLabel == null) return;

            var count = _draftPositions.Count;
            _statusLabel.text = count == 0
                ? $"No {ObjectName.ToLowerInvariant()} in progress"
                : $"{count} {(count == 1 ? "piece" : "pieces")} placed";
            _finishButton.SetEnabled(count > 0);
            _cancelButton.SetEnabled(count > 0);
        }
    }
}
