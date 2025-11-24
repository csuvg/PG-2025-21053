using UnityEngine;
using Project.AI;
using Project.Core;

namespace Project.Interactables
{
    [RequireComponent(typeof(Collider))]
    public class Potion : MonoBehaviour
    {
        [SerializeField] private int healAmount = 1;
        [SerializeField] private float healReward = 0.2f;

        private Vector3 startPosition;
        private Quaternion startRotation;

        private void Start()
        {
            startPosition = transform.position;
            startRotation = transform.rotation;

            EventBus.Subscribe<OnEpisodeRestartEvent>(OnReset);
        }

        private void OnDestroy()
        {
            EventBus.Unsubscribe<OnEpisodeRestartEvent>(OnReset);
        }

        private void OnReset(OnEpisodeRestartEvent evt)
        {
            gameObject.SetActive(true);
            transform.SetPositionAndRotation(startPosition, startRotation);
        }

        private void OnTriggerEnter(Collider other)
        {
            if (!other.CompareTag("Agent")) return;

            var stats = other.GetComponent<AgentStats>();
            var rlAgent = other.GetComponent<RLAgentAdapter>();
            if (stats == null || rlAgent == null) return;

            stats.Heal(healAmount);

            if (stats.CurrentHealth < stats.MaxHealth)
                rlAgent.AddCustomReward(healReward);

            gameObject.SetActive(false);  
        }
    }
}