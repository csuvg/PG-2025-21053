using Unity.MLAgents;
using UnityEditor;
using UnityEngine;

namespace Project.Core
{
    public class GameStateManager : MonoBehaviour
    {
        public static GameStateManager Instance { get; private set; }
        public GameState CurrentState { get; private set; } = GameState.EditMode;

        private void Start()
        {
            EnterEditMode();
        }

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

        public void ChangeState(GameState newState)
        {
            if (CurrentState == newState) return;

            var oldState = CurrentState;
            CurrentState = newState;

            EventBus.Publish(new OnGameStateChangedEvent
            {
                OldState = oldState,
                NewState = newState
            });

            EventBus.Publish(new OnDisplayMessageEvent
            {
                Message = $"Estado del juego: {newState}"
            });

            Debug.Log($"[GameStateManager] Cambio de estado: {oldState} → {newState}");

            switch (newState)
            {
                case GameState.EditMode:
                    Time.timeScale = 0f;
                    break;

                case GameState.PlayMode:
                    Time.timeScale = 1f;
                    EventBus.Publish(new OnPlayModeStartedEvent { seed = Random.Range(0, 10000) });
                    break;

                case GameState.Paused:
                    Time.timeScale = 0f;
                    EventBus.Publish(new OnGamePausedEvent());
                    break;

                case GameState.Results:
                    Time.timeScale = 0f;
                    EventBus.Publish(new OnLevelCompletedEvent());
                    break;

                case GameState.GameOver:
                    Time.timeScale = 0f;
                    EventBus.Publish(new OnAgentDiedEvent());
                    break;
            }
        }

        public void EnterEditMode() => ChangeState(GameState.EditMode);
        public void StartPlayMode() => ChangeState(GameState.PlayMode);
        public void PauseGame() => ChangeState(GameState.Paused);
        public void ResumeGame() => ChangeState(GameState.PlayMode);
        public void ShowResults() => ChangeState(GameState.Results);
        public void EndGame() => ChangeState(GameState.GameOver);
    }
}
