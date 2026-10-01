using System;
using RollicGames.ColorBlockJamClone.Level.Runtime.Model;

namespace RollicGames.ColorBlockJamClone.Level.Editor
{
    public class LevelEditorSession
    {
        public event Action Changed;
        public event Action SelectionChanged;

        public LevelGridModel Grid { get; } = new LevelGridModel();
        public int LevelId { get; private set; }
        public float Duration { get; private set; }
        public string FilePath { get; private set; }
        public bool IsOpen { get; private set; }
        public bool IsDirty { get; private set; }
        public LevelObjectModel Selected { get; private set; }

        public bool IsSavedToFile => FilePath != null;

        public void Open(int levelId, float duration, string filePath)
        {
            LevelId = levelId;
            Duration = duration;
            FilePath = filePath;
            IsOpen = true;
            IsDirty = filePath == null;
            Selected = null;

            Changed?.Invoke();
            SelectionChanged?.Invoke();
        }

        public void Close()
        {
            LevelGridRegistry.Clear(Grid);
            FilePath = null;
            IsOpen = false;
            IsDirty = false;
            Selected = null;

            Changed?.Invoke();
            SelectionChanged?.Invoke();
        }

        public void SetLevelId(int levelId)
        {
            if(LevelId == levelId) return;

            LevelId = levelId;
            MarkChanged();
        }

        public void SetDuration(float duration)
        {
            if(Duration.Equals(duration)) return;

            Duration = duration;
            MarkChanged();
        }

        public void Select(LevelObjectModel levelObject)
        {
            if(Selected == levelObject) return;

            Selected = levelObject;
            SelectionChanged?.Invoke();
        }

        public void MarkChanged()
        {
            IsDirty = true;
            if(Selected != null && !Grid.ContainsObject(Selected))
            {
                Select(null);
            }

            Changed?.Invoke();
        }

        public void MarkSaved(string filePath)
        {
            FilePath = filePath;
            IsDirty = false;
            Changed?.Invoke();
        }
    }
}
