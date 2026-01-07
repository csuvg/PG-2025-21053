using UnityEngine;
using Project.AI;

namespace Project.Interactables
{
    [RequireComponent(typeof(Collider))]
    public class ElectricField : MonoBehaviour
    {
        [Tooltip("Daño por contacto (por 'hit')")]
        public int damagePerHit = 1;

        private void OnTriggerEnter(Collider other)
        {
            if (!other.CompareTag("Agent")) return;

            var stats = other.GetComponent<AgentStats>();
            if (stats != null)
            {
                stats.TakeDamage(damagePerHit);
            }
        }

    }
}
