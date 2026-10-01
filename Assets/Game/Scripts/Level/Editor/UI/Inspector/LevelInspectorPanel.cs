using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

namespace RollicGames.ColorBlockJamClone.Level.Editor
{
    public class LevelInspectorPanel : VisualElement
    {
        public const int MinLevelId = 1;
        public const float MinDuration = 1f;
        public const int MinGridSize = 1;
        public const int MaxGridSize = 50;

        public event Action<string> NotificationRequested;

        private readonly LevelEditorSession _session;
        private readonly LevelFileRepository _repository;
        private readonly LevelSaveService _saveService;
        private readonly LevelGridResizer _resizer;
        private readonly IReadOnlyList<ILevelSelectionDrawer> _selectionDrawers;

        private IntegerField _levelIdField;
        private LevelIdAvailabilityLabel _idAvailability;
        private FloatField _durationField;
        private IntegerField _widthField;
        private IntegerField _heightField;
        private Button _applySizeButton;
        private VisualElement _selectionContainer;
        private Label _cellCountLabel;
        private Label _blockCountLabel;
        private Label _doorCountLabel;
        private Label _messageLabel;
        private Button _saveAsNewButton;

        public LevelInspectorPanel(LevelEditorSession session, LevelFileRepository repository, LevelSaveService saveService,
            LevelGridResizer resizer, IReadOnlyList<ILevelSelectionDrawer> selectionDrawers)
        {
            _session = session;
            _repository = repository;
            _saveService = saveService;
            _resizer = resizer;
            _selectionDrawers = selectionDrawers;

            AddToClassList("le-inspector");

            var scrollView = new ScrollView(ScrollViewMode.Vertical);
            scrollView.AddToClassList("le-inspector__scroll");
            scrollView.Add(LevelEditorElements.CreateSection("Level", CreateLevelContent()));
            scrollView.Add(LevelEditorElements.CreateSection("Selection", _selectionContainer = new VisualElement()));
            scrollView.Add(LevelEditorElements.CreateSection("Overview", CreateOverviewContent()));
            Add(scrollView);
            Add(CreateFooter());

            _session.Changed += Refresh;
            _session.SelectionChanged += RefreshSelection;
        }

        public void ClearMessage()
        {
            _messageLabel.style.display = DisplayStyle.None;
        }

        private VisualElement CreateLevelContent()
        {
            var content = new VisualElement();

            _levelIdField = new IntegerField("Level ID") { isDelayed = true };
            _levelIdField.AddToClassList("le-field");
            _levelIdField.RegisterValueChangedCallback(evt =>
            {
                var levelId = Mathf.Max(MinLevelId, evt.newValue);
                _levelIdField.SetValueWithoutNotify(levelId);
                _session.SetLevelId(levelId);
            });

            _idAvailability = new LevelIdAvailabilityLabel();

            _durationField = new FloatField("Duration (sec)") { isDelayed = true };
            _durationField.AddToClassList("le-field");
            _durationField.RegisterValueChangedCallback(evt =>
            {
                var duration = Mathf.Max(MinDuration, evt.newValue);
                _durationField.SetValueWithoutNotify(duration);
                _session.SetDuration(duration);
            });

            var sizeRow = new VisualElement();
            sizeRow.AddToClassList("le-size-row");
            _widthField = CreateSizeField("Width");
            _heightField = CreateSizeField("Height");
            _applySizeButton = LevelEditorElements.CreateButton("Apply", ApplySize, "le-button--small");
            _applySizeButton.tooltip = "Resize the grid";
            sizeRow.Add(_widthField);
            sizeRow.Add(_heightField);
            sizeRow.Add(_applySizeButton);

            content.Add(_levelIdField);
            content.Add(_idAvailability);
            content.Add(_durationField);
            content.Add(sizeRow);
            return content;
        }

        private IntegerField CreateSizeField(string label)
        {
            var field = new IntegerField(label);
            field.AddToClassList("le-field");
            field.AddToClassList("le-field--stacked");
            field.RegisterValueChangedCallback(evt =>
            {
                field.SetValueWithoutNotify(Mathf.Clamp(evt.newValue, MinGridSize, MaxGridSize));
                RefreshApplySizeButton();
            });
            return field;
        }

        private VisualElement CreateOverviewContent()
        {
            var content = new VisualElement();
            content.AddToClassList("le-overview");
            content.Add(CreateStat("Cells", out _cellCountLabel));
            content.Add(CreateStat("Blocks", out _blockCountLabel));
            content.Add(CreateStat("Doors", out _doorCountLabel));
            return content;
        }

        private static VisualElement CreateStat(string title, out Label valueLabel)
        {
            var stat = new VisualElement();
            stat.AddToClassList("le-stat");

            valueLabel = new Label();
            valueLabel.AddToClassList("le-stat__value");
            var titleLabel = new Label(title);
            titleLabel.AddToClassList("le-stat__title");

            stat.Add(valueLabel);
            stat.Add(titleLabel);
            return stat;
        }

        private VisualElement CreateFooter()
        {
            var footer = new VisualElement();
            footer.AddToClassList("le-inspector__footer");

            _messageLabel = new Label();
            _messageLabel.AddToClassList("le-message");
            _messageLabel.style.display = DisplayStyle.None;

            var saveButton = LevelEditorElements.CreateButton("Save Level", () => HandleSaveResult(_saveService.Save(_session)), "le-button--primary");
            saveButton.AddToClassList("le-button--large");
            _saveAsNewButton = LevelEditorElements.CreateButton("Save As New", () => HandleSaveResult(_saveService.SaveAsNew(_session)));
            _saveAsNewButton.AddToClassList("le-button--large");

            footer.Add(_messageLabel);
            footer.Add(saveButton);
            footer.Add(_saveAsNewButton);
            return footer;
        }

        private void HandleSaveResult(LevelSaveResult result)
        {
            _messageLabel.text = result.Message;
            _messageLabel.style.display = DisplayStyle.Flex;
            _messageLabel.EnableInClassList("le-message--success", result.IsSuccess);
            _messageLabel.EnableInClassList("le-message--error", !result.IsSuccess);
            NotificationRequested?.Invoke(result.Message);
        }

        private void ApplySize()
        {
            var width = _widthField.value;
            var height = _heightField.value;
            if(_resizer.IsShrinking(width, height))
            {
                var confirmed = EditorUtility.DisplayDialog("Shrink grid?",
                    $"Resizing to {width} x {height} removes the cells, blocks and doors outside the new size.", "Resize", "Cancel");
                if(!confirmed) return;
            }

            _resizer.Resize(width, height);
            _session.MarkChanged();
        }

        private void Refresh()
        {
            if(!_session.IsOpen) return;

            _levelIdField.SetValueWithoutNotify(_session.LevelId);
            _idAvailability.Refresh(_repository.FindLevel(_session.LevelId, _session.FilePath));
            _durationField.SetValueWithoutNotify(_session.Duration);
            _widthField.SetValueWithoutNotify(_session.Grid.Width);
            _heightField.SetValueWithoutNotify(_session.Grid.Height);
            RefreshApplySizeButton();

            _cellCountLabel.text = _session.Grid.Cells.Count.ToString();
            _blockCountLabel.text = _session.Grid.Blocks.ObjectsById.Count.ToString();
            _doorCountLabel.text = _session.Grid.Doors.ObjectsById.Count.ToString();

            _saveAsNewButton.style.display = _session.IsSavedToFile ? DisplayStyle.Flex : DisplayStyle.None;
            RefreshSelection();
        }

        private void RefreshApplySizeButton()
        {
            var isChanged = _widthField.value != _session.Grid.Width || _heightField.value != _session.Grid.Height;
            _applySizeButton.SetEnabled(isChanged);
        }

        private void RefreshSelection()
        {
            _selectionContainer.Clear();

            var selected = _session.Selected;
            if(selected == null)
            {
                var hint = new Label("Nothing selected. Pick a block or door with the Select tool.");
                hint.AddToClassList("le-hint");
                _selectionContainer.Add(hint);
                return;
            }

            _selectionDrawers.First(drawer => drawer.ObjectType == selected.ObjectType).Draw(_selectionContainer, selected);
        }
    }
}
