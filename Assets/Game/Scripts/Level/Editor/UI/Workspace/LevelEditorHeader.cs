using System;
using System.IO;
using UnityEngine.UIElements;

namespace RollicGames.ColorBlockJamClone.Level.Editor
{
    public class LevelEditorHeader : VisualElement
    {
        public event Action CloseRequested;

        private readonly LevelEditorSession _session;
        private readonly Label _titleLabel;
        private readonly Label _fileLabel;
        private readonly Label _dirtyBadge;

        public LevelEditorHeader(LevelEditorSession session)
        {
            _session = session;
            AddToClassList("le-header");

            var backButton = LevelEditorElements.CreateButton("‹  Levels", () => CloseRequested?.Invoke(), "le-button--ghost");
            backButton.tooltip = "Back to create / load";

            var titles = new VisualElement();
            titles.AddToClassList("le-header__titles");
            _titleLabel = new Label();
            _titleLabel.AddToClassList("le-header__title");
            _fileLabel = new Label();
            _fileLabel.AddToClassList("le-header__file");
            titles.Add(_titleLabel);
            titles.Add(_fileLabel);

            _dirtyBadge = new Label("Unsaved changes");
            _dirtyBadge.AddToClassList("le-badge");

            Add(backButton);
            Add(titles);
            Add(_dirtyBadge);

            _session.Changed += Refresh;
        }

        private void Refresh()
        {
            if(!_session.IsOpen) return;

            _titleLabel.text = $"Level {_session.LevelId}";
            _fileLabel.text = _session.IsSavedToFile ? Path.GetFileName(_session.FilePath) : "New level, not saved yet";
            _dirtyBadge.style.display = _session.IsDirty ? DisplayStyle.Flex : DisplayStyle.None;
        }
    }
}
