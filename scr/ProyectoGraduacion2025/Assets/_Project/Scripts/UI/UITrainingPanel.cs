using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;  
using Project.Core;
using Project.EditorRuntime;  

namespace Project.UI
{
    public class UITrainingPanel : MonoBehaviour
    {
        [SerializeField] private Button trainButton;    
        [SerializeField] private Button restartButton;  
        [SerializeField] private Button backButton;     
        [SerializeField] private Button startTrainingButton;

        private LevelEditorManager editor;  

        private void Start()
        {
            editor = FindAnyObjectByType<LevelEditorManager>();  

            trainButton?.onClick.AddListener(OnTrainPressed);
            restartButton?.onClick.AddListener(OnRestartPressed);
            backButton?.onClick.AddListener(OnBackPressed);
            startTrainingButton?.onClick.AddListener(OnStartTraining);
        }

        private void OnStartTraining()
        {
            EventBus.Publish(new OnStartTrainingEvent());
            Debug.Log("[UI] ¡Entrenamiento iniciado! Corre 'mlagents-learn' ahora.");
        }

        private void OnTrainPressed()
        {
            
            if (editor != null)
                editor.SavePrePlayState();  

            GameStateManager.Instance.StartPlayMode();
            Debug.Log("[UITrainingPanel] Training iniciado");
        }

        private void OnRestartPressed()
        {
            
            EventBus.Publish(new OnRestartLevelEvent());
            GameStateManager.Instance.EnterEditMode();  
            Debug.Log("[UITrainingPanel] Reiniciando nivel");
        }

        private void OnBackPressed()
        {
            
            SceneManager.LoadScene("LevelSelector");
            Debug.Log("[UITrainingPanel] Volviendo a LevelSelector");
        }
    }
}