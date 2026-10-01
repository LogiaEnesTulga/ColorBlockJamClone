using RollicGames.Math.Runtime.Model;
using UnityEngine;
using UnityEngine.UIElements;

namespace RollicGames.ColorBlockJamClone.Level.Editor
{
    public class LevelCellTool : LevelEditorToolBase
    {
        private readonly LevelCellEditor _cellEditor;

        public override string DisplayName => "Cell";
        public override string Hint => "Click or drag over empty spots inside the grid to place cells. Spots with a door can't hold a cell.";

        public LevelCellTool(LevelEditorSession session, LevelCellEditor cellEditor) : base(session)
        {
            _cellEditor = cellEditor;
        }

        public override void DrawIcon(Painter2D painter, Rect rect, Color color)
        {
            LevelEditorIcons.DrawCell(painter, rect, color);
        }

        public override LevelEditorHover GetHover(int2 position)
        {
            if(!Grid.IsInsideDoorArea(position)) return LevelEditorHover.None;

            return LevelEditorHover.Single(position, _cellEditor.CanAdd(position));
        }

        public override void OnPointerDown(int2 position)
        {
            TryPlaceCell(position);
        }

        public override void OnPointerDrag(int2 position)
        {
            TryPlaceCell(position);
        }

        private void TryPlaceCell(int2 position)
        {
            if(!_cellEditor.CanAdd(position)) return;

            _cellEditor.Add(position);
            Session.MarkChanged();
        }
    }
}
