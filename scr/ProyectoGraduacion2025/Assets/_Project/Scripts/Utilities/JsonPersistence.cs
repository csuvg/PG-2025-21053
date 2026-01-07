using System.IO;
using UnityEngine;

namespace Project.Core
{
    public static class JsonPersistence
    {
        private static string GetPath(string fileName)
        {
            return Path.Combine(Application.persistentDataPath, $"{fileName}.json");
        }

        public static void Save<T>(T data, string fileName)
        {
            string json = JsonUtility.ToJson(data, true);
            File.WriteAllText(GetPath(fileName), json);
            Debug.Log($"[JsonPersistence] Guardado: {fileName}");
        }

        public static T Load<T>(string fileName)
        {
            string path = GetPath(fileName);
            if (!File.Exists(path))
            {
                Debug.LogWarning($"[JsonPersistence] Archivo no encontrado: {path}");
                return default;
            }

            string json = File.ReadAllText(path);
            return JsonUtility.FromJson<T>(json);
        }
    }
}
