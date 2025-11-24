using UnityEngine;
using TMPro;
using Project.Core;

namespace Project.UI
{
    public class UIHUD : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI resourcesText;
        [SerializeField] private TextMeshProUGUI stateText;

        private void OnEnable()
        {
            EventBus.Subscribe<OnBudgetChangedEvent>(OnBudgetChanged);  
            EventBus.Subscribe<OnGameStateChangedEvent>(OnGameStateChanged);
        }

        private void OnDisable()
        {
            EventBus.Unsubscribe<OnBudgetChangedEvent>(OnBudgetChanged);
            EventBus.Unsubscribe<OnGameStateChangedEvent>(OnGameStateChanged);
        }

        private void OnBudgetChanged(OnBudgetChangedEvent evt)
        {
            resourcesText.text = $"Recursos: {evt.newBudget}";  
        }

        private void OnResourcesChanged(OnResourcesChangedEvent evt)
        {
            resourcesText.text = $"Recursos: {evt.newValue}";
        }

        private void OnGameStateChanged(OnGameStateChangedEvent evt)
        {
            stateText.text = $"Estado: {evt.NewState}";
        }
    }
}
