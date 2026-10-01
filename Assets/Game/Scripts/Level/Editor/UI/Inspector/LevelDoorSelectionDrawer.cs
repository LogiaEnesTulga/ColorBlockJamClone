using System;
using System.Linq;
using RollicGames.ColorBlockJamClone.Level.Runtime.Model;
using UnityEngine.UIElements;

namespace RollicGames.ColorBlockJamClone.Level.Editor
{
    public class LevelDoorSelectionDrawer : ILevelSelectionDrawer
    {
        private readonly LevelEditorSession _session;
        private readonly LevelDoorEditor _doorEditor;
        private readonly LevelEditorColors _colors;

        public LevelObjectType ObjectType => LevelObjectType.Door;

        public LevelDoorSelectionDrawer(LevelEditorSession session, LevelDoorEditor doorEditor, LevelEditorColors colors)
        {
            _session = session;
            _doorEditor = doorEditor;
            _colors = colors;
        }

        public void Draw(VisualElement container, LevelObjectModel levelObject)
        {
            var door = (LevelDoorObjectModel)levelObject;

            container.Add(LevelEditorElements.CreateObjectHeader("Door", door.Id, _colors.GetObjectColor(door.Color)));
            container.Add(LevelEditorElements.CreateInfoRow("Length", $"{door.Length}  (auto)"));
            container.Add(LevelEditorElements.CreateInfoRow("Position", door.GridPosition.ToString()));
            container.Add(LevelEditorElements.CreateColorPalette(_colors, door.Color, color =>
            {
                _doorEditor.SetColor(door, color);
                _session.MarkChanged();
            }));

            var directions = _doorEditor.GetAvailableDirections(door).Select(direction => direction.ToString()).ToList();
            var directionField = new DropdownField("Absorb Direction", directions, directions.IndexOf(door.AbsorbDirection.ToString()));
            directionField.AddToClassList("le-field");
            directionField.RegisterValueChangedCallback(evt =>
            {
                _doorEditor.SetDirection(door, Enum.Parse<LevelDirection>(evt.newValue));
                _session.MarkChanged();
            });
            container.Add(directionField);
        }
    }
}
