using System.Collections.Generic;
using UnityEngine;
using Project.Core;
using Project.Data;

namespace Project.EditorRuntime
{

    public class GridManager : MonoBehaviour
    {
        [Header("Grid Settings")]
        [SerializeField] private int width = 20;
        [SerializeField] private int height = 20;
        [SerializeField] private float cellSize = 1f;
        [SerializeField] private Vector3 origin = Vector3.zero;
        [SerializeField] private bool drawGizmos = true;
        [SerializeField] private Color gridColor = Color.gray;
        [SerializeField] private Color occupiedColor = new Color(1f, 0.2f, 0.2f, 0.5f);


        private readonly HashSet<Vector2Int> occupied = new HashSet<Vector2Int>();


        private void Start()
        {
            RegisterStaticObjects();
        }

        private void RegisterStaticObjects()
        {
            var staticObjects = GameObject.FindGameObjectsWithTag("StaticObject");

            foreach (var obj in staticObjects)
            {
                var reference = obj.GetComponent<Project.Core.StaticObjectReference>();
                if (reference == null)
                {
                    Debug.LogWarning($"[GridManager] El objeto '{obj.name}' tiene el tag 'StaticObject' pero no tiene StaticObjectReference.");
                    continue;
                }

                ObjectDataSO data = reference.Data;
                if (data == null)
                {
                    Debug.LogWarning($"[GridManager] '{obj.name}' no tiene asignado un ObjectDataSO.");
                    continue;
                }

                Quaternion rotation = obj.transform.rotation;

                
                Vector3 worldPos = obj.transform.position;

                
                float offsetX = (data.Size.x - 1) * 0.5f * cellSize;
                float offsetZ = (data.Size.y - 1) * 0.5f * cellSize;

                
                Vector3 adjustedPos = worldPos - new Vector3(offsetX, 0, offsetZ);

                Vector3Int gridPos = WorldToGrid(adjustedPos);

                var footprint = GetFootprint(gridPos, data.Size, rotation);
                ReserveCells(footprint);
            }

            Debug.Log($"[GridManager] Registrados {staticObjects.Length} objetos estáticos en el grid (alineados al centro).");
        }







        private void OnValidate()
        {
            if (width < 1) width = 1;
            if (height < 1) height = 1;
            if (cellSize <= 0.01f) cellSize = 0.1f;
        }

        // 🔹 Conversión de coordenadas
        public Vector3Int WorldToGrid(Vector3 worldPos)
        {
            // Convertir a coordenadas locales del grid
            Vector3 local = transform.InverseTransformPoint(worldPos) - origin;
            int x = Mathf.FloorToInt(local.x / cellSize);
            int z = Mathf.FloorToInt(local.z / cellSize);
            return new Vector3Int(x, 0, z);
        }

        public Vector3 GridToWorld(Vector3Int gridPos, Vector2Int size, GameObject prefab = null)
        {
            // Calcular posición en espacio local
            float localX = origin.x + (gridPos.x + size.x * 0.5f) * cellSize;
            float localZ = origin.z + (gridPos.z + size.y * 0.5f) * cellSize;
            float localY = origin.y;

            if (prefab != null)
            {
                var renderer = prefab.GetComponentInChildren<Renderer>();
                if (renderer != null)
                {
                    float height = renderer.bounds.size.y;
                    localY += height * 0.5f;
                }
            }

            // Transformar a coordenadas globales del mundo
            return transform.TransformPoint(new Vector3(localX, localY, localZ));
        }




        // 🔹 Reserva de una celda
        public bool ReserveCell(Vector3Int gridPos)
        {
            var key = new Vector2Int(gridPos.x, gridPos.z);
            if (IsOutOfBounds(key))
            {
                Debug.LogWarning($"[GridManager] Celda fuera de rango: {gridPos}");
                return false;
            }
            if (occupied.Contains(key)) return false;

            occupied.Add(key);
            return true;
        }

        // 🔹 Liberar una celda
        public bool ReleaseCell(Vector3Int gridPos)
        {
            var key = new Vector2Int(gridPos.x, gridPos.z);
            return occupied.Remove(key);
        }

        public bool IsCellOccupied(Vector3Int gridPos)
        {
            var key = new Vector2Int(gridPos.x, gridPos.z);
            return occupied.Contains(key);
        }

        public bool IsOutOfBounds(Vector2Int key)
        {
            return key.x < 0 || key.x >= width || key.y < 0 || key.y >= height;
        }

        // 🔹 Objetos multicelda
        public bool ReserveCells(IEnumerable<Vector3Int> cells)
        {
            // Primero validar que todas las celdas estén libres
            foreach (var cell in cells)
            {
                var key = new Vector2Int(cell.x, cell.z);
                if (IsOutOfBounds(key) || occupied.Contains(key))
                    return false;
            }

            // Luego reservar todas
            foreach (var cell in cells)
                occupied.Add(new Vector2Int(cell.x, cell.z));

            return true;
        }

        public void ReleaseCells(IEnumerable<Vector3Int> cells)
        {
            foreach (var cell in cells)
                occupied.Remove(new Vector2Int(cell.x, cell.z));
        }

        // 🔹 Limpieza completa (por ejemplo, al reiniciar nivel)
        public void ClearReservations()
        {
            occupied.Clear();
        }
        
        public List<Vector3Int> GetFootprint(Vector3Int gridPos, Vector2Int size, Quaternion rotation)
        {
            List<Vector3Int> footprint = new();

            int rotationIndex = Mathf.RoundToInt(rotation.eulerAngles.y / 90f) % 4;

            for (int x = 0; x < size.x; x++)
            {
                for (int z = 0; z < size.y; z++)
                {
                    Vector3Int offset = new(x, 0, z);
                    Vector3Int rotated = offset;

                    switch (rotationIndex)
                    {
                        case 1: // 90°
                            rotated = new Vector3Int(z, 0, size.x - 1 - x);
                            break;
                        case 2: // 180°
                            rotated = new Vector3Int(size.x - 1 - x, 0, size.y - 1 - z);
                            break;
                        case 3: // 270°
                            rotated = new Vector3Int(size.y - 1 - z, 0, x);
                            break;
                    }

                    footprint.Add(gridPos + rotated);
                }
            }

            return footprint;
        }


        // 🔹 Dibujo de grid (solo en modo edición)
        private void OnDrawGizmos()
        {
            if (!drawGizmos) return;

        #if UNITY_EDITOR
            if (Application.isPlaying)
            {
                var gsm = GameStateManager.Instance;
                if (gsm != null && gsm.CurrentState != GameState.EditMode)
                    return;
            }
        #endif

            Gizmos.color = gridColor;

            // 🔸 Base visual del grid justo sobre el plano
            float gridY = transform.position.y + origin.y;

            for (int x = 0; x < width; x++)
            {
                for (int z = 0; z < height; z++)
                {
                    var center = GridToWorld(new Vector3Int(x, 0, z), Vector2Int.one);
                    var half = cellSize * 0.5f;

                    // 🔸 Dibuja las líneas del grid
                    var min = new Vector3(center.x - half, gridY, center.z - half);
                    var max = new Vector3(center.x + half, gridY, center.z + half);

                    Gizmos.DrawLine(min, new Vector3(max.x, gridY, min.z));
                    Gizmos.DrawLine(min, new Vector3(min.x, gridY, max.z));

                    // 🔸 Si está ocupada, pinta una celda semitransparente
                    if (occupied.Contains(new Vector2Int(x, z)))
                    {
                        Gizmos.color = occupiedColor;
                        Gizmos.DrawCube(new Vector3(center.x, gridY + 0.01f, center.z), new Vector3(cellSize, 0.02f, cellSize));
                        Gizmos.color = gridColor;
                    }
                }
            }

            // 🔸 Dibuja borde del grid completo
            Gizmos.color = Color.white;
            Vector3 cornerA = transform.TransformPoint(origin + new Vector3(0, 0, 0));
            Vector3 cornerB = transform.TransformPoint(origin + new Vector3(width * cellSize, 0, 0));
            Vector3 cornerC = transform.TransformPoint(origin + new Vector3(width * cellSize, 0, height * cellSize));
            Vector3 cornerD = transform.TransformPoint(origin + new Vector3(0, 0, height * cellSize));

            Gizmos.DrawLine(cornerA, cornerB);
            Gizmos.DrawLine(cornerB, cornerC);
            Gizmos.DrawLine(cornerC, cornerD);
            Gizmos.DrawLine(cornerD, cornerA);
        }


    }
}
