using System.Collections.Generic;
using RollicGames.ColorBlockJamClone.Level.Runtime.Model;

namespace RollicGames.ColorBlockJamClone.Level.Editor
{
    public class LevelObjectEditors
    {
        private readonly Dictionary<LevelObjectType, ILevelObjectEditor> _editors = new();

        public LevelObjectEditors(IEnumerable<ILevelObjectEditor> editors)
        {
            foreach(var editor in editors)
            {
                _editors.Add(editor.ObjectType, editor);
            }
        }

        public ILevelObjectEditor For(LevelObjectModel levelObject)
        {
            return _editors[levelObject.ObjectType];
        }
    }
}
