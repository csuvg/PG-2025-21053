using UnityEngine;
using Project.AI;  

namespace Project.Interactables
{
    [RequireComponent(typeof(Collider))]  
    public class StickyBlock : MonoBehaviour
    {
        [Header("Configuración")]
        [Tooltip("Factor de ralentización (0.5f = mitad velocidad)")]
        [SerializeField, Range(0.1f, 1f)] private float speedMultiplier = 0.5f;

        [Tooltip("Penalización por segundo de contacto (e.g., -0.01f)")]
        [SerializeField] private float contactPenaltyPerSecond = -0.01f;

        private void Awake()
        {
            
            var collider = GetComponent<Collider>();
            collider.isTrigger = true;
        }

        private void OnTriggerEnter(Collider other)
        {
            if (!other.CompareTag("Agent")) return;

            var agent = other.GetComponent<RLAgentAdapter>();
            if (agent != null)
            {
                agent.ApplySlow(speedMultiplier); 
                Debug.Log($"[StickyBlock] Agente ralentizado a {speedMultiplier}x");
            }
        }

        private void OnTriggerStay(Collider other)
        {
            if (!other.CompareTag("Agent")) return;

            var agent = other.GetComponent<RLAgentAdapter>();
            if (agent != null)
            {
                
                agent.AddCustomReward(contactPenaltyPerSecond * Time.deltaTime);
            }
        }

        private void OnTriggerExit(Collider other)
        {
            if (!other.CompareTag("Agent")) return;

            var agent = other.GetComponent<RLAgentAdapter>();
            if (agent != null)
            {
                agent.RemoveSlow();  
                Debug.Log("[StickyBlock] Velocidad del agente restaurada");
            }
        }
    }
}