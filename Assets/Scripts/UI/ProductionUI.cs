using UnityEngine;
using UnityEngine.UI;

public class ProductionUI : MonoBehaviour
{
    [Header("UI Elements")]
    public Button workerButton;
    public Button soldierButton;
    public Button closeButton;
    public Text titleText;
    public Text resourcesText;
    
    [Header("Unit Costs")]
    public int workerCost = 50;
    public int soldierCost = 100;
    
    private BuildingClickHandler buildingHandler;
    private BuildingClickHandler.BuildingType buildingType;
    private int playerIndex;
    private GameManager gameManager;
    private ResourceManager resourceManager;
    
    public void Initialize(BuildingClickHandler handler, BuildingClickHandler.BuildingType type, int player)
    {
        buildingHandler = handler;
        buildingType = type;
        playerIndex = player;
        gameManager = FindFirstObjectByType<GameManager>();
        
        var playerCapital = gameManager.playerCapitals[playerIndex];
        if (playerCapital != null)
        {
            resourceManager = playerCapital.GetComponent<ResourceManager>();
            if (resourceManager == null)
            {
                resourceManager = playerCapital.AddComponent<ResourceManager>();
                resourceManager.playerIndex = playerIndex;
            }
        }
        
        SetupUI();
        UpdateResourceDisplay();
    }
    
    void SetupUI()
    {
        if (titleText != null)
        {
            titleText.text = $"{buildingType} - Player {playerIndex + 1}";
        }
        
        if (workerButton != null)
        {
            workerButton.onClick.AddListener(() => OnUnitButtonClicked("worker"));
            UpdateButtonState(workerButton, workerCost);
        }
        
        if (soldierButton != null)
        {
            soldierButton.onClick.AddListener(() => OnUnitButtonClicked("soldier"));
            UpdateButtonState(soldierButton, soldierCost);
        }
        
        if (closeButton != null)
        {
            closeButton.onClick.AddListener(OnCloseButtonClicked);
        }
    }
    
    public void CreateSimpleUI()
    {
        var rectTransform = gameObject.AddComponent<RectTransform>();
        rectTransform.sizeDelta = new Vector2(300, 200);
        
        // Background
        var background = gameObject.AddComponent<Image>();
        background.color = new Color(0.1f, 0.1f, 0.1f, 0.9f);
        
        // Title
        var titleObj = new GameObject("Title");
        titleObj.transform.SetParent(transform, false);
        titleText = titleObj.AddComponent<Text>();
        titleText.text = "Production";
        titleText.font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
        titleText.fontSize = 18;
        titleText.color = Color.white;
        titleText.alignment = TextAnchor.MiddleCenter;
        
        var titleRect = titleObj.GetComponent<RectTransform>();
        titleRect.anchorMin = new Vector2(0, 0.8f);
        titleRect.anchorMax = new Vector2(1, 1);
        titleRect.offsetMin = Vector2.zero;
        titleRect.offsetMax = Vector2.zero;
        
        workerButton = CreateButton("WorkerButton", "Worker (50 Gold)", new Vector2(0, 0.5f), new Vector2(1, 0.8f));
        
        soldierButton = CreateButton("SoldierButton", "Soldier (100 Gold)", new Vector2(0, 0.2f), new Vector2(1, 0.5f));
        
        closeButton = CreateButton("CloseButton", "Close", new Vector2(0, 0), new Vector2(1, 0.2f));
        closeButton.GetComponent<Image>().color = new Color(0.8f, 0.2f, 0.2f, 1f);
        
        var resourcesObj = new GameObject("Resources");
        resourcesObj.transform.SetParent(transform, false);
        resourcesText = resourcesObj.AddComponent<Text>();
        resourcesText.text = "Resources: Loading...";
        resourcesText.font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
        resourcesText.fontSize = 12;
        resourcesText.color = Color.yellow;
        resourcesText.alignment = TextAnchor.MiddleCenter;
        
        var resourcesRect = resourcesObj.GetComponent<RectTransform>();
        resourcesRect.anchorMin = new Vector2(0, 0.8f);
        resourcesRect.anchorMax = new Vector2(1, 1f);
        resourcesRect.offsetMin = new Vector2(0, -20);
        resourcesRect.offsetMax = Vector2.zero;
    }
    
    Button CreateButton(string name, string text, Vector2 anchorMin, Vector2 anchorMax)
    {
        var buttonObj = new GameObject(name);
        buttonObj.transform.SetParent(transform, false);
        
        var button = buttonObj.AddComponent<Button>();
        var image = buttonObj.AddComponent<Image>();
        image.color = new Color(0.2f, 0.3f, 0.5f, 1f);
        
        var textObj = new GameObject("Text");
        textObj.transform.SetParent(buttonObj.transform, false);
        var buttonText = textObj.AddComponent<Text>();
        buttonText.text = text;
        buttonText.font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
        buttonText.fontSize = 14;
        buttonText.color = Color.white;
        buttonText.alignment = TextAnchor.MiddleCenter;
        
        var buttonRect = buttonObj.GetComponent<RectTransform>();
        buttonRect.anchorMin = anchorMin;
        buttonRect.anchorMax = anchorMax;
        buttonRect.offsetMin = new Vector2(10, 5);
        buttonRect.offsetMax = new Vector2(-10, -5);
        
        var textRect = textObj.GetComponent<RectTransform>();
        textRect.anchorMin = Vector2.zero;
        textRect.anchorMax = Vector2.one;
        textRect.offsetMin = Vector2.zero;
        textRect.offsetMax = Vector2.zero;
        
        return button;
    }
    
    void UpdateResourceDisplay()
    {
        if (resourcesText == null) return;
        
        if (resourceManager != null)
        {
            resourcesText.text = resourceManager.GetResourceString();
        }
        else
        {
            resourcesText.text = "Resources: Loading...";
        }
    }
    
    void UpdateButtonState(Button button, int cost)
    {
        if (button == null) return;
        
        bool canAfford = true;
        if (resourceManager != null)
        {
            canAfford = resourceManager.CanAfford(gold: cost);
        }
        
        button.interactable = canAfford;
        
        var buttonImage = button.GetComponent<Image>();
        if (buttonImage != null)
        {
            buttonImage.color = canAfford ? 
                new Color(0.2f, 0.3f, 0.5f, 1f) : 
                new Color(0.5f, 0.2f, 0.2f, 1f);
        }
    }
    
    void OnUnitButtonClicked(string unitType)
    {
        Debug.Log($"Producing {unitType} for Player {playerIndex + 1}");
        
        int cost = (unitType == "worker") ? workerCost : soldierCost;
        
        if (resourceManager != null && resourceManager.SpendResources(gold: cost))
        {
            if (buildingHandler != null)
            {
                buildingHandler.ProduceUnit(unitType);
            }
        }
        else
        {
            Debug.LogWarning($"Cannot afford {unitType} (Cost: {cost} gold)");
        }
        
        UpdateResourceDisplay();
        UpdateButtonState(workerButton, workerCost);
        UpdateButtonState(soldierButton, soldierCost);
    }
    
    void OnCloseButtonClicked()
    {
        if (buildingHandler != null)
        {
            buildingHandler.CloseProductionUI();
        }
    }
    
    void Update()
    {
        UpdateResourceDisplay();
    }
}
