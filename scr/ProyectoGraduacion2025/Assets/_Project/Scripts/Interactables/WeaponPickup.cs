using UnityEngine;
using Project.AI;
using Project.Core;

namespace Project.Interactables
{
    [RequireComponent(typeof(Collider))]
    public class WeaponPickup : MonoBehaviour
    {
        [Tooltip("Nombre del arma o id")]
        public string weaponId = "sword";

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

            var agent = other.GetComponent<AgentController>();
            agent?.GiveWeapon(weaponId);

            gameObject.SetActive(false);  // Hide
        }
    }
}