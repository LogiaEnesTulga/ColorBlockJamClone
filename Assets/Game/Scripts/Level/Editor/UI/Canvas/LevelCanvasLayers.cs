using System.Collections.Generic;

namespace RollicGames.ColorBlockJamClone.Level.Editor
{
    public static class LevelCanvasLayers
    {
        public static IReadOnlyList<ILevelCanvasLayer> CreateEditorLayers()
        {
            return new ILevelCanvasLayer[]
            {
                new LevelBoardLayer(true),
                new LevelCellLayer(),
                new LevelDoorLayer(),
                new LevelBlockLayer(),
                new LevelDraftLayer(),
                new LevelSelectionLayer(),
                new LevelHoverLayer(),
                new LevelCursorLayer(),
            };
        }

        public static IReadOnlyList<ILevelCanvasLayer> CreatePreviewLayers()
        {
            return new ILevelCanvasLayer[]
            {
                new LevelBoardLayer(false),
                new LevelCellLayer(),
                new LevelDoorLayer(),
                new LevelBlockLayer(),
            };
        }
    }
}
