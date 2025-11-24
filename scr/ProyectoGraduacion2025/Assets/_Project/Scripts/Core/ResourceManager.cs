using UnityEngine;

namespace Project.Core
{
    /// <summary>
    /// Administra el presupuesto (budget) del jugador.
    /// Durante el modo edición, define cuántos objetos puede colocar.
    /// También puede incrementarse con recolectables durante la simulación.
    /// </summary>
    public class ResourceManager : MonoBehaviour
    {
        [Header("Configuración inicial")]
        [SerializeField] private int initialBudget = 15;
        public int CurrentBudget { get; private set; }

        private void Awake()
        {
            CurrentBudget = initialBudget;
            PublishBudgetChange(0, initialBudget);
        }

        private void OnEnable()
        {
            EventBus.Subscribe<OnGameStateChangedEvent>(OnGameStateChanged);
        }

        private void OnDisable()
        {
            EventBus.Unsubscribe<OnGameStateChangedEvent>(OnGameStateChanged);
        }


        private void OnGameStateChanged(OnGameStateChangedEvent evt)
        {
            if (evt.NewState == GameState.EditMode)
            {
                ResetBudget();
            }
        }


        public bool TryConsume(int cost)
        {
            if (cost <= 0) return true;
            if (cost > CurrentBudget) return false;

            int prev = CurrentBudget;
            CurrentBudget -= cost;
            PublishBudgetChange(prev, CurrentBudget);

            return true;
        }


        public void AddBudget(int amount)
        {
            if (amount <= 0) return;

            int prev = CurrentBudget;
            CurrentBudget += amount;
            PublishBudgetChange(prev, CurrentBudget);
        }


        public void Refund(int amount)
        {
            if (amount <= 0) return;

            int prev = CurrentBudget;
            CurrentBudget += amount;
            PublishBudgetChange(prev, CurrentBudget);
        }


        public void ResetBudget()
        {
            int prev = CurrentBudget;
            CurrentBudget = initialBudget;
            PublishBudgetChange(prev, CurrentBudget);
        }


        public void SetBudget(int amount)
        {
            int prev = CurrentBudget;
            CurrentBudget = Mathf.Max(0, amount);
            PublishBudgetChange(prev, CurrentBudget);
        }

        private void PublishBudgetChange(int previous, int current)
        {
            EventBus.Publish(new OnResourcesChangedEvent
            {
                previousValue = previous,
                newValue = current
            });

            EventBus.Publish(new OnBudgetChangedEvent
            {
                previousBudget = previous,
                newBudget = current
            });
        }
    }
}
