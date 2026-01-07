using UnityEngine;
using Project.Core;
using Project.AI;

namespace Project.Interactables
{
    [RequireComponent(typeof(Collider))]
    public class Switch : MonoBehaviour
    {
        [Tooltip("Identificador único para vincular con puertas u otros objetos")]
        public string targetId;

        [Tooltip("Recompensa que gana el agente al activar este switch por primera vez")]
        public float activationReward = 0.7f;

        private bool wasActivatedThisEpisode = false;

        private void Start()
        {
            
            EventBus.Subscribe<OnRestartLevelEvent>(OnReset);
            EventBus.Subscribe<OnEpisodeRestartEvent>(OnReset);
            EventBus.Subscribe<OnAgentDiedEvent>(OnReset);
        }

        private void OnDestroy()
        {
            EventBus.Unsubscribe<OnRestartLevelEvent>(OnReset);
            EventBus.Unsubscribe<OnEpisodeRestartEvent>(OnReset);
            EventBus.Unsubscribe<OnAgentDiedEvent>(OnReset);
        }

        private void OnTriggerEnter(Collider other)
        {
            if (!other.CompareTag("Agent")) return;

            var agent = other.GetComponent<RLAgentAdapter>();
            if (agent == null) return;

            if (!wasActivatedThisEpisode)
            {
                wasActivatedThisEpisode = true;
                agent.AddCustomReward(activationReward); 

                
                EventBus.Publish(new OnDoorTriggeredEvent { targetId = targetId });
            }
        }

        private void OnReset(OnRestartLevelEvent e) => wasActivatedThisEpisode = false;
        private void OnReset(OnEpisodeRestartEvent e) => wasActivatedThisEpisode = false;
        private void OnReset(OnAgentDiedEvent e) => wasActivatedThisEpisode = false;
    }
}
