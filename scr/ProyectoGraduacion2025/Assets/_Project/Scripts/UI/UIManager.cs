using UnityEngine;
using Project.Core;

namespace Project.UI
{
    public class UIManager : MonoBehaviour
    {
        [Header("Panels")]
        [SerializeField] private GameObject editorPanel;
        [SerializeField] private GameObject gameplayPanel;
        [SerializeField] private GameObject hudPanel;


        private void OnEnable()
        {
            EventBus.Subscribe<OnGameStateChangedEvent>(OnGameStateChanged);
        }

        private void OnDisable()
        {
            EventBus.Unsubscribe<OnGameStateChangedEvent>(OnGameStateChanged);
        }

        private void Start()
        {
            UpdateUI(GameStateManager.Instance.CurrentState);
        }

        private void OnGameStateChanged(OnGameStateChangedEvent evt)
        {
            UpdateUI(evt.NewState);
        }

        private void UpdateUI(GameState state)
        {
            bool inEditMode = state == GameState.EditMode;

            editorPanel?.SetActive(inEditMode);
            gameplayPanel?.SetActive(state == GameState.PlayMode || state == GameState.Paused || state == GameState.EditMode);
            hudPanel?.SetActive(state == GameState.PlayMode || state == GameState.Paused || state == GameState.EditMode);
        }

    }
}
