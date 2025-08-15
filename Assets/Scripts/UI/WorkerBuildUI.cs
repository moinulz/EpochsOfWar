using UnityEngine;
using UnityEngine.UI;

public class WorkerBuildUI : MonoBehaviour
{
    [Header("UI Elements")]
    public Button barracksButton;
    public Button towerButton;
    public Button wallButton;
    public Button farmButton;
    public Button closeButton;
    
    [Header("Building Prefabs")]
    public GameObject barracksPrefab;
    public GameObject towerPrefab;
    public GameObject wallPrefab;
    public GameObject farmPrefab;
    
    [Header("Building Costs")]
    public int barracksCost = 200;
    public int towerCost = 150;
    public int wallCost = 50;
    public int farmCost = 100;
    
    private Unit selectedWorker;
    private BuildingPlacer buildingPlacer;
    private ResourceManager resourceManager;
    
    public void Initialize(Unit worker)
    {
        selectedWorker = worker;
        buildingPlacer = FindFirstObjectByType<BuildingPlacer>();
        
        var gameManager = FindFirstObjectByType<GameManager>();
        if (gameManager != null && gameManager.playerCapitals[worker.playerIndex] != null)
        {
            resourceManager = gameManager.playerCapitals[worker.playerIndex].GetComponent<ResourceManager>();
        }
        
        SetupUI();
        UpdateButtonStates();
    }
    
    void SetupUI()
    {
        if (barracksButton != null)
            barracksButton.onClick.AddListener(() => StartBuilding(barracksPrefab, barracksCost));
        
        if (towerButton != null)
            towerButton.onClick.AddListener(() => StartBuilding(towerPrefab, towerCost));
        
        if (wallButton != null)
            wallButton.onClick.AddListener(() => StartBuilding(wallPrefab, wallCost));
        
        if (farmButton != null)
            farmButton.onClick.AddListener(() => StartBuilding(farmPrefab, farmCost));
        
        if (closeButton != null)
            closeButton.onClick.AddListener(CloseUI);
    }
    
    void UpdateButtonStates()
    {
        if (resourceManager == null) return;
        
        UpdateButtonState(barracksButton, barracksCost);
        UpdateButtonState(towerButton, towerCost);
        UpdateButtonState(wallButton, wallCost);
        UpdateButtonState(farmButton, farmCost);
    }
    
    void UpdateButtonState(Button button, int cost)
    {
        if (button == null) return;
        
        bool canAfford = resourceManager.CanAfford(wood: cost);
        button.interactable = canAfford;
        
        var buttonImage = button.GetComponent<Image>();
        if (buttonImage != null)
        {
            buttonImage.color = canAfford ? 
                new Color(0.2f, 0.5f, 0.3f, 1f) : 
                new Color(0.5f, 0.2f, 0.2f, 1f);
        }
    }
    
    void StartBuilding(GameObject buildingPrefab, int cost)
    {
        if (buildingPlacer == null || resourceManager == null) return;
        
        if (resourceManager.CanAfford(wood: cost))
        {
            buildingPlacer.StartBuildingPlacement(buildingPrefab);
            CloseUI();
        }
        else
        {
            Debug.LogWarning($"Cannot afford building (Cost: {cost} wood)");
        }
    }
    
    void CloseUI()
    {
        Destroy(gameObject);
    }
}
