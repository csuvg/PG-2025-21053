using System;
using System.Collections.Generic;
using UnityEngine;

namespace Project.Data
{
    [Serializable]
    public class LevelData
    {
        [Serializable]
        public class PlacedObjectData
        {
            public string objectId;
            public Vector3Int gridPosition;
            public Quaternion rotation;
        }

        public string levelName;
        public Vector3Int agentSpawn;
        public Vector3Int goalPosition;
        public List<PlacedObjectData> placedObjects = new();


        public Dictionary<string, int> limitsPerObject = new();

        public LevelData(string name)
        {
            levelName = name;
        }

        public void AddObject(string objectId, Vector3Int pos, Quaternion rot)
        {
            placedObjects.Add(new PlacedObjectData
            {
                objectId = objectId,
                gridPosition = pos,
                rotation = rot
            });
        }

        public void Clear()
        {
            placedObjects.Clear();
        }

        public int GetLimitFor(string objectId)
        {
            if (limitsPerObject == null || !limitsPerObject.ContainsKey(objectId))
                return 0; 
            return limitsPerObject[objectId];
        }

        public PlacedObjectData GetObjectAt(Vector3Int gridPos)
        {
            return placedObjects.Find(o => o.gridPosition == gridPos);
        }
    }
}
