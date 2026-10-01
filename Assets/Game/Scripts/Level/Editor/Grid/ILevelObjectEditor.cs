using System.Collections.Generic;
using RollicGames.Math.Runtime.Model;
using RollicGames.ColorBlockJamClone.Level.Runtime.Model;

namespace RollicGames.ColorBlockJamClone.Level.Editor
{
    public interface ILevelObjectEditor
    {
        LevelObjectType ObjectType { get; }

        IReadOnlyList<int2> GetPositions(LevelObjectModel levelObject);
        LevelObjectColor GetColor(LevelObjectModel levelObject);
        void SetColor(LevelObjectModel levelObject, LevelObjectColor color);

        void Remove(LevelObjectModel levelObject);
        bool CanRemovePiece(LevelObjectModel levelObject, int2 position);
        void RemovePiece(LevelObjectModel levelObject, int2 position);

        bool CanMove(LevelObjectModel levelObject, int2 targetOrigin);
        void Move(LevelObjectModel levelObject, int2 targetOrigin);
    }
}
