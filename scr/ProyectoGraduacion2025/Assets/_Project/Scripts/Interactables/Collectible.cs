using UnityEngine;
using Project.Core;  

namespace Project.Interactables
{
    [RequireComponent(typeof(Collider))]
    public class Collectible : MonoBehaviour
    {
        [Tooltip("Valor que añade al presupuesto")]
        public int value = 1;

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

            var rm = FindAnyObjectByType<ResourceManager>();
            rm?.AddBudget(value);

            EventBus.Publish(new OnDisplayMessageEvent { Message = $"+{value} budget" });

            gameObject.SetActive(false);  
        }
    }
}