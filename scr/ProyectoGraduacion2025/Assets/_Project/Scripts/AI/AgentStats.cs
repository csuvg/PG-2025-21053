using UnityEngine;
using Project.Core;

namespace Project.AI
{
    public class AgentStats : MonoBehaviour
    {
        [Header("Atributos del Agente")]
        [SerializeField] private int maxHealth = 3;
        public int MaxHealth => maxHealth;

        public int CurrentHealth { get; private set; }

        private RLAgentAdapter agent;

        private void Awake()
        {
            CurrentHealth = maxHealth;
            agent = GetComponent<RLAgentAdapter>();
        }

        private void OnEnable()
        {
            EventBus.Subscribe<OnEpisodeRestartEvent>(OnEpisodeRestart);  
        }

        private void OnDisable()
        {
            EventBus.Unsubscribe<OnEpisodeRestartEvent>(OnEpisodeRestart); 
        }

        private void OnEpisodeRestart(OnEpisodeRestartEvent evt)
        {
            ResetHealth();
        }

        public void TakeDamage(int amount)
        {
            CurrentHealth -= amount;
            agent.AddReward(-0.5f);
            if (CurrentHealth <= 0)
            {
                CurrentHealth = 0;
                Die();
            }
            

        }

        public void Heal(int amount)
        {
            CurrentHealth = Mathf.Min(CurrentHealth + amount, maxHealth);
        }

        public void Die()
        {
            
            if (agent != null)
            {
                agent.AddReward(-1f);
                agent.EndEpisode();
            }

            
            EventBus.Publish(new OnAgentDiedEvent());
        }

        public void ResetHealth()
        {
            CurrentHealth = maxHealth;
        }
    }
}
