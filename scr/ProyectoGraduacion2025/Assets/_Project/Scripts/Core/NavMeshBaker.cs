using UnityEngine;
using UnityEngine.AI;
using Project.Core;
using System.Collections;
using Unity.AI.Navigation;
using Unity.MLAgents;

namespace Project.Navigation
{
    public class NavMeshRuntimeBaker : MonoBehaviour
    {
        [SerializeField] private float delayBeforeBake = 0.2f;

        private void OnEnable()
        {
            EventBus.Subscribe<OnPlayModeStartedEvent>(OnPlayStarted);
        }

        private void OnDisable()
        {
            EventBus.Unsubscribe<OnPlayModeStartedEvent>(OnPlayStarted);
        }

        private void OnPlayStarted(OnPlayModeStartedEvent evt)
        {

            StartCoroutine(RebakeAllAndActivateEnemies());
        }

        private IEnumerator RebakeAllAndActivateEnemies()
        {
            yield return new WaitForSeconds(delayBeforeBake);

            var allSurfaces = FindObjectsByType<NavMeshSurface>(FindObjectsSortMode.None);
            Debug.Log($"[Baker] Baking {allSurfaces.Length} surfaces...");

            foreach (var surface in allSurfaces)
            {
                int previewLayer = LayerMask.NameToLayer("Preview");
                if (previewLayer != -1)
                    surface.layerMask = ~(1 << previewLayer);

                surface.BuildNavMesh();
                yield return null;
                Debug.Log($"[Baker] Baked: {surface.name}");
            }

            Debug.Log("[Baker] ✅ Bake completo → Activating enemies");
            ActivateAllEnemies();
        }

        private void ActivateAllEnemies()
        {
            var allEnemies = FindObjectsByType<AI.EnemyController>(FindObjectsSortMode.None);
            foreach (var enemy in allEnemies)
                enemy.ActivateAI();
        }
    }
}