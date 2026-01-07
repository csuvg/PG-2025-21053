using UnityEngine;
using Project.Core;

namespace Project.Interactables
{
    [RequireComponent(typeof(Collider))]
    public class Goal : MonoBehaviour
    {
        [SerializeField] private float goalReward = 2f;

        private void OnTriggerEnter(Collider other)
        {
            if (!other.CompareTag("Agent")) return;

            var agent = other.GetComponent<Project.AI.RLAgentAdapter>();
            if (agent != null)
            {
                agent.AddCustomReward(goalReward);  
                agent.EndEpisode(); 
            }

            EventBus.Publish(new OnGoalReachedEvent());
        }
    }
}