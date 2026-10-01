using System;
using System.Collections.Generic;
using Newtonsoft.Json.Linq;
using RollicGames.Math.Runtime.Model;
using RollicGames.Pooling.Runtime.Controller;
using RollicGames.ColorBlockJamClone.Level.Runtime.Model;
using RollicGames.ColorBlockJamClone.Level.Runtime.Presenter;
using Zenject;

namespace RollicGames.ColorBlockJamClone.Level.Runtime.Controller
{
    public interface ILevelLoadController
    {
        void PrepareForReuse();
        void LoadLevel(int playerLevel);
    }

    public class LevelLoadController : ILevelLoadController
    {
        [Inject] private readonly LevelModel _levelModel;
        [Inject] private readonly LevelGridModel _gridModel;
        [Inject] private readonly ILevelsProvider _levelsProvider;
        [Inject] private readonly IObjectPool<LevelBlockObjectModel> _blockPool;
        [Inject] private readonly IObjectPool<LevelDoorObjectModel> _doorPool;

        public void PrepareForReuse()
        {
            _doorPool.PoolAll();
            _blockPool.PoolAll();
        }

        public void LoadLevel(int playerLevel)
        {
            var levelIndex = (playerLevel - 1) % _levelsProvider.LevelCount;
            var levelJson = JObject.Parse(_levelsProvider.GetLevel(levelIndex));

            _levelModel.RemainingDuration = levelJson.Value<float>("duration");

            var gridJson = levelJson["grid"];
            _gridModel.Width = gridJson.Value<int>("width");
            _gridModel.Height = gridJson.Value<int>("height");

            foreach(var cellJson in gridJson["cells"])
            {
                _gridModel.Cells.Add(ParsePosition(cellJson));
            }

            foreach(var blockJson in levelJson["blocks"])
            {
                var localPositions = new List<int2>();
                foreach(var localPositionJson in blockJson["localPositions"])
                {
                    localPositions.Add(ParsePosition(localPositionJson));
                }

                // Model ids come from the pool, so the ids in the json are not used.
                var block = _blockPool.Spawn();
                block.Initialize(ParsePosition(blockJson["position"]), ParseEnum<LevelObjectColor>(blockJson["color"]), localPositions);
                AddBlockToGrid(block);
            }

            foreach(var doorJson in levelJson["doors"])
            {
                var door = _doorPool.Spawn();
                door.Initialize(ParsePosition(doorJson["position"]), doorJson.Value<int>("length"),
                    ParseEnum<LevelObjectColor>(doorJson["color"]), ParseEnum<LevelDirection>(doorJson["absorbDirection"]));
                AddDoorToGrid(door);
            }
        }

        private void AddBlockToGrid(LevelBlockObjectModel blockObject)
        {
            _gridModel.Blocks.AddObjectWithId(blockObject);

            var localPositions = blockObject.BlocksLocalPositions;
            for(var i = 0; i < localPositions.Count; i++)
            {
                var activePosition = blockObject.GridPosition + localPositions[i];
                _gridModel.Blocks.ObjectsByPosition.Add(activePosition, blockObject);
            }
        }

        private void AddDoorToGrid(LevelDoorObjectModel doorObject)
        {
            _gridModel.Doors.AddObjectWithId(doorObject);

            var isHorizontal = doorObject.AbsorbDirection is LevelDirection.Up or LevelDirection.Down;
            var direction = isHorizontal ? int2.Right : int2.Down;
            for(var i = 0; i < doorObject.Length; i++)
            {
                var activePosition = doorObject.GridPosition + (direction * i);
                _gridModel.Doors.ObjectsByPosition.Add(activePosition, doorObject);
            }
        }

        private static int2 ParsePosition(JToken positionJson)
        {
            return new int2(positionJson.Value<int>("x"), positionJson.Value<int>("y"));
        }

        private static T ParseEnum<T>(JToken enumJson) where T : struct, Enum
        {
            return Enum.Parse<T>((string)enumJson);
        }
    }
}
