using System.Collections.Generic;
using UnityEngine;
using Project.Core;
using Project.Data;

namespace Project.EditorRuntime
{
    
    public class PlacementValidator
    {
        private readonly GridManager grid;
        private readonly ResourceManager resources;
        private readonly LevelData currentLevel;
        private readonly Dictionary<string, int> placedCount = new();

        public PlacementValidator(GridManager grid, ResourceManager resources, LevelData levelData)
        {
            this.grid = grid;
            this.resources = resources;
            this.currentLevel = levelData;
        }

        public bool CanPlace(ObjectDataSO data, Vector3Int gridPos, Quaternion rotation)
        {
            if (data == null) return false;
            if (!data.PlaceableByPlayer) return false;

            
            if (resources.CurrentBudget < data.Cost)
                return false;

            
            int currentCount = placedCount.ContainsKey(data.ObjectId) ? placedCount[data.ObjectId] : 0;
            int levelLimit = currentLevel.GetLimitFor(data.ObjectId);
            int objectLimit = data.MaxPerLevel > 0 ? data.MaxPerLevel : levelLimit;

            if (objectLimit > 0 && currentCount >= objectLimit)
                return false;

            
            List<Vector3Int> footprint = grid.GetFootprint(gridPos, data.Size, rotation);
            foreach (var cell in footprint)
            {
                if (grid.IsOutOfBounds(new Vector2Int(cell.x, cell.z)))
                    return false;
                if (grid.IsCellOccupied(cell))
                    return false;
            }

            return true;
        }

        
        public void RegisterPlacement(ObjectDataSO data)
        {
            if (data == null) return;
            if (!placedCount.ContainsKey(data.ObjectId))
                placedCount[data.ObjectId] = 0;
            placedCount[data.ObjectId]++;
        }

        
        public void RegisterRemoval(ObjectDataSO data)
        {
            if (data == null) return;
            if (placedCount.ContainsKey(data.ObjectId))
            {
                placedCount[data.ObjectId] = Mathf.Max(0, placedCount[data.ObjectId] - 1);
            }
        }

        
        public void ResetPlacedCount()
        {
            placedCount.Clear();
        }
    }
}
