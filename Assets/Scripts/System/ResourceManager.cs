using UnityEngine;

public class ResourceManager : MonoBehaviour
{
    [Header("Current Resources")]
    public Resources currentResources;
    public int playerIndex = 0;
    
    [Header("UI Updates")]
    public UnityEngine.Events.UnityEvent OnResourcesChanged;
    
    private GameManager gameManager;
    
    void Start()
    {
        gameManager = FindFirstObjectByType<GameManager>();
        InitializeResources();
    }
    
    void InitializeResources()
    {
        if (gameManager != null && gameManager.currentGameSettings != null)
        {
            if (playerIndex < gameManager.currentGameSettings.players.Length)
            {
                var playerSetup = gameManager.currentGameSettings.players[playerIndex];
                currentResources = new Resources(
                    playerSetup.startingResources.wood,
                    playerSetup.startingResources.stone,
                    playerSetup.startingResources.iron,
                    playerSetup.startingResources.gold
                );
            }
        }
        
        if (currentResources == null)
        {
            currentResources = new Resources(500, 500, 500, 500);
        }
        
        OnResourcesChanged?.Invoke();
    }
    
    public bool CanAfford(int wood = 0, int stone = 0, int iron = 0, int gold = 0)
    {
        return currentResources.wood >= wood &&
               currentResources.stone >= stone &&
               currentResources.iron >= iron &&
               currentResources.gold >= gold;
    }
    
    public bool SpendResources(int wood = 0, int stone = 0, int iron = 0, int gold = 0)
    {
        if (!CanAfford(wood, stone, iron, gold))
        {
            Debug.LogWarning($"Insufficient resources! Need: W{wood} S{stone} I{iron} G{gold}");
            return false;
        }
        
        currentResources.wood -= wood;
        currentResources.stone -= stone;
        currentResources.iron -= iron;
        currentResources.gold -= gold;
        
        OnResourcesChanged?.Invoke();
        Debug.Log($"Spent resources: W{wood} S{stone} I{iron} G{gold}");
        return true;
    }
    
    public void AddResources(int wood = 0, int stone = 0, int iron = 0, int gold = 0)
    {
        currentResources.wood += wood;
        currentResources.stone += stone;
        currentResources.iron += iron;
        currentResources.gold += gold;
        
        OnResourcesChanged?.Invoke();
        Debug.Log($"Gained resources: W{wood} S{stone} I{iron} G{gold}");
    }
    
    public string GetResourceString()
    {
        return $"Wood: {currentResources.wood} | Stone: {currentResources.stone} | Iron: {currentResources.iron} | Gold: {currentResources.gold}";
    }
}
