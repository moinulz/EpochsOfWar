using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

public class SkirmishSetup : MonoBehaviour
{
    [Header("Game Settings")]
    public GameSettings gameSettings;
    
    [Header("Map Selection")]
    public Dropdown mapDropdown;
    public Text mapInfoText;
    
    [Header("Player Setup")]
    public Transform playerSetupParent;
    public GameObject playerSetupPrefab;
    private List<PlayerSetupUI> playerSetupUIs = new List<PlayerSetupUI>();
    
    [Header("Victory Conditions")]
    public Dropdown victoryDropdown;
    
    [Header("Resource Settings")]
    public Toggle customResourcesToggle;
    public GameObject resourceSettingsPanel;
    
    [Header("Action Buttons")]
    public Button startGameButton;
    public Button backButton;
    
    void Start()
    {
        if (gameSettings == null)
            gameSettings = new GameSettings();
            
        SetupUI();
    }
    
    public void InitializeSetup()
    {
        if (gameSettings == null)
            gameSettings = new GameSettings();
            
        SetupUI();
        RefreshUI();
    }
    
    void SetupUI()
    {
        // Setup map dropdown
        if (mapDropdown)
        {
            mapDropdown.ClearOptions();
            mapDropdown.AddOptions(new List<string> { "Default Map (4 Players)", "Small Map (2 Players)", "Large Map (6 Players)" });
            mapDropdown.onValueChanged.AddListener(OnMapChanged);
        }
        
        // Setup victory conditions dropdown
        if (victoryDropdown)
        {
            victoryDropdown.ClearOptions();
            var victoryOptions = new List<string>
            {
                "All Capitals Destroyed",
                "All Units Destroyed", 
                "All Opponents Surrender",
                "Custom Conditions"
            };
            victoryDropdown.AddOptions(victoryOptions);
            victoryDropdown.onValueChanged.AddListener(OnVictoryConditionChanged);
        }
        
        // Setup custom resources toggle
        if (customResourcesToggle)
        {
            customResourcesToggle.onValueChanged.AddListener(OnCustomResourcesToggled);
        }
        
        // Setup action buttons
        if (startGameButton) startGameButton.onClick.AddListener(StartSkirmishGame);
        if (backButton) backButton.onClick.AddListener(GoBack);
        
        CreatePlayerSetupUIs();
    }
    
    void CreatePlayerSetupUIs()
    {
        // Clear existing UIs
        foreach (var ui in playerSetupUIs)
        {
            if (ui != null && ui.gameObject != null)
                DestroyImmediate(ui.gameObject);
        }
        playerSetupUIs.Clear();
        
        // Create new player setup UIs
        for (int i = 0; i < gameSettings.maxPlayers; i++)
        {
            CreatePlayerSetupUI(i);
        }
    }
    
    void CreatePlayerSetupUI(int playerIndex)
    {
        if (playerSetupPrefab == null || playerSetupParent == null) return;
        
        var playerUI = Instantiate(playerSetupPrefab, playerSetupParent);
        var playerSetupUI = playerUI.GetComponent<PlayerSetupUI>();
        
        if (playerSetupUI == null)
            playerSetupUI = playerUI.AddComponent<PlayerSetupUI>();
        
        playerSetupUI.Initialize(playerIndex, gameSettings.players[playerIndex], this);
        playerSetupUIs.Add(playerSetupUI);
    }
    
    void RefreshUI()
    {
        UpdateMapInfo();
        UpdatePlayerSetups();
        UpdateResourceSettings();
    }
    
    void UpdateMapInfo()
    {
        if (mapInfoText)
        {
            string mapInfo = $"Map: {gameSettings.mapName}\\nMax Players: {gameSettings.maxPlayers}\\nSpawn Points: Random";
            mapInfoText.text = mapInfo;
        }
    }
    
    void UpdatePlayerSetups()
    {
        foreach (var ui in playerSetupUIs)
        {
            if (ui != null)
                ui.RefreshUI();
        }
    }
    
    void UpdateResourceSettings()
    {
        if (resourceSettingsPanel)
            resourceSettingsPanel.SetActive(gameSettings.customResources);
    }
    
    void OnMapChanged(int value)
    {
        switch (value)
        {
            case 0: // Default Map
                gameSettings.mapName = "DefaultMap";
                gameSettings.maxPlayers = 4;
                break;
            case 1: // Small Map
                gameSettings.mapName = "SmallMap";
                gameSettings.maxPlayers = 2;
                break;
            case 2: // Large Map
                gameSettings.mapName = "LargeMap";
                gameSettings.maxPlayers = 6;
                break;
        }
        
        // Recreate player setups for new player count
        CreatePlayerSetupUIs();
        RefreshUI();
    }
    
    void OnVictoryConditionChanged(int value)
    {
        gameSettings.victoryCondition = (VictoryCondition)value;
    }
    
    void OnCustomResourcesToggled(bool enabled)
    {
        gameSettings.customResources = enabled;
        UpdateResourceSettings();
    }
    
    public void OnPlayerSettingsChanged(int playerIndex, PlayerSetup newSettings)
    {
        if (playerIndex >= 0 && playerIndex < gameSettings.players.Length)
        {
            gameSettings.players[playerIndex] = newSettings;
        }
    }
    
    void StartSkirmishGame()
    {
        // Validate settings
        if (!ValidateGameSettings())
        {
            Debug.LogWarning("Invalid game settings!");
            return;
        }
        
        // Save game settings to a singleton or PlayerPrefs for the game scene to use
        GameSettingsManager.CurrentGameSettings = gameSettings;
        
        // Load the game scene
        UnityEngine.SceneManagement.SceneManager.LoadScene("Game");
    }
    
    bool ValidateGameSettings()
    {
        // Check if at least one human player is active
        bool hasHumanPlayer = false;
        int activePlayers = 0;
        
        foreach (var player in gameSettings.players)
        {
            if (player.isActive && player.playerType != PlayerType.Disabled)
            {
                activePlayers++;
                if (player.playerType == PlayerType.Human)
                    hasHumanPlayer = true;
            }
        }
        
        if (!hasHumanPlayer)
        {
            Debug.LogWarning("At least one human player must be active!");
            return false;
        }
        
        if (activePlayers < 2)
        {
            Debug.LogWarning("At least 2 players must be active for a game!");
            return false;
        }
        
        return true;
    }
    
    void GoBack()
    {
        var gameModeMenu = FindFirstObjectByType<GameModeMenu>();
        if (gameModeMenu != null)
        {
            gameModeMenu.ShowMainGameModeSelection();
        }
    }
}

// Singleton to hold game settings between scenes
public static class GameSettingsManager
{
    public static GameSettings CurrentGameSettings;
}
