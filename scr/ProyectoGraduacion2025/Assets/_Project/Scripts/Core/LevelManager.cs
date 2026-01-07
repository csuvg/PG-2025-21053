using UnityEngine;
using Project.Data;
using Project.EditorRuntime;
using Project.Core;
using System.IO;

namespace Project.Core
{
    public class LevelManager : MonoBehaviour
    {
        public static LevelManager Instance { get; private set; }

        [SerializeField] private LevelEditorManager editor;
        [SerializeField] private Transform objectParent;
        [SerializeField] private ObjectCatalogSO catalog;

        public LevelData CurrentLevel { get; private set; }

        private void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Destroy(gameObject);
                return;
            }
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }

        private void Start()
        {
            LoadLevel("DefaultLevel");
        }

        public void NewLevel(string name)
        {
            if (editor != null)
                editor.ClearAllObjects();

            CurrentLevel = new LevelData(name);
            Debug.Log($"[LevelManager] Nuevo nivel: {name}");
        }


        public void SaveCurrentLevel()
        {
            if (CurrentLevel == null)
            {
                Debug.LogWarning("[LevelManager] No hay nivel actual para guardar.");
                return;
            }

            CurrentLevel.Clear();
            foreach (var entry in editor.GetPlacedObjects())
            {
                CurrentLevel.AddObject(entry.ObjectId, entry.GridPosition, entry.Rotation);
            }

            JsonPersistence.Save(CurrentLevel, CurrentLevel.levelName);
        }

        public void LoadLevel(string name)
        {
            var loaded = JsonPersistence.Load<LevelData>(name);
            if (loaded == null)
            {
                Debug.LogWarning($"[LevelManager] Nivel {name} no encontrado, creando uno nuevo.");
                loaded = new LevelData(name);
            }

            CurrentLevel = loaded;

            if (editor != null)
            {
                editor.ClearAllObjects();

                editor.LoadLevel(CurrentLevel, catalog, objectParent);
            }
        }



        public void DeleteLevel(string name)
        {
            string path = Path.Combine(Application.persistentDataPath, $"{name}.json");

            if (File.Exists(path))
            {
                File.Delete(path);
                Debug.Log($"[LevelManager] Nivel eliminado: {name}");
            }
            else
            {
                Debug.LogWarning($"[LevelManager] No se encontró el nivel '{name}' para eliminar.");
            }

            if (CurrentLevel != null && CurrentLevel.levelName == name && editor != null)
            {
                editor.ClearAllObjects();
                CurrentLevel = null;
            }
        }


    }
}
