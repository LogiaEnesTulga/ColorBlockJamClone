using System;
using System.Collections.Generic;
using UnityEngine.UIElements;

namespace RollicGames.ColorBlockJamClone.Level.Editor
{
    public class LevelEditorToolbar : VisualElement
    {
        public event Action<ILevelEditorTool> ToolSelected;

        private readonly Dictionary<ILevelEditorTool, Button> _buttons = new();

        public LevelEditorToolbar(IReadOnlyList<ILevelEditorTool> tools)
        {
            AddToClassList("le-toolbar");

            foreach(var tool in tools)
            {
                var button = new Button(() => ToolSelected?.Invoke(tool)) { tooltip = tool.DisplayName };
                button.AddToClassList("le-tool");

                var icon = new ToolIcon(tool);
                var label = new Label(tool.DisplayName);
                label.AddToClassList("le-tool__label");

                button.Add(icon);
                button.Add(label);
                button.RegisterCallback<PointerEnterEvent>(_ => icon.MarkDirtyRepaint());
                button.RegisterCallback<PointerLeaveEvent>(_ => icon.MarkDirtyRepaint());

                _buttons.Add(tool, button);
                Add(button);
            }
        }

        public void SetActiveTool(ILevelEditorTool activeTool)
        {
            foreach(var pair in _buttons)
            {
                pair.Value.EnableInClassList("le-tool--active", pair.Key == activeTool);
                pair.Value.Q<ToolIcon>().MarkDirtyRepaint();
            }
        }

        /// <summary>Draws the tool's vector icon in the button's current text color, so hover and active states tint it.</summary>
        private class ToolIcon : VisualElement
        {
            private readonly ILevelEditorTool _tool;

            public ToolIcon(ILevelEditorTool tool)
            {
                _tool = tool;
                AddToClassList("le-tool__icon");
                pickingMode = PickingMode.Ignore;
                generateVisualContent += context => _tool.DrawIcon(context.painter2D, contentRect, resolvedStyle.color);
            }
        }
    }
}
