using System;
using System.Collections.Generic;
using System.Linq;
using RollicGames.ColorBlockJamClone.Level.Runtime.Model;
using UnityEngine;
using UnityEngine.UIElements;

namespace RollicGames.ColorBlockJamClone.Level.Editor
{
    /// <summary>Start screen: create a new level or load a saved one.</summary>
    public class LevelEditorHomePanel : VisualElement
    {
        private const float DefaultDuration = 60f;
        private const int DefaultWidth = 5;
        private const int DefaultHeight = 6;

        public event Action LevelOpened;

        private readonly LevelFileRepository _repository;
        private readonly LevelDocumentService _documentService;
        private readonly LevelEditorColors _colors;

        private IntegerField _levelIdField;
        private FloatField _durationField;
        private IntegerField _widthField;
        private IntegerField _heightField;
        private LevelIdAvailabilityLabel _idAvailability;
        private ScrollView _levelList;
        private Label _levelCountLabel;
        private Label _errorLabel;

        public LevelEditorHomePanel(LevelFileRepository repository, LevelDocumentService documentService, LevelEditorColors colors)
        {
            _repository = repository;
            _documentService = documentService;
            _colors = colors;

            AddToClassList("le-home");

            var eyebrow = new Label("COLOR BLOCK JAM");
            eyebrow.AddToClassList("le-home__eyebrow");
            var title = new Label("Level Editor");
            title.AddToClassList("le-home__title");

            var cards = new VisualElement();
            cards.AddToClassList("le-home__cards");
            cards.Add(CreateNewLevelCard());
            cards.Add(CreateLoadLevelCard());

            _errorLabel = new Label();
            _errorLabel.AddToClassList("le-message");
            _errorLabel.AddToClassList("le-message--error");
            _errorLabel.AddToClassList("le-home__error");

            Add(eyebrow);
            Add(title);
            Add(cards);
            Add(_errorLabel);
        }

        public void Refresh()
        {
            var levels = _repository.GetAllLevels();
            _levelIdField.SetValueWithoutNotify(levels.Count == 0 ? 1 : levels.Max(level => level.Data.LevelId) + 1);
            RefreshIdAvailability();
            RebuildLevelList(levels);
            _errorLabel.style.display = DisplayStyle.None;
        }

        private VisualElement CreateNewLevelCard()
        {
            var card = CreateCard("Create New Level", "Start from a grid filled with cells.");

            _levelIdField = new IntegerField("Level ID");
            _levelIdField.AddToClassList("le-field");
            _levelIdField.RegisterValueChangedCallback(evt =>
            {
                _levelIdField.SetValueWithoutNotify(Mathf.Max(LevelInspectorPanel.MinLevelId, evt.newValue));
                RefreshIdAvailability();
            });

            _idAvailability = new LevelIdAvailabilityLabel();

            _durationField = new FloatField("Duration (sec)") { value = DefaultDuration };
            _durationField.AddToClassList("le-field");
            _durationField.RegisterValueChangedCallback(evt => _durationField.SetValueWithoutNotify(Mathf.Max(LevelInspectorPanel.MinDuration, evt.newValue)));

            var sizeRow = new VisualElement();
            sizeRow.AddToClassList("le-size-row");
            _widthField = CreateSizeField("Width", DefaultWidth);
            _heightField = CreateSizeField("Height", DefaultHeight);
            sizeRow.Add(_widthField);
            sizeRow.Add(_heightField);

            var spacer = new VisualElement();
            spacer.AddToClassList("le-flex-spacer");

            var createButton = LevelEditorElements.CreateButton("Create Level", CreateLevel, "le-button--primary");
            createButton.AddToClassList("le-button--large");

            card.Add(_levelIdField);
            card.Add(_idAvailability);
            card.Add(_durationField);
            card.Add(sizeRow);
            card.Add(spacer);
            card.Add(createButton);
            return card;
        }

        private VisualElement CreateLoadLevelCard()
        {
            var card = CreateCard("Load Level", $"Saved levels in {LevelFileRepository.LevelsDirectory}");

            var listHeader = new VisualElement();
            listHeader.AddToClassList("le-list-header");
            _levelCountLabel = new Label();
            _levelCountLabel.AddToClassList("le-list-header__count");
            var refreshButton = LevelEditorElements.CreateButton("Refresh", Refresh, "le-button--small");
            listHeader.Add(_levelCountLabel);
            listHeader.Add(refreshButton);

            _levelList = new ScrollView(ScrollViewMode.Vertical);
            _levelList.AddToClassList("le-level-list");

            card.Add(listHeader);
            card.Add(_levelList);
            return card;
        }

        private static VisualElement CreateCard(string title, string subtitle)
        {
            var card = new VisualElement();
            card.AddToClassList("le-card");

            var titleLabel = new Label(title);
            titleLabel.AddToClassList("le-card__title");
            var subtitleLabel = new Label(subtitle);
            subtitleLabel.AddToClassList("le-card__subtitle");

            card.Add(titleLabel);
            card.Add(subtitleLabel);
            return card;
        }

        private static IntegerField CreateSizeField(string label, int defaultValue)
        {
            var field = new IntegerField(label) { value = defaultValue };
            field.AddToClassList("le-field");
            field.AddToClassList("le-field--stacked");
            field.RegisterValueChangedCallback(evt =>
                field.SetValueWithoutNotify(Mathf.Clamp(evt.newValue, LevelInspectorPanel.MinGridSize, LevelInspectorPanel.MaxGridSize)));
            return field;
        }

        private void RefreshIdAvailability()
        {
            _idAvailability.Refresh(_repository.FindLevel(_levelIdField.value, null));
        }

        private void RebuildLevelList(IReadOnlyList<LevelFileEntry> levels)
        {
            _levelList.Clear();
            _levelCountLabel.text = levels.Count == 1 ? "1 level" : $"{levels.Count} levels";

            if(levels.Count == 0)
            {
                var empty = new Label("No saved levels yet. Create one to get started.");
                empty.AddToClassList("le-hint");
                _levelList.Add(empty);
                return;
            }

            foreach(var level in levels)
            {
                _levelList.Add(CreateLevelItem(level));
            }
        }

        private VisualElement CreateLevelItem(LevelFileEntry entry)
        {
            var data = entry.Data;
            var item = new VisualElement();
            item.AddToClassList("le-level-item");
            item.AddManipulator(new Clickable(() => LoadLevel(entry)));

            var previewFrame = new VisualElement();
            previewFrame.AddToClassList("le-level-item__preview");
            previewFrame.Add(CreatePreview(data));

            var info = new VisualElement();
            info.AddToClassList("le-level-item__info");
            var title = new Label($"Level {data.LevelId}");
            title.AddToClassList("le-level-item__title");
            var fileName = new Label(entry.FileName);
            fileName.AddToClassList("le-level-item__file");
            var meta = new Label($"{data.Grid.Width} x {data.Grid.Height}   ·   {data.Blocks.Count} blocks   ·   {data.Doors.Count} doors   ·   {data.Duration:0.#}s");
            meta.AddToClassList("le-level-item__meta");
            info.Add(title);
            info.Add(fileName);
            info.Add(meta);

            var chevron = new Label("›");
            chevron.AddToClassList("le-level-item__chevron");

            item.Add(previewFrame);
            item.Add(info);
            item.Add(chevron);
            return item;
        }

        private VisualElement CreatePreview(LevelFileData data)
        {
            try
            {
                var grid = new LevelGridModel();
                LevelFileMapper.FillGrid(grid, data);
                var preview = new LevelGridPreview(grid, _colors);
                preview.AddToClassList("le-level-item__preview-canvas");
                return preview;
            }
            catch(Exception)
            {
                var invalid = new Label("?");
                invalid.AddToClassList("le-level-item__preview-invalid");
                return invalid;
            }
        }

        private void CreateLevel()
        {
            _documentService.CreateNew(_levelIdField.value, _durationField.value, _widthField.value, _heightField.value);
            LevelOpened?.Invoke();
        }

        private void LoadLevel(LevelFileEntry entry)
        {
            try
            {
                _documentService.Load(entry.Path);
                LevelOpened?.Invoke();
            }
            catch(Exception exception)
            {
                _errorLabel.text = $"Couldn't load {entry.FileName}: {exception.Message}";
                _errorLabel.style.display = DisplayStyle.Flex;
            }
        }
    }
}
