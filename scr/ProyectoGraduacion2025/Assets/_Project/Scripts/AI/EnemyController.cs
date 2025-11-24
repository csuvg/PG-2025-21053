using UnityEngine;
using UnityEngine.AI;
using Project.Core;
using Unity.AI.Navigation;
using Project.AI;

namespace Project.AI
{
    [RequireComponent(typeof(NavMeshAgent))]
    [RequireComponent(typeof(Collider))]
    public class EnemyController : MonoBehaviour
    {
        [Header("Configuración")]
        [SerializeField] private bool isStatic = false;

        private NavMeshAgent agent;
        private Transform target;
        private Vector3 startPosition;
        private bool isActive;

        private void Awake()
        {
            agent = GetComponent<NavMeshAgent>();
            startPosition = transform.position;
            GetComponent<Collider>().isTrigger = true;

            if (isStatic)
            {
                agent.enabled = false;
                Debug.Log($"[Enemy {name}] ESTÁTICO: Solo colisiones.");
            }
        }

        private void OnEnable()
        {
            EventBus.Subscribe<OnRestartLevelEvent>(OnRestart);
            EventBus.Subscribe<OnEpisodeRestartEvent>(OnResetEpisode);
            Debug.Log($"[Enemy {name}] Suscrito a eventos.");
        }

        private void OnDisable()
        {
            
            EventBus.Unsubscribe<OnRestartLevelEvent>(OnRestart);
            EventBus.Unsubscribe<OnEpisodeRestartEvent>(OnResetEpisode);  
            Debug.Log($"[Enemy {name}] Desuscrito de eventos.");
        }

        public void ActivateAI()
        {
            if (agent == null) 
            {
                Debug.LogError($"[Enemy {name}] NavMeshAgent NULL!");
                return;
            }

            gameObject.SetActive(true);  

            if (isStatic)
            {
                isActive = true;
                Debug.Log($"[Enemy {name}] Activado ESTÁTICO.");
                return;
            }

            
            var localSurface = GetComponentInParent<NavMeshSurface>();
            if (localSurface != null)
            {
                agent.agentTypeID = localSurface.agentTypeID;
                Debug.Log($"[Enemy {name}] NavMesh: {localSurface.name} (TypeID: {agent.agentTypeID})");
            }

            
            var parentEnv = transform.parent;
            if (parentEnv != null)
            {
                var localAgents = parentEnv.GetComponentsInChildren<AgentController>(true);
                if (localAgents.Length > 0)
                {
                    target = localAgents[0].transform;
                    Debug.Log($"[Enemy {name}] Target: {target.name}");
                }
                else
                {
                    Debug.LogWarning($"[Enemy {name}] NO Agent en {parentEnv.name}");
                }
            }

            agent.enabled = true;
            agent.Warp(startPosition);
            isActive = true;
            Debug.Log($"[Enemy {name}] ACTIVADO ✅ target={target != null}");
        }

        private void Update()
        {
            if (!isActive || isStatic || target == null) return;

            if (!agent.isOnNavMesh)
            {
                Debug.LogWarning($"[Enemy {name}] !isOnNavMesh → Warp");
                agent.Warp(startPosition);
                return;
            }

            agent.isStopped = false;
            agent.SetDestination(target.position);
        }

        private void OnRestart(OnRestartLevelEvent evt)
        {
            ResetEnemyState();
        }

        private void OnResetEpisode(OnEpisodeRestartEvent evt)
        {
            ResetEnemyState();
            ActivateAI();  
        }

        private void ResetEnemyState()
        {
            gameObject.SetActive(true);
            transform.position = startPosition;
            if (agent != null && !isStatic)
                agent.Warp(startPosition);
            isActive = false;
            target = null;
        }

        private void OnTriggerEnter(Collider other)
        {
            if (!isActive || !other.CompareTag("Agent")) return;

            var agentController = other.GetComponent<AgentController>();
            if (agentController?.HasWeapon ?? false)
            {
                agentController.OnEnemyKilledWithWeapon();
                Die();
                return;
            }

            var agentStats = other.GetComponent<AgentStats>();
            agentStats?.TakeDamage(1);
        }

        public void Die()
        {
            gameObject.SetActive(false);
        }
    }
}