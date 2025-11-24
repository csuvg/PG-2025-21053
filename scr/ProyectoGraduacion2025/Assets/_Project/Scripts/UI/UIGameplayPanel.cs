using UnityEngine;
using UnityEngine.UI;
using Project.Core;
using Project.EditorRuntime;
using UnityEngine.SceneManagement;

namespace Project.UI
{
    public class UIGameplayPanel : MonoBehaviour
    {
        [SerializeField] private Button playButton;
        [SerializeField] private Button pauseButton;
        [SerializeField] private Button editButton;
        [SerializeField] private Button restartButton;
        [SerializeField] private Button backButton;
        [SerializeField] private Button startTrainingButton;

        private LevelEditorManager editor;

        private void Start()
        {
            editor = FindAnyObjectByType<LevelEditorManager>();

            playButton?.onClick.AddListener(OnPlayPressed);
            pauseButton?.onClick.AddListener(OnPausePressed);
            editButton?.onClick.AddListener(OnEditPressed);
            restartButton?.onClick.AddListener(() =>
            {
                EventBus.Publish(new OnRestartLevelEvent());
                GameStateManager.Instance.EnterEditMode();
            });
            backButton?.onClick.AddListener(OnBackPressed);
            startTrainingButton?.onClick.AddListener(OnStartTraining);

        }

        private void OnStartTraining()
        {
            EventBus.Publish(new OnStartTrainingEvent());
            Debug.Log("[UI] ¡Entrenamiento iniciado! Corre 'mlagents-learn' ahora.");
        }

        private void OnPlayPressed()
        {
            if (editor == null)
                editor = FindAnyObjectByType<LevelEditorManager>();

            
            editor.SavePrePlayState();

            
            GameStateManager.Instance.StartPlayMode();
        }

        private void OnPausePressed()
        {
            GameStateManager.Instance.PauseGame();
        }

        private void OnEditPressed()
        {
            if (editor == null)
                editor = FindAnyObjectByType<LevelEditorManager>();

            
            editor.RestorePrePlayState(editor.GetCatalog());
            GameStateManager.Instance.EnterEditMode();
        }

        private void OnBackPressed()
        {
            
            SceneManager.LoadScene("LevelSelectorScene");  
            Debug.Log("[UIGameplayPanel] Volviendo a LevelSelector");
        }

    }
}
