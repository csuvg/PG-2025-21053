using UnityEngine;

namespace Project.Core
{
    // =============================
    // ENUMERACIONES
    // =============================

    public enum GameState
    {
        None,
        MainMenu,
        EditMode,
        PlayMode,
        Paused,
        Results,
        GameOver
    }

    // =============================
    // EVENTOS DE ESTADO GLOBAL
    // =============================

    public struct OnGameStateChangedEvent
    {
        public GameState OldState;
        public GameState NewState;
    }

    
    public struct OnGamePausedEvent { }

    
    public struct OnGameResumedEvent { }

    
    public struct OnPlayModeStartedEvent
    {
        public int seed;
    }

    
    public struct OnGoalReachedEvent { }

    
    public struct OnAgentDiedEvent { }

    
    public struct OnLevelCompletedEvent { }

    
    public struct OnDisplayMessageEvent
    {
        public string Message;
    }

    // =============================
    // EVENTOS DEL EDITOR
    // =============================

    public struct OnObjectPlacedEvent
    {
        public string objectId;
        public Vector3Int gridPosition; 
    }

    public struct OnObjectRemovedEvent
    {
        public string objectId;
        public Vector3Int gridPosition;
    }

    // =============================
    // EVENTOS DE RECURSOS
    // =============================

    public struct OnResourcesChangedEvent
    {
        public int previousValue;
        public int newValue;
    }

    public struct OnBudgetChangedEvent  
    {
        public int previousBudget;
        public int newBudget;
    }

    public struct OnNavMeshBuiltEvent { }


    public class OnRestartLevelEvent { }

    public struct OnEditModeEnteredEvent { }

    public class OnDoorTriggeredEvent
    {
        public string targetId;
    }

    public struct OnEpisodeRestartEvent { }

    public struct OnStartTrainingEvent { }

    


}
