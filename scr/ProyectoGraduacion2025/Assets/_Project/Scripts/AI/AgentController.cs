using UnityEngine;
using Project.Core;
using Project.Interactables;

namespace Project.AI
{
    [RequireComponent(typeof(Collider))]
    public class AgentController : MonoBehaviour
    {
        [Header("Arma")]
        public bool HasWeapon { get; private set; } = false;  
        public string CurrentWeaponId { get; private set; }

        [Tooltip("Recompensa por matar enemigo con arma.")]
        [SerializeField] private float killEnemyReward = 1f;  

        private AgentStats stats;
        private RLAgentAdapter rlAgent;

        private void Awake()
        {
            stats = GetComponent<AgentStats>();
            rlAgent = GetComponent<RLAgentAdapter>();
        }

        public void GiveWeapon(string weaponId)
        {
            HasWeapon = true;
            CurrentWeaponId = weaponId;

            EventBus.Publish(new OnDisplayMessageEvent
            {
                Message = $"Agente ha obtenido arma: {weaponId}"
            });
        }

        
        public void OnEnemyKilledWithWeapon()
        {
            if (rlAgent != null)
                rlAgent.AddCustomReward(killEnemyReward);  
        }
    }
}