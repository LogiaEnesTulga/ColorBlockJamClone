using System;
using RollicGames.Math.Runtime.Model;
using RollicGames.ColorBlockJamClone.Level.Runtime.Model;
using UnityEngine;
using UnityEngine.UIElements;

namespace RollicGames.ColorBlockJamClone.Level.Editor
{
    public abstract class LevelEditorToolBase : ILevelEditorTool
    {
        public event Action StateChanged;

        protected readonly LevelEditorSession Session;
        protected LevelGridModel Grid => Session.Grid;

        public abstract string DisplayName { get; }
        public abstract string Hint { get; }

        protected LevelEditorToolBase(LevelEditorSession session)
        {
            Session = session;
        }

        public abstract void DrawIcon(Painter2D painter, Rect rect, Color color);
        public abstract LevelEditorHover GetHover(int2 position);

        public virtual VisualElement CreateOptions() => null;
        public virtual void Activate() {}
        public virtual void Deactivate() {}
        public virtual void OnPointerDown(int2 position) {}
        public virtual void OnPointerDrag(int2 position) {}
        public virtual void OnPointerUp(int2 position) {}
        public virtual bool OnKeyDown(KeyCode keyCode) => false;

        protected void NotifyStateChanged()
        {
            StateChanged?.Invoke();
        }
    }
}
