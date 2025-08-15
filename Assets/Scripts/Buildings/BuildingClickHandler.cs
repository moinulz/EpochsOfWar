using UnityEngine;
using UnityEngine.InputSystem;

public class BuildingClickHandler : MonoBehaviour
{
    [Header("Building Properties")]
    public BuildingType buildingType = BuildingType.Capital;
    public int playerIndex = 0;
    
    [Header("UI References")]
    public GameObject productionUIPrefab;
    
    private ProductionUI currentProductionUI;
    private Camera playerCamera;
    private bool isSelected = false;
    
    public enum BuildingType
    {
        Capital,
        Barracks,
        DefenseTower,
        Storage,
        Farm
    }
    
    void Start()
    {
        playerCamera = Camera.main;
        if (playerCamera == null)
            playerCamera = FindFirstObjectByType<Camera>();
    }
    
    void Update()
    {
        HandleInput();
    }
    
    void HandleInput()
    {
        Mouse mouse = Mouse.current;
        if (mouse != null && mouse.leftButton.wasPressedThisFrame)
        {
            CheckForClick();
        }
        
        Keyboard keyboard = Keyboard.current;
        if (keyboard != null && keyboard.escapeKey.wasPressedThisFrame)
        {
            CloseProductionUI();
        }
    }
    
    void CheckForClick()
    {
        if (playerCamera == null) return;
        
        Vector2 mousePosition = Mouse.current.position.ReadValue();
        Ray ray = playerCamera.ScreenPointToRay(mousePosition);
        
        if (Physics.Raycast(ray, out RaycastHit hit))
        {
            if (hit.collider.gameObject == gameObject)
            {
                OnBuildingClicked();
            }
            else
            {
                CloseProductionUI();
            }
        }
    }
    
    public void OnBuildingClicked()
    {
        var gameManager = FindFirstObjectByType<GameManager>();
        if (gameManager != null && gameManager.currentGameSettings != null)
        {
            var humanPlayerIndex = GetHumanPlayerIndex(gameManager);
            if (playerIndex != humanPlayerIndex)
            {
                Debug.Log($"Cannot interact with Player {playerIndex + 1}'s building");
                return;
            }
        }
        
        Debug.Log($"Building clicked: {buildingType} (Player {playerIndex + 1})");
        
        if (isSelected)
        {
            CloseProductionUI();
        }
        else
        {
            ShowProductionUI();
        }
    }
    
    int GetHumanPlayerIndex(GameManager gameManager)
    {
        for (int i = 0; i < gameManager.currentGameSettings.players.Length; i++)
        {
            if (gameManager.currentGameSettings.players[i].playerType == PlayerType.Human)
            {
                return i;
            }
        }
        return 0; // Default to first player
    }
    
    void ShowProductionUI()
    {
        CloseProductionUI(); // Close any existing UI first
        
        if (productionUIPrefab != null)
        {
            var uiCanvas = FindFirstObjectByType<Canvas>();
            if (uiCanvas != null)
            {
                var uiObject = Instantiate(productionUIPrefab, uiCanvas.transform);
                currentProductionUI = uiObject.GetComponent<ProductionUI>();
                
                if (currentProductionUI != null)
                {
                    currentProductionUI.Initialize(this, buildingType, playerIndex);
                    isSelected = true;
                    
                    PositionUI();
                }
            }
        }
        else
        {
            CreateSimpleProductionUI();
        }
    }
    
    void PositionUI()
    {
        if (currentProductionUI == null || playerCamera == null) return;
        
        Vector3 screenPos = playerCamera.WorldToScreenPoint(transform.position);
        
        screenPos.x += 200f;
        screenPos.y = Mathf.Clamp(screenPos.y, 100f, Screen.height - 200f);
        
        var rectTransform = currentProductionUI.GetComponent<RectTransform>();
        if (rectTransform != null)
        {
            rectTransform.position = screenPos;
        }
    }
    
    void CreateSimpleProductionUI()
    {
        var uiCanvas = FindFirstObjectByType<Canvas>();
        if (uiCanvas == null) return;
        
        var uiObject = new GameObject("ProductionUI_Simple");
        uiObject.transform.SetParent(uiCanvas.transform, false);
        
        var productionUI = uiObject.AddComponent<ProductionUI>();
        productionUI.CreateSimpleUI();
        productionUI.Initialize(this, buildingType, playerIndex);
        
        currentProductionUI = productionUI;
        isSelected = true;
        
        PositionUI();
    }
    
    public void CloseProductionUI()
    {
        if (currentProductionUI != null)
        {
            Destroy(currentProductionUI.gameObject);
            currentProductionUI = null;
        }
        isSelected = false;
    }
    
    public void ProduceUnit(string unitType)
    {
        var unitSpawner = GetComponent<UnitSpawner>();
        if (unitSpawner != null)
        {
            switch (unitType.ToLower())
            {
                case "worker":
                    unitSpawner.SpawnWorker();
                    break;
                case "soldier":
                    unitSpawner.SpawnSoldier();
                    break;
                default:
                    Debug.LogWarning($"Unknown unit type: {unitType}");
                    break;
            }
        }
        else
        {
            Debug.LogWarning("No UnitSpawner component found on building");
        }
    }
    
    void OnDestroy()
    {
        CloseProductionUI();
    }
}
