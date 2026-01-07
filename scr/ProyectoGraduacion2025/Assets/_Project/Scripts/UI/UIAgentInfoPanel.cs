using UnityEngine;
using TMPro;
using Project.AI;
using Unity.MLAgents;

namespace Project.UI
{
    public class UIAgentInfoPanel : MonoBehaviour
    {
        [Header("Referencias de UI")]
        [SerializeField] private TextMeshProUGUI healthText;
        [SerializeField] private TextMeshProUGUI episodeText;
        [SerializeField] private TextMeshProUGUI stepText;
        [SerializeField] private TextMeshProUGUI rewardText;

        [Header("Referencias del Agente")]
        [SerializeField] private RLAgentAdapter agent;
        [SerializeField] private AgentStats stats;


        private void Awake()
        {
            if (agent == null)
                agent = FindAnyObjectByType<RLAgentAdapter>();

            if (stats == null)
                stats = FindAnyObjectByType<AgentStats>();
        }

        private void Update()
        {
            if (agent == null || stats == null) return;

            
            healthText.text = $"Vida: {stats.CurrentHealth} / {stats.MaxHealth}";

            
            episodeText.text = $"Época: {agent.CompletedEpisodes}";

            
            stepText.text = $"Pasos: {agent.StepCount}";

            
            rewardText.text = $"Reward: {agent.GetCumulativeReward():F2}";
        }
    }
}
