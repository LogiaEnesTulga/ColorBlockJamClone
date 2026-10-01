using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using Newtonsoft.Json.Serialization;
using UnityEditor;
using UnityEngine;

namespace RollicGames.ColorBlockJamClone.Level.Editor
{
    public class LevelFileEntry
    {
        public string Path;
        public LevelFileData Data;

        public string FileName => System.IO.Path.GetFileName(Path);
    }

    public class LevelFileRepository
    {
        public const string LevelsDirectory = "Assets/Game/Levels";

        private readonly JsonSerializerSettings _serializerSettings = new()
        {
            Formatting = Formatting.Indented,
            ContractResolver = new DefaultContractResolver { NamingStrategy = new CamelCaseNamingStrategy() },
            Converters = { new StringEnumConverter() },
        };

        public string GetLevelPath(int levelId)
        {
            return $"{LevelsDirectory}/level_{levelId:00000}.json";
        }

        public IReadOnlyList<LevelFileEntry> GetAllLevels()
        {
            var entries = new List<LevelFileEntry>();
            if(!Directory.Exists(LevelsDirectory)) return entries;

            foreach(var filePath in Directory.GetFiles(LevelsDirectory, "*.json"))
            {
                var path = filePath.Replace('\\', '/');
                try
                {
                    entries.Add(new LevelFileEntry { Path = path, Data = Read(path) });
                }
                catch(Exception exception)
                {
                    Debug.LogWarning($"[Level Editor] Skipped unreadable level file '{path}': {exception.Message}");
                }
            }

            return entries.OrderBy(entry => entry.Data.LevelId).ToList();
        }

        public LevelFileEntry FindLevel(int levelId, string ignoredPath)
        {
            return GetAllLevels().FirstOrDefault(entry => entry.Data.LevelId == levelId && !IsSamePath(entry.Path, ignoredPath));
        }

        public bool Exists(string path)
        {
            return File.Exists(path);
        }

        public LevelFileData Read(string path)
        {
            return JsonConvert.DeserializeObject<LevelFileData>(File.ReadAllText(path), _serializerSettings);
        }

        public void Write(string path, LevelFileData data)
        {
            Directory.CreateDirectory(LevelsDirectory);
            File.WriteAllText(path, JsonConvert.SerializeObject(data, _serializerSettings));
            AssetDatabase.ImportAsset(path);
        }

        public void Delete(string path)
        {
            AssetDatabase.DeleteAsset(path);
        }

        public static bool IsSamePath(string a, string b)
        {
            if(a == null || b == null) return false;

            return string.Equals(a.Replace('\\', '/'), b.Replace('\\', '/'), StringComparison.OrdinalIgnoreCase);
        }
    }
}
