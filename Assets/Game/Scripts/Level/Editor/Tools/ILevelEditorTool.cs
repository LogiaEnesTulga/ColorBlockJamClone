using System;
using System.Collections.Generic;
using RollicGames.Math.Runtime.Model;
using RollicGames.ColorBlockJamClone.Level.Runtime.Model;
using UnityEngine;
using UnityEngine.UIElements;

namespace RollicGames.ColorBlockJamClone.Level.Editor
{
    public enum LevelEditorFeedback
    {
        None,
        Neutral,
        Valid,
        Invalid,
    }

    public enum LevelEditorCursor
    {
        Default,
        Delete,
    }

    public readonly struct LevelEditorHover
    {
        public static readonly LevelEditorHover None = new(Array.Empty<int2>(), LevelEditorFeedback.None);

        public readonly IReadOnlyList<int2> Positions;
        public readonly LevelEditorFeedback Feedback;
        public readonly LevelEditorCursor Cursor;

        public LevelEditorHover(IReadOnlyList<int2> positions, LevelEditorFeedback feedback, LevelEditorCursor cursor = LevelEditorCursor.Default)
        {
            Positions = positions;
            Feedback = feedback;
            Cursor = cursor;
        }

        public static LevelEditorHover Single(int2 position, bool isValid)
        {
            return new LevelEditorHover(new[] { position }, isValid ? LevelEditorFeedback.Valid : LevelEditorFeedback.Invalid);
        }
    }

    public interface ILevelEditorTool
    {
        event Action StateChanged;

        string DisplayName { get; }
        string Hint { get; }

        void DrawIcon(Painter2D painter, Rect rect, Color color);
        VisualElement CreateOptions();

        void Activate();
        void Deactivate();

        LevelEditorHover GetHover(int2 position);
        void OnPointerDown(int2 position);
        void OnPointerDrag(int2 position);
        void OnPointerUp(int2 position);
        bool OnKeyDown(KeyCode keyCode);
    }

    /// <summary>Implemented by tools that build an object piece by piece before it is created.</summary>
    public interface ILevelEditorDraftProvider
    {
        IReadOnlyList<int2> DraftPositions { get; }
        LevelObjectColor DraftColor { get; }
    }
}
