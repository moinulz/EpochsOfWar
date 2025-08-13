using UnityEngine;

public class TerrainManager : MonoBehaviour
{
    [Header("Grid Settings")]
    public float gridSize = 2f;
    public int gridWidth = 50;
    public int gridHeight = 50;
    public bool showGrid = true;
    
    [Header("Visual Settings")]
    public Color gridLineColor = new Color(0.5f, 0.7f, 0.3f, 0.8f);
    public float lineWidth = 0.1f;
    
    [Header("Building Placement")]
    public LayerMask buildingLayer = 1;
    
    private GameObject gridParent;
    private Vector3 centerOffset;
    
    void Start()
    {
        centerOffset = new Vector3(-(gridWidth * gridSize) / 2f, 0, -(gridHeight * gridSize) / 2f);
        if (showGrid)
        {
            CreateRuntimeGrid();
        }
    }
    
    void CreateRuntimeGrid()
    {
        if (gridParent != null)
            DestroyImmediate(gridParent);
            
        gridParent = new GameObject("RuntimeGrid");
        gridParent.transform.SetParent(transform);
        
        // Create material for grid lines
        var lineMaterial = new Material(Shader.Find("Sprites/Default"));
        lineMaterial.color = gridLineColor;
        
        // Vertical lines
        for (int x = 0; x <= gridWidth; x++)
        {
            CreateGridLine(gridParent.transform, $"GridLine_V_{x}", lineMaterial,
                new Vector3(x * gridSize + centerOffset.x, 0.05f, centerOffset.z),
                new Vector3(x * gridSize + centerOffset.x, 0.05f, gridHeight * gridSize + centerOffset.z));
        }
        
        // Horizontal lines
        for (int z = 0; z <= gridHeight; z++)
        {
            CreateGridLine(gridParent.transform, $"GridLine_H_{z}", lineMaterial,
                new Vector3(centerOffset.x, 0.05f, z * gridSize + centerOffset.z),
                new Vector3(gridWidth * gridSize + centerOffset.x, 0.05f, z * gridSize + centerOffset.z));
        }
    }
    
    void CreateGridLine(Transform parent, string name, Material material, Vector3 start, Vector3 end)
    {
        var line = new GameObject(name);
        line.transform.SetParent(parent);
        var lr = line.AddComponent<LineRenderer>();
        lr.material = material;
        lr.startWidth = lineWidth;
        lr.endWidth = lineWidth;
        lr.positionCount = 2;
        lr.useWorldSpace = true;
        lr.SetPosition(0, start);
        lr.SetPosition(1, end);
    }
    
    // Snap position to grid
    public Vector3 SnapToGrid(Vector3 worldPosition)
    {
        float snappedX = Mathf.Round((worldPosition.x - centerOffset.x) / gridSize) * gridSize + centerOffset.x;
        float snappedZ = Mathf.Round((worldPosition.z - centerOffset.z) / gridSize) * gridSize + centerOffset.z;
        
        return new Vector3(snappedX, worldPosition.y, snappedZ);
    }
    
    // Check if a grid position is available for building
    public bool IsGridPositionAvailable(Vector3 gridPosition)
    {
        Collider[] colliders = Physics.OverlapBox(
            gridPosition + Vector3.up * 0.5f,
            new Vector3(gridSize * 0.4f, 1f, gridSize * 0.4f),
            Quaternion.identity,
            buildingLayer
        );
        
        return colliders.Length == 0;
    }
    
    // Get grid coordinates from world position
    public Vector2Int GetGridCoordinates(Vector3 worldPosition)
    {
        int x = Mathf.RoundToInt((worldPosition.x - centerOffset.x) / gridSize);
        int z = Mathf.RoundToInt((worldPosition.z - centerOffset.z) / gridSize);
        
        return new Vector2Int(x, z);
    }
    
    // Convert grid coordinates to world position
    public Vector3 GridToWorldPosition(Vector2Int gridCoords)
    {
        return new Vector3(
            gridCoords.x * gridSize + centerOffset.x,
            0,
            gridCoords.y * gridSize + centerOffset.z
        );
    }
    
    // Toggle grid visibility
    public void ToggleGridVisibility()
    {
        showGrid = !showGrid;
        if (gridParent != null)
            gridParent.SetActive(showGrid);
    }
    
    void OnDrawGizmosSelected()
    {
        // Draw grid bounds in editor
        Gizmos.color = Color.yellow;
        Vector3 size = new Vector3(gridWidth * gridSize, 0.1f, gridHeight * gridSize);
        Vector3 center = new Vector3(0, 0, 0);
        Gizmos.DrawWireCube(center, size);
    }
}
