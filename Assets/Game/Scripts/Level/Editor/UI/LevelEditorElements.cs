using System;
using RollicGames.ColorBlockJamClone.Level.Runtime.Model;
using UnityEngine;
using UnityEngine.UIElements;

namespace RollicGames.ColorBlockJamClone.Level.Editor
{
    /// <summary>Small reusable UI pieces shared by the editor panels.</summary>
    public static class LevelEditorElements
    {
        public static VisualElement CreateSection(string title, VisualElement content)
        {
            var section = new VisualElement();
            section.AddToClassList("le-section");

            var header = new Label(title.ToUpperInvariant());
            header.AddToClassList("le-section__title");

            section.Add(header);
            section.Add(content);
            return section;
        }

        public static Button CreateButton(string text, Action onClick, string modifierClass = null)
        {
            var button = new Button(onClick) { text = text };
            button.AddToClassList("le-button");
            if(modifierClass != null)
            {
                button.AddToClassList(modifierClass);
            }

            return button;
        }

        public static VisualElement CreateInfoRow(string label, string value)
        {
            var row = new VisualElement();
            row.AddToClassList("le-info-row");

            var labelElement = new Label(label);
            labelElement.AddToClassList("le-info-row__label");

            var valueElement = new Label(value);
            valueElement.AddToClassList("le-info-row__value");

            row.Add(labelElement);
            row.Add(valueElement);
            return row;
        }

        public static VisualElement CreateObjectHeader(string typeName, int id, Color color)
        {
            var header = new VisualElement();
            header.AddToClassList("le-object-header");

            var chip = new VisualElement();
            chip.AddToClassList("le-object-header__chip");
            chip.style.backgroundColor = color;

            var title = new Label($"{typeName} #{id}");
            title.AddToClassList("le-object-header__title");

            header.Add(chip);
            header.Add(title);
            return header;
        }

        public static VisualElement CreateColorPalette(LevelEditorColors colors, LevelObjectColor selectedColor, Action<LevelObjectColor> onSelected)
        {
            var palette = new VisualElement();
            palette.AddToClassList("le-palette");

            var header = new VisualElement();
            header.AddToClassList("le-palette__header");
            var label = new Label("Color");
            label.AddToClassList("le-field-label");
            var selectedLabel = new Label(selectedColor.ToString());
            selectedLabel.AddToClassList("le-palette__value");
            header.Add(label);
            header.Add(selectedLabel);

            var swatches = new VisualElement();
            swatches.AddToClassList("le-palette__swatches");
            foreach(LevelObjectColor color in Enum.GetValues(typeof(LevelObjectColor)))
            {
                if(color == LevelObjectColor.None) continue;

                var swatch = new Button(() => onSelected(color)) { tooltip = color.ToString() };
                swatch.AddToClassList("le-swatch");
                swatch.EnableInClassList("le-swatch--selected", color == selectedColor);
                swatch.style.backgroundColor = colors.GetObjectColor(color);
                swatches.Add(swatch);
            }

            palette.Add(header);
            palette.Add(swatches);
            return palette;
        }
    }

    /// <summary>Shows whether a level id is free, with a green or red status dot.</summary>
    public class LevelIdAvailabilityLabel : VisualElement
    {
        private readonly VisualElement _dot;
        private readonly Label _label;

        public LevelIdAvailabilityLabel()
        {
            AddToClassList("le-availability");

            _dot = new VisualElement();
            _dot.AddToClassList("le-availability__dot");
            _label = new Label();
            _label.AddToClassList("le-availability__label");

            Add(_dot);
            Add(_label);
        }

        public void Refresh(LevelFileEntry conflict)
        {
            var isAvailable = conflict == null;
            EnableInClassList("le-availability--taken", !isAvailable);
            _label.text = isAvailable ? "ID is available" : $"ID is used by {conflict.FileName}";
        }
    }
}
