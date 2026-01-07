using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;
using System.Collections;
using System.Collections.Generic;

namespace Project.UI
{
    public class UILevelSelectorDynamic : MonoBehaviour
    {
        [Header("Referencias UI")]
        [SerializeField] private Transform contentParent;
        [SerializeField] private GameObject levelButtonPrefab;
        [SerializeField] private TextMeshProUGUI titleText;

        private readonly List<Button> levelButtons = new();

        private void Start()
        {
            StartCoroutine(GenerateButtonsDelayed());  
        }

        private IEnumerator GenerateButtonsDelayed()
        {
            yield return null;  
            GenerateLevelButtons();
        }

        private void GenerateLevelButtons()
        {
            if (levelButtonPrefab == null || contentParent == null)
            {
                Debug.LogError("[UILevelSelectorDynamic] Falta prefab/contenedor.");
                return;
            }

            
            foreach (var btn in levelButtons)
                if (btn != null) Destroy(btn.gameObject);
            levelButtons.Clear();

            int totalScenes = SceneManager.sceneCountInBuildSettings;
            Debug.Log($"[UILevelSelectorDynamic] Total escenas: {totalScenes}");

            int buttonCount = 0;
            string currentScene = SceneManager.GetActiveScene().name;

            for (int i = 0; i < totalScenes; i++)
            {
                string scenePath = SceneUtility.GetScenePathByBuildIndex(i);
                string sceneName = System.IO.Path.GetFileNameWithoutExtension(scenePath);

                if (sceneName == currentScene) continue;

                var buttonGO = Instantiate(levelButtonPrefab, contentParent);
                buttonGO.name = sceneName;  

                var label = buttonGO.GetComponentInChildren<TextMeshProUGUI>();
                string displayName = sceneName.StartsWith("Level_") ? 
                    sceneName.Replace("Level_", "Nivel ") : sceneName;
                label.text = displayName;

                var btn = buttonGO.GetComponent<Button>();
                string sceneToLoad = sceneName;
                btn.onClick.AddListener(() => LoadLevel(sceneToLoad));

                levelButtons.Add(btn);
                buttonCount++;
            }

            
            LayoutRebuilder.ForceRebuildLayoutImmediate(contentParent as RectTransform);
            Canvas.ForceUpdateCanvases();  

            if (titleText != null)
                titleText.text = $"Selecciona un Nivel ({buttonCount})";

            Debug.Log($"[UILevelSelectorDynamic] {buttonCount} botones generados y layout rebuilt.");
        }

        private void LoadLevel(string sceneName)
        {
            Debug.Log($"Cargando: {sceneName}");
            SceneManager.LoadScene(sceneName);
        }
    }
}