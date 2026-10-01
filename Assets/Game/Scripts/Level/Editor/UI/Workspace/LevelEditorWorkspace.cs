using System;
using System.Collections.Generic;
using RollicGames.Math.Runtime.Model;
using UnityEngine.UIElements;

namespace RollicGames.ColorBlockJamClone.Level.Editor
{
    /// <summary>Editing screen: header, toolbar, tool options, grid canvas, status bar and inspector.</summary>
    public class LevelEditorWorkspace : VisualElement
    {
        public event Action CloseRequested;

        private readonly LevelEditorSession _session;
        private readonly IReadOnlyList<ILevelEditorTool> _tools;
        private readonly LevelEditorToolbar _toolbar;
        private readonly LevelGridCanvas _canvas;
        private readonly LevelInspectorPanel _inspector;
        private readonly Label _optionsTitle;
        private readonly VisualElement _optionsContent;
        private readonly Label _positionLabel;
        private readonly Label _hintLabel;

        private ILevelEditorTool _activeTool;

        public LevelEditorWorkspace(LevelEditorSession session, IReadOnlyList<ILevelEditorTool> tools, LevelGridCanvas canvas, LevelInspectorPanel inspector)
        {
            _session = session;
            _tools = tools;
            _canvas = canvas;
            _inspector = inspector;

            AddToClassList("le-workspace");

            var header = new LevelEditorHeader(session);
            header.CloseRequested += () => CloseRequested?.Invoke();

            var body = new VisualElement();
            body.AddToClassList("le-body");

            _toolbar = new LevelEditorToolbar(tools);
            _toolbar.ToolSelected += SelectTool;

            var center = new VisualElement();
            center.AddToClassList("le-center");

            var optionsBar = new VisualElement();
            optionsBar.AddToClassList("le-options");
            _optionsTitle = new Label();
            _optionsTitle.AddToClassList("le-options__title");
            _optionsContent = new VisualElement();
            _optionsContent.AddToClassList("le-options__content");
            optionsBar.Add(_optionsTitle);
            optionsBar.Add(_optionsContent);

            var statusBar = new VisualElement();
            statusBar.AddToClassList("le-status");
            _positionLabel = new Label("-");
            _positionLabel.AddToClassList("le-status__position");
            _hintLabel = new Label();
            _hintLabel.AddToClassList("le-status__hint");
            statusBar.Add(_positionLabel);
            statusBar.Add(_hintLabel);

            center.Add(optionsBar);
            center.Add(_canvas);
            center.Add(statusBar);

            body.Add(_toolbar);
            body.Add(center);
            body.Add(_inspector);

            Add(header);
            Add(body);

            foreach(var tool in tools)
            {
                tool.StateChanged += OnToolStateChanged;
            }

            _canvas.HoveredPositionChanged += OnHoveredPositionChanged;
            _session.Changed += _canvas.MarkDirtyRepaint;
            _session.SelectionChanged += OnToolStateChanged;
        }

        public void Open()
        {
            _inspector.ClearMessage();
            SelectTool(_tools[0]);
        }

        public void Close()
        {
            _activeTool?.Deactivate();
            _activeTool = null;
        }

        private void SelectTool(ILevelEditorTool tool)
        {
            _activeTool?.Deactivate();
            _activeTool = tool;
            _activeTool.Activate();

            _toolbar.SetActiveTool(tool);
            _canvas.SetTool(tool);

            _optionsTitle.text = tool.DisplayName;
            _optionsContent.Clear();
            var options = tool.CreateOptions();
            if(options != null)
            {
                _optionsContent.Add(options);
            }

            _hintLabel.text = tool.Hint;
            _canvas.Focus();
        }

        private void OnToolStateChanged()
        {
            if(_activeTool != null)
            {
                _hintLabel.text = _activeTool.Hint;
            }

            _canvas.MarkDirtyRepaint();
        }

        private void OnHoveredPositionChanged(int2? position)
        {
            _positionLabel.text = position.HasValue ? $"x {position.Value.X}   y {position.Value.Y}" : "-";
        }
    }
}
