using UnityEngine;
using Project.Data;

namespace Project.Core
{

    public class StaticObjectReference : MonoBehaviour
    {
        [SerializeField] private ObjectDataSO data;
        public ObjectDataSO Data => data;
    }
}
