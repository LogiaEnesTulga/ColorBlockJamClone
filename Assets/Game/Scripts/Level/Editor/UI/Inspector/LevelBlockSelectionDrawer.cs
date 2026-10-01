using RollicGames.ColorBlockJamClone.Level.Runtime.Model;
using UnityEngine.UIElements;

namespace RollicGames.ColorBlockJamClone.Level.Editor
{
    public class LevelBlockSelectionDrawer : ILevelSelectionDrawer
    {
        private readonly LevelEditorSession _session;
        private readonly LevelBlockEditor _blockEditor;
        private readonly LevelEditorColors _colors;

        public LevelObjectType ObjectType => LevelObjectType.Block;

        public LevelBlockSelectionDrawer(LevelEditorSession session, LevelBlockEditor blockEditor, LevelEditorColors colors)
        {
            _session = session;
            _blockEditor = blockEditor;
            _colors = colors;
        }

        public void Draw(VisualElement container, LevelObjectModel levelObject)
        {
            var block = (LevelBlockObjectModel)levelObject;

            container.Add(LevelEditorElements.CreateObjectHeader("Block", block.Id, _colors.GetObjectColor(block.Color)));
            container.Add(LevelEditorElements.CreateInfoRow("Pieces", block.BlocksLocalPositions.Count.ToString()));
            container.Add(LevelEditorElements.CreateInfoRow("Position", block.GridPosition.ToString()));
            container.Add(LevelEditorElements.CreateColorPalette(_colors, block.Color, color =>
            {
                _blockEditor.SetColor(block, color);
                _session.MarkChanged();
            }));
        }
    }
}
