using UnityEngine;
using Unity.MLAgents;
using Unity.MLAgents.Sensors;
using Unity.MLAgents.Actuators;
using Project.Core;

namespace Project.AI
{
    [RequireComponent(typeof(Rigidbody))]
    public class RLAgentAdapter : Agent
    {
        [Header("Movimiento")]
        [SerializeField] private float moveSpeed = 1.5f;
        private float originalMoveSpeed;
        private float currentMoveSpeed;
        private Rigidbody rb;

        [Header("Entorno local")]
        [Tooltip("Meta asociada a este agente (asignada automáticamente si está en el mismo entorno).")]
        [SerializeField] private Transform localGoal;

        private Vector3 localStartPosition;
        private Quaternion localStartRotation;

        [Header("Recompensas")]
        [SerializeField] private float stepPenalty = -0.001f;
        [SerializeField] private float collisionPenalty = -0.1f;


        [Header("Timeout Época")]
        [Tooltip("Pasos máx por episodio (e.g., 500 ~30s)")]
        [SerializeField] private int maxStepsPerEpisode = 1000;
        [Tooltip("Penalty por timeout (pierde)")]
        [SerializeField] private float timeoutPenalty = -1f;

        private int episodeStepCount = 0;

        private bool isActive = false;


        private new void Awake()
        {
            rb = GetComponent<Rigidbody>();
            originalMoveSpeed = moveSpeed;
            currentMoveSpeed = moveSpeed;
        }

        private void Start()
        {
            localStartPosition = transform.localPosition;
            localStartRotation = transform.localRotation;

            EventBus.Subscribe<OnPlayModeStartedEvent>(OnPlayStarted);
            EventBus.Subscribe<OnEditModeEnteredEvent>(OnEditMode);
            EventBus.Subscribe<OnRestartLevelEvent>(OnResetLevel);
            EventBus.Subscribe<OnStartTrainingEvent>(OnTrainingStarted);
        }

        

        private void OnDestroy()
        {
            EventBus.Unsubscribe<OnPlayModeStartedEvent>(OnPlayStarted);
            EventBus.Unsubscribe<OnEditModeEnteredEvent>(OnEditMode);
            EventBus.Unsubscribe<OnRestartLevelEvent>(OnResetLevel);
        }

        private void OnTrainingStarted(OnStartTrainingEvent evt)
        {
            
            if (Academy.Instance != null && !Academy.Instance.IsCommunicatorOn)
            {
                Debug.LogWarning("[RLAgent] Esperando conexión Python... (Corre mlagents-learn)");
                return;
            }

            
            Academy.Instance.EnvironmentStep();  
            Debug.Log("[RLAgent] Training hot-started! (Communicator ON)");
        }

        
        public void SetLocalGoal(Transform goal)
        {
            localGoal = goal;
        }

        private void OnPlayStarted(OnPlayModeStartedEvent e)
        {
            ResetAgentPosition();
            isActive = true;
            rb.isKinematic = false;
        }

        private void OnEditMode(OnEditModeEnteredEvent e)
        {
            isActive = false;
            rb.isKinematic = true;
        }

        private void OnResetLevel(OnRestartLevelEvent e)
        {
            ResetAgentPosition();
            isActive = false;
            EndEpisode();
        }

        public override void OnEpisodeBegin()
        {
            ResetAgentPosition();
            episodeStepCount = 0;
            EventBus.Publish(new OnEpisodeRestartEvent());
            RemoveSlow();
        }

        public void ResetAgentPosition()
        {
            rb.linearVelocity = Vector3.zero;
            rb.angularVelocity = Vector3.zero;
            transform.localPosition = localStartPosition;
            transform.localRotation = localStartRotation;
            rb.isKinematic = false;
        }

        public override void CollectObservations(VectorSensor sensor)
        {
            sensor.AddObservation(transform.localPosition);
            sensor.AddObservation(rb.linearVelocity);

            if (localGoal != null)
                sensor.AddObservation(localGoal.localPosition - transform.localPosition);
            else
                sensor.AddObservation(Vector3.zero);

        }

        public override void OnActionReceived(ActionBuffers actions)
        {
            if (!isActive) return;

            episodeStepCount++;

            if (episodeStepCount >= maxStepsPerEpisode)
            {
                AddReward(timeoutPenalty);  
                EndEpisode(); 
                Debug.Log($"[RLAgent] Timeout! Pasos: {episodeStepCount}. Penalty: {timeoutPenalty}");
                return;
            }

            int move = actions.DiscreteActions[0];
            Vector3 dir = Vector3.zero;
            

            switch (move)
            {
                case 1: dir = Vector3.forward; break;
                case 2: dir = Vector3.back; break;
                case 3: dir = Vector3.left; break;
                case 4: dir = Vector3.right; break;
            }
            

            float moveStep = currentMoveSpeed * Time.fixedDeltaTime;

            Vector3 localDir = transform.parent.InverseTransformDirection(dir);  
            Vector3 localNewPos = transform.localPosition + localDir * moveStep;

            localNewPos.x = Mathf.Clamp(localNewPos.x, 0f, 20f);  
            localNewPos.z = Mathf.Clamp(localNewPos.z, 0f, 20f);

            Vector3 newPos = transform.parent.TransformPoint(localNewPos);
            rb.MovePosition(newPos);
            AddReward(stepPenalty);
        }

        public void ApplySlow(float multiplier)
        {
            currentMoveSpeed = originalMoveSpeed * multiplier;
        }

        
        public void RemoveSlow()
        {
            currentMoveSpeed = originalMoveSpeed;
        }

        public override void Heuristic(in ActionBuffers actionsOut)
        {
            var discrete = actionsOut.DiscreteActions;
            discrete[0] = 0;

            if (Input.GetKey(KeyCode.W)) discrete[0] = 1;
            else if (Input.GetKey(KeyCode.S)) discrete[0] = 2;
            else if (Input.GetKey(KeyCode.A)) discrete[0] = 3;
            else if (Input.GetKey(KeyCode.D)) discrete[0] = 4;
        }

        private void OnTriggerEnter(Collider other)
        {
            if (!isActive) return;
        }

        private void OnCollisionEnter(Collision collision)
        {
            if (!isActive) return;

            if (collision.gameObject.CompareTag("Wall") ||
                collision.gameObject.CompareTag("StaticObject"))
            {
                AddReward(collisionPenalty);
            }
        }

        public void AddCustomReward(float amount)
        {
            AddReward(amount);
        }
    }
}
