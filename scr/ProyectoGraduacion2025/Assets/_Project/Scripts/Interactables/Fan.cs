using UnityEngine;
using Project.Core;

namespace Project.Interactables
{
    [RequireComponent(typeof(Collider))]
    public class Fan : MonoBehaviour
    {
        [Tooltip("Fuerza aplicada al agente (magnitude).")]
        public float force = 10f;

        [Tooltip("Dirección local del empuje (ej: forward).")]
        public Vector3 localDirection = Vector3.forward;

        [Tooltip("Si true, solo empuja cuando recibe evento (targetId)")]
        public bool eventControlled = false;

        [Tooltip("Si eventControlled, id que escucha")]
        public string listensToTargetId;

        private bool active = true;

        private void Start()
        {
            if (eventControlled)
            {
                active = false; 
                EventBus.Subscribe<OnDisplayMessageEvent>(OnDisplayMessage);
            }
        }

        private void OnDestroy()
        {
            if (eventControlled) EventBus.Unsubscribe<OnDisplayMessageEvent>(OnDisplayMessage);
        }

        private void OnDisplayMessage(OnDisplayMessageEvent e)
        {
            if (string.IsNullOrEmpty(listensToTargetId)) return;
            if (e.Message.Contains(listensToTargetId))
            {
                active = !active;
                EventBus.Publish(new OnDisplayMessageEvent { Message = $"{name} active={active}" });
            }
        }

        private void OnTriggerStay(Collider other)
        {
            if (!active) return;
            
            if (!other.CompareTag("Agent")) return;

            var rb = other.attachedRigidbody;
            if (rb == null) return;

            Vector3 dir = transform.TransformDirection(localDirection.normalized);
            rb.AddForce(dir * force, ForceMode.Force);
        }
    }
}
