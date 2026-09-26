using System;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEngine;

namespace RollicGames.Persistence.Runtime.Presenter
{
    public interface IPersistence
    {
        void SaveData<T>(string key, T data);
        T LoadData<T>(string key) where T : class;
    }

    public class BinaryPlayerPrefsPersistence : IPersistence
    {
        private readonly BinaryFormatter _formatter = new();
        private readonly MemoryStream _stream = new();

        public void SaveData<T>(string key, T data)
        {
            _stream.SetLength(0);
            _formatter.Serialize(_stream, data);

            PlayerPrefs.SetString(key, Convert.ToBase64String(_stream.GetBuffer(), 0, (int)_stream.Length));
            PlayerPrefs.Save();
        }

        public T LoadData<T>(string key) where T : class
        {
            if (!PlayerPrefs.HasKey(key))
            {
                return null;
            }

            var bytes = Convert.FromBase64String(PlayerPrefs.GetString(key));

            _stream.SetLength(0);
            _stream.Write(bytes, 0, bytes.Length);
            _stream.Position = 0;

            return (T)_formatter.Deserialize(_stream);
        }
    }
}
