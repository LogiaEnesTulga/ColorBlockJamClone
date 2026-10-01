using RollicGames.ColorBlockJamClone.Level.Runtime.Model;
using UnityEngine.UIElements;

namespace RollicGames.ColorBlockJamClone.Level.Editor
{
    /// <summary>Draws the inspector content for one type of selected object.</summary>
    public interface ILevelSelectionDrawer
    {
        LevelObjectType ObjectType { get; }
        void Draw(VisualElement container, LevelObjectModel levelObject);
    }
}
