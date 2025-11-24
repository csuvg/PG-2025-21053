using UnityEngine;
using Project.Core;

namespace Project.Interactables
{
    [RequireComponent(typeof(Collider))]
    public class Door : MonoBehaviour
    {
        [Tooltip("Id que conecta esta puerta con un Switch.targetId")]
        public string listensToTargetId;

        private Vector3 startPosition;
        private Quaternion startRotation;
        private bool wasRemoved = false;

        private void Start()
        {
            startPosition = transform.position;
            startRotation = transform.rotation;

            
            EventBus.Subscribe<OnDoorTriggeredEvent>(OnDoorTriggered);
            EventBus.Subscribe<OnRestartLevelEvent>(OnLevelReset);
            EventBus.Subscribe<OnAgentDiedEvent>(OnLevelReset);
            EventBus.Subscribe<OnPlayModeStartedEvent>(OnLevelReset);
            EventBus.Subscribe<OnEpisodeRestartEvent>(OnLevelReset);
        }

        private void OnDestroy()
        {
            
            EventBus.Unsubscribe<OnDoorTriggeredEvent>(OnDoorTriggered);
            EventBus.Unsubscribe<OnRestartLevelEvent>(OnLevelReset);
            EventBus.Unsubscribe<OnAgentDiedEvent>(OnLevelReset);
            EventBus.Unsubscribe<OnPlayModeStartedEvent>(OnLevelReset);
            EventBus.Unsubscribe<OnEpisodeRestartEvent>(OnLevelReset);
        }

        
        private void OnDoorTriggered(OnDoorTriggeredEvent e)
        {
            if (string.IsNullOrEmpty(listensToTargetId)) return;
            if (e.targetId != listensToTargetId) return;

            gameObject.SetActive(false);
            wasRemoved = true;
        }

        
        private void OnLevelReset(OnRestartLevelEvent e) => ResetDoor();
        private void OnLevelReset(OnAgentDiedEvent e) => ResetDoor();
        private void OnLevelReset(OnPlayModeStartedEvent e) => ResetDoor();
        private void OnLevelReset(OnEpisodeRestartEvent e) => ResetDoor();

        
        private void ResetDoor()
        {
            if (!wasRemoved) return;

            transform.SetPositionAndRotation(startPosition, startRotation);
            gameObject.SetActive(true);
            wasRemoved = false;
        }
    }
}
