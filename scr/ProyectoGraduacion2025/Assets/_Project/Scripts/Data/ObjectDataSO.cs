using UnityEngine;

namespace Project.Data
{

    [CreateAssetMenu(fileName = "ObjectData", menuName = "Game Data/Object Data")]
    public class ObjectDataSO : ScriptableObject
    {
        [Header("Identificación")]
        [SerializeField] private string objectId;
        public string ObjectId => objectId;

        [Header("Referencia de Prefab")]
        [SerializeField] private GameObject prefab;
        public GameObject Prefab => prefab;

        [Header("Costo y límites")]
        [SerializeField, Min(0)] private int cost = 1;
        public int Cost => cost;

        [Tooltip("Cantidad máxima de este tipo de objeto permitida por nivel (0 = sin límite)")]
        [SerializeField, Min(0)] private int maxPerLevel = 0;
        public int MaxPerLevel => maxPerLevel;

        [Header("Tamaño y colocación")]
        [Tooltip("Tamaño del objeto en celdas (X,Z) dentro del grid")]
        [SerializeField] private Vector2Int size = Vector2Int.one;
        public Vector2Int Size => size;

        [Tooltip("¿Puede el jugador colocar este objeto durante el modo edición?")]
        [SerializeField] private bool placeableByPlayer = true;
        public bool PlaceableByPlayer => placeableByPlayer;

        [Tooltip("¿El objeto puede rotarse al colocarse?")]
        [SerializeField] private bool rotatable = false;
        public bool Rotatable => rotatable;

        [Header("Interacción")]
        [Tooltip("Tipo general del objeto (obstáculo, trampa, interactivo, decorativo...)")]
        [SerializeField] private string category;
        public string Category => category;

        [Header("Descripción (opcional)")]
        [TextArea]
        [SerializeField] private string description;
        public string Description => description;
    }
}
