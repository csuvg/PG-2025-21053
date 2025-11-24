using UnityEngine;
using System.Collections.Generic;

namespace Project.Data
{

    [CreateAssetMenu(fileName = "ObjectCatalog", menuName = "Game Data/Object Catalog")]
    public class ObjectCatalogSO : ScriptableObject
    {
        [SerializeField] private List<ObjectDataSO> allObjects = new();


        public IReadOnlyList<ObjectDataSO> AllObjects => allObjects;


        public ObjectDataSO GetById(string id)
        {
            return allObjects.Find(o => o.ObjectId == id);
        }


        public bool Contains(string id)
        {
            return allObjects.Exists(o => o.ObjectId == id);
        }

#if UNITY_EDITOR

        [ContextMenu("Eliminar duplicados")]
        private void RemoveDuplicates()
        {
            var seen = new HashSet<string>();
            allObjects.RemoveAll(o => o == null || !seen.Add(o.ObjectId));
            Debug.Log("[ObjectCatalogSO] Duplicados eliminados.");
        }
#endif
    }
}
