using System.Collections.Generic;
using UnityEngine;
using Project.Core;
using Project.Data;

namespace Project.EditorRuntime
{

    public class LevelEditorManager : MonoBehaviour
    {


        [Header("Configuración general")]
        [SerializeField] private Camera editorCamera;
        [SerializeField] private ObjectCatalogSO catalog;
        private List<ObjectDataSO> availableObjects;
        

        [SerializeField] private Transform objectsParent;
        [SerializeField] private LevelData levelData;

        [Header("Vista previa")]
        [SerializeField] private Material previewMaterial;
        private GameObject currentPreview;
        private Material[] originalMaterials;


        private LevelData prePlayLevelState;
        private GridManager grid;
        private ResourceManager resources;
        private PlacementValidator validator;
        private ObjectDataSO selectedObject;
        private const string PREVIEW_LAYER_NAME = "Preview";

        
        private readonly Dictionary<Vector3Int, (GameObject instance, ObjectDataSO data, List<Vector3Int> footprint)> placedObjects 
            = new();


        public ObjectCatalogSO GetCatalog() => catalog;

        private Dictionary<GameObject, Vector3Int> initialPositions = new();



        
        private void Awake()
        {
            if (grid == null)
                grid = FindAnyObjectByType<GridManager>();
            if (resources == null)
                resources = FindAnyObjectByType<ResourceManager>();
        }

        
        private void Start()
        {
            validator = new PlacementValidator(grid, resources, levelData);

            if (catalog != null)
                availableObjects = new List<ObjectDataSO>(catalog.AllObjects);
            else
                Debug.LogError("[LevelEditorManager] No hay catálogo asignado.");

            if (availableObjects.Count > 0)
                selectedObject = availableObjects[0];

            if (editorCamera == null)
                editorCamera = Camera.main;
        }

        private void OnEnable()
        {
            EventBus.Subscribe<OnPlayModeStartedEvent>(OnPlayModeStarted);
            EventBus.Subscribe<OnRestartLevelEvent>(OnRestartLevel);
        }

        private void OnDisable()
        {
            EventBus.Unsubscribe<OnPlayModeStartedEvent>(OnPlayModeStarted);
            EventBus.Unsubscribe<OnRestartLevelEvent>(OnRestartLevel);
        }

        private void OnPlayModeStarted(OnPlayModeStartedEvent evt)
        {
            initialPositions.Clear();
            
            if (currentPreview != null)
                currentPreview.SetActive(false);  
            foreach (var kvp in placedObjects)
            {
                var gridPos = kvp.Key;
                var entry = kvp.Value;
                if (entry.instance != null)
                    initialPositions[entry.instance] = gridPos; 
            }

        }

        private void OnRestartLevel(OnRestartLevelEvent evt)
        {
            
            grid.ClearReservations();

            foreach (var entry in initialPositions)
            {
                if (entry.Key == null)
                    continue;

                
                Vector3Int gridPos = entry.Value;
                var placedData = placedObjects[gridPos];

                
                Vector3 worldPos = grid.GridToWorld(gridPos, placedData.data.Size, placedData.instance);
                placedData.instance.transform.position = worldPos;
                placedData.instance.transform.rotation = Quaternion.identity;

                
                var footprint = grid.GetFootprint(gridPos, placedData.data.Size, placedData.instance.transform.rotation);
                grid.ReserveCells(footprint);
            }

            Debug.Log("[LevelEditorManager] Escena reiniciada, objetos restaurados en su grid exacto.");
        }




        
        private void Update()
        {
            if (editorCamera == null)
            {
                editorCamera = Camera.main;
                if (editorCamera == null)
                    return; 
            }


            if (GameStateManager.Instance.CurrentState != GameState.EditMode)
                return;

            HandleObjectSelection();
            HandlePlacement();
            HandleDeletion();
            UpdatePlacementPreview();
        }


        
        public void SavePrePlayState()
        {
            prePlayLevelState = new LevelData("PrePlayState");
            foreach (var entry in GetPlacedObjects())
            {
                prePlayLevelState.AddObject(entry.ObjectId, entry.GridPosition, entry.Rotation);
            }
        }

        
        public void RestorePrePlayState(ObjectCatalogSO catalog)
        {
            if (prePlayLevelState == null)
            {
                Debug.LogWarning("[LevelEditorManager] No hay estado previo guardado para restaurar.");
                return;
            }

            ClearAllObjects();
            LoadLevel(prePlayLevelState, catalog, objectsParent);
        }


        
        private void HandleObjectSelection()
        {
            for (int i = 0; i < availableObjects.Count; i++)
            {
                if (Input.GetKeyDown(KeyCode.Alpha1 + i))
                {
                    selectedObject = availableObjects[i];
                    Debug.Log($"Seleccionado: {selectedObject.name}");
                }
            }
        }

        public void SelectObject(ObjectDataSO obj)
        {
            selectedObject = obj;

            if (currentPreview != null)
            {
                Destroy(currentPreview);
                currentPreview = null;
            }

            Debug.Log($"Objeto seleccionado desde UI: {obj.ObjectId}");
        }


        
        private void HandlePlacement()
        {
            if (Input.GetMouseButtonDown(0))
            {
                if (selectedObject == null) return;
                if (!TryGetGridPosition(out Vector3Int gridPos)) return;

                
                Destroy(currentPreview);
                currentPreview = null;

                Quaternion rotation = Quaternion.identity; 

                if (grid.IsCellOccupied(gridPos))
                {
                    Debug.Log("No puedes colocar objetos sobre estructuras fijas.");
                    return;
                }

                if (!validator.CanPlace(selectedObject, gridPos, rotation))
                {
                    Debug.Log("No se puede colocar el objeto aquí.");
                    return;
                }

                var footprint = grid.GetFootprint(gridPos, selectedObject.Size, rotation);
                if (!grid.ReserveCells(footprint))
                {
                    Debug.Log("Error al reservar celdas para el objeto.");
                    return;
                }

                Vector3 worldPos = grid.GridToWorld(gridPos, selectedObject.Size, selectedObject.Prefab);

                

                var instance = Instantiate(selectedObject.Prefab, worldPos, rotation, objectsParent);

                placedObjects[gridPos] = (instance, selectedObject, footprint);

                validator.RegisterPlacement(selectedObject);
                resources.TryConsume(selectedObject.Cost);
                
                
                
                

                EventBus.Publish(new OnObjectPlacedEvent
                {
                    objectId = selectedObject.ObjectId,
                    gridPosition = gridPos
                });
            }


        }

        
        private void HandleDeletion()
        {
            if (Input.GetMouseButtonDown(1))
            {
                if (!TryGetGridPosition(out Vector3Int gridPos)) return;

                if (placedObjects.TryGetValue(gridPos, out var entry))
                {
                    if (entry.instance != null)
                        Destroy(entry.instance);

                    if (entry.footprint != null && entry.footprint.Count > 0)
                        grid.ReleaseCells(entry.footprint);

                    placedObjects.Remove(gridPos);

                    resources.Refund(entry.data.Cost);
                    validator.RegisterRemoval(entry.data);

                    EventBus.Publish(new OnObjectRemovedEvent
                    {
                        objectId = entry.data.ObjectId,
                        gridPosition = gridPos
                    });
                }
            }
        }

    
    private void UpdatePlacementPreview()
    {
        if (selectedObject == null || selectedObject.Prefab == null)
            return;

        
        if (currentPreview == null)
        {
            currentPreview = Instantiate(selectedObject.Prefab);
            DisableBehavioursAndColliders(currentPreview);

            
            int previewLayer = LayerMask.NameToLayer("Preview");
            if (previewLayer == -1)
            {
                Debug.LogWarning("[LevelEditorManager] La layer 'Preview' no existe. Créala en Project Settings > Tags and Layers.");
            }
            else
            {
                SetLayerRecursively(currentPreview, previewLayer);
            }

            
            SaveAndReplaceMaterials(currentPreview, previewMaterial);
        }

        
        if (TryGetGridPosition(out Vector3Int gridPos))
        {
            Quaternion rotation = Quaternion.identity; 

            
            Vector3 worldPos = grid.GridToWorld(gridPos, selectedObject.Size, selectedObject.Prefab);
            currentPreview.transform.SetPositionAndRotation(worldPos, rotation);

            // 🔸 Cambiar color según validez
            bool canPlace = validator.CanPlace(selectedObject, gridPos, rotation);
            SetPreviewColor(canPlace ? Color.green : Color.red);
        }
    }

    
    private void DisableBehavioursAndColliders(GameObject go)
    {
        foreach (var col in go.GetComponentsInChildren<Collider>())
            col.enabled = false;

        foreach (var mb in go.GetComponentsInChildren<MonoBehaviour>())
        {
            
            if (mb == this) continue;
            mb.enabled = false;
        }
    }

    
    private void SetLayerRecursively(GameObject obj, int layer)
    {
        obj.layer = layer;
        foreach (Transform child in obj.transform)
            SetLayerRecursively(child.gameObject, layer);
    }


        private void SaveAndReplaceMaterials(GameObject obj, Material newMat)
        {
            var renderers = obj.GetComponentsInChildren<Renderer>();
            originalMaterials = new Material[renderers.Length];

            for (int i = 0; i < renderers.Length; i++)
            {
                originalMaterials[i] = renderers[i].sharedMaterial;
                renderers[i].sharedMaterial = newMat;
            }
        }

        private void SetPreviewColor(Color color)
        {
            if (previewMaterial != null)
            {
                previewMaterial.color = color;
            }
        }


        private bool TryGetGridPosition(out Vector3Int gridPos)
        {
            gridPos = default;
            Ray ray = editorCamera.ScreenPointToRay(Input.mousePosition);

            if (Physics.Raycast(ray, out RaycastHit hit, 100f))
            {
                gridPos = grid.WorldToGrid(hit.point);
                return true;
            }

            return false;
        }

        
        public void ClearAllObjects()
        {
            if (placedObjects != null)
            {
                foreach (var entry in placedObjects.Values)
                {
                    if (entry.instance != null)
                        Destroy(entry.instance);

                    if (entry.footprint != null && entry.footprint.Count > 0)
                        grid.ReleaseCells(entry.footprint);
                }
                placedObjects.Clear();
            }
        }

        
        public IEnumerable<(string ObjectId, Vector3Int GridPosition, Quaternion Rotation)> GetPlacedObjects()
        {
            foreach (var kvp in placedObjects)
            {
                var data = kvp.Value.data;
                var instance = kvp.Value.instance;
                yield return (data.ObjectId, kvp.Key, instance.transform.rotation);
            }
        }

        
        public void LoadLevel(LevelData data, ObjectCatalogSO catalog, Transform parent)
        {
            ClearAllObjects();

            if (data == null)
            {
                Debug.LogWarning("[LevelEditorManager] No se proporcionó LevelData para cargar.");
                return;
            }

            foreach (var obj in data.placedObjects)
            {
                var dataSO = catalog.GetById(obj.objectId);
                if (dataSO == null)
                {
                    Debug.LogWarning($"[LevelEditorManager] Object ID '{obj.objectId}' no encontrado en catálogo.");
                    continue;
                }

                Quaternion rotation = obj.rotation;
                Vector3Int gridPos = obj.gridPosition;
                Vector3 worldPos = grid.GridToWorld(gridPos, dataSO.Size);

                var instance = Instantiate(dataSO.Prefab, worldPos, rotation, parent);

                var footprint = grid.GetFootprint(gridPos, dataSO.Size, rotation);
                grid.ReserveCells(footprint);

                placedObjects[gridPos] = (instance, dataSO, footprint);

                EventBus.Publish(new OnObjectPlacedEvent
                {
                    objectId = dataSO.ObjectId,
                    gridPosition = gridPos
                });
            }

            Debug.Log($"[LevelEditorManager] Nivel '{data.levelName}' cargado con {data.placedObjects.Count} objetos.");
        }





    }
}
