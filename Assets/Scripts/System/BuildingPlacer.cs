using UnityEngine;
using UnityEngine.InputSystem;

public class BuildingPlacer : MonoBehaviour
{
    [Header("Building Placement")]
    public GameObject buildingPrefab;
    public LayerMask groundLayer = 1;
    public Material previewMaterial;
    
    [Header("Grid Settings")]
    public TerrainManager terrainManager;
    
    private GameObject previewBuilding;
    private Camera playerCamera;
    private bool isPlacementMode = false;
    
    private Material validPreviewMaterial;
    private Material invalidPreviewMaterial;
    
    void Start()
    {
        playerCamera = Camera.main;
        if (terrainManager == null)
            terrainManager = FindFirstObjectByType<TerrainManager>();
            
        // Create and cache preview materials
        CreatePreviewMaterials();
    }
    
    void CreatePreviewMaterials()
    {
        if (previewMaterial == null)
        {
            previewMaterial = new Material(Shader.Find("Standard"));
            previewMaterial.color = new Color(0, 1, 0, 0.5f); // Semi-transparent green
        }
        
        // Create cached materials for valid and invalid placement
        validPreviewMaterial = new Material(previewMaterial);
        validPreviewMaterial.color = new Color(0, 1, 0, 0.5f); // Green
        
        invalidPreviewMaterial = new Material(previewMaterial);
        invalidPreviewMaterial.color = new Color(1, 0, 0, 0.5f); // Red
    }
    
    void Update()
    {
        if (isPlacementMode)
        {
            UpdateBuildingPreview();
            
            // Use new Input System for mouse clicks
            Mouse mouse = Mouse.current;
            if (mouse != null && mouse.leftButton.wasPressedThisFrame)
            {
                TryPlaceBuilding();
            }
            
            // Use new Input System for escape key
            Keyboard keyboard = Keyboard.current;
            if (keyboard != null && keyboard.escapeKey.wasPressedThisFrame)
            {
                CancelPlacement();
            }
        }
        
    }
    
    void UpdateBuildingPreview()
    {
        if (previewBuilding == null) return;
        
        // Use new Input System for mouse position
        Mouse mouse = Mouse.current;
        if (mouse == null) return;
        
        Vector2 mousePosition = mouse.position.ReadValue();
        Ray ray = playerCamera.ScreenPointToRay(mousePosition);
        
        if (Physics.Raycast(ray, out RaycastHit hit, Mathf.Infinity, groundLayer))
        {
            Vector3 targetPosition = hit.point;
            
            if (terrainManager != null)
            {
                targetPosition = terrainManager.SnapToGrid(targetPosition);
            }
            
            previewBuilding.transform.position = targetPosition;
            
            // Change color based on whether position is valid
            bool canPlace = terrainManager == null || terrainManager.IsGridPositionAvailable(targetPosition);
            Color previewColor = canPlace ? Color.green : Color.red;
            previewColor.a = 0.5f;
            
            var renderer = previewBuilding.GetComponent<Renderer>();
            if (renderer != null)
            {
                renderer.material = canPlace ? validPreviewMaterial : invalidPreviewMaterial;
            }
        }
    }
    
    void TryPlaceBuilding()
    {
        if (previewBuilding == null || buildingPrefab == null) return;
        
        Vector3 buildPosition = previewBuilding.transform.position;
        
        // Check if position is valid
        if (terrainManager != null && !terrainManager.IsGridPositionAvailable(buildPosition))
        {
            Debug.Log("Cannot place building here - position occupied!");
            return;
        }
        
        // Instantiate the actual building
        var newBuilding = Instantiate(buildingPrefab, buildPosition, Quaternion.identity);
        newBuilding.name = buildingPrefab.name + "_" + System.DateTime.Now.Ticks;
        
        Debug.Log($"Building placed at {buildPosition}");
        
        // Continue placement mode (don't exit automatically)
    }
    
    public void TogglePlacementMode()
    {
        isPlacementMode = !isPlacementMode;
        
        if (isPlacementMode)
        {
            StartPlacement();
        }
        else
        {
            CancelPlacement();
        }
    }
    
    public void StartBuildingPlacement(GameObject buildingPrefab)
    {
        SetBuildingPrefab(buildingPrefab);
        if (!isPlacementMode)
        {
            TogglePlacementMode();
        }
    }
    
    void StartPlacement()
    {
        if (buildingPrefab == null)
        {
            Debug.LogWarning("No building prefab assigned!");
            isPlacementMode = false;
            return;
        }
        
        // Create preview building
        previewBuilding = Instantiate(buildingPrefab);
        previewBuilding.name = "BuildingPreview";
        
        // Disable colliders on preview
        var colliders = previewBuilding.GetComponentsInChildren<Collider>();
        foreach (var col in colliders)
        {
            col.enabled = false;
        }
        
        Debug.Log("Building placement mode activated. Press B to cancel, Left-click to place, Escape to cancel.");
    }
    
    void CancelPlacement()
    {
        isPlacementMode = false;
        
        if (previewBuilding != null)
        {
            DestroyImmediate(previewBuilding);
            previewBuilding = null;
        }
        
        Debug.Log("Building placement cancelled.");
    }
    
    // Set building prefab to place
    public void SetBuildingPrefab(GameObject prefab)
    {
        buildingPrefab = prefab;
        
        if (isPlacementMode)
        {
            CancelPlacement();
            StartPlacement();
        }
    }
    
    void OnDestroy()
    {
        if (validPreviewMaterial != null)
            DestroyImmediate(validPreviewMaterial);
        if (invalidPreviewMaterial != null)
            DestroyImmediate(invalidPreviewMaterial);
    }
}
