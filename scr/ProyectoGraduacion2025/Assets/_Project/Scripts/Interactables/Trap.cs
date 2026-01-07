using UnityEngine;
using Project.AI;

namespace Project.Interactables
{
    [RequireComponent(typeof(Collider))]
    public class Trap : MonoBehaviour
    {
        [SerializeField] private bool instantKill = true;
        [SerializeField] private int damage = 1;

        private void OnTriggerEnter(Collider other)
        {
            if (!other.CompareTag("Agent")) return;

            var agent = other.GetComponent<RLAgentAdapter>();
            var stats = other.GetComponent<AgentStats>();
            if (stats == null || agent == null) return;

            if (instantKill)
            {
                Debug.Log($"instantKill activado");
                stats.Die();
            }
            else
            {
                stats.TakeDamage(damage);
            }
        }
    }
}