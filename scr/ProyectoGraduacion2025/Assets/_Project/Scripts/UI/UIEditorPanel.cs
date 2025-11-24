using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections.Generic;
using Project.Data;
using Project.EditorRuntime;

namespace Project.UI
{
   
    public class UIEditorPanel : MonoBehaviour
    {
        [Header("Referencias")]
        [SerializeField] private Transform contentParent;
        [SerializeField] private GameObject buttonPrefab;
        [SerializeField] private ObjectCatalogSO catalog;
        [SerializeField] private LevelEditorManager levelEditor;

        private readonly List<Button> createdButtons = new();

        private void Awake()
        {
            
            if (levelEditor == null)
                levelEditor = FindAnyObjectByType<LevelEditorManager>();

            if (catalog == null)
                Debug.LogError("[UIEditorPanel] Falta asignar el catálogo de objetos.");
        }

        private void Start()
        {
            GenerateButtons();
        }

        public void GenerateButtons()
        {
            if (catalog == null || contentParent == null || buttonPrefab == null)
            {
                Debug.LogError("[UIEditorPanel] Referencias faltantes.");
                return;
            }

            if (levelEditor == null)
            {
                Debug.LogWarning("[UIEditorPanel] No se encontró LevelEditorManager en la escena.");
                return;
            }

            
            foreach (var b in createdButtons)
                if (b != null) Destroy(b.gameObject);
            createdButtons.Clear();

            
            foreach (var objData in catalog.AllObjects)
            {
                var buttonGO = Instantiate(buttonPrefab, contentParent);
                var label = buttonGO.GetComponentInChildren<TextMeshProUGUI>();
                label.text = objData.name;

                var btn = buttonGO.GetComponent<Button>();

                
                var localObj = objData;
                btn.onClick.AddListener(() =>
                {
                    if (levelEditor != null)
                        levelEditor.SelectObject(localObj);
                    else
                        Debug.LogError("[UIEditorPanel] LevelEditorManager no disponible al seleccionar objeto.");
                });

                createdButtons.Add(btn);
            }

            Debug.Log($"[UIEditorPanel] Generados {createdButtons.Count} botones de objetos.");
        }
    }
}
