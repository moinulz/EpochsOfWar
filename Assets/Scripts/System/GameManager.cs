using UnityEngine;
using UnityEngine.InputSystem;

public class GameManager : MonoBehaviour
{
    [Header("Game Objects")]
    public GameObject capitalBuilding;
    public Camera gameCamera;
    public CameraController cameraController;
    
    [Header("Game Settings")]
    public bool isPaused = false;
    public GameSettings currentGameSettings;
    
    [Header("Managers")]
    public MapManager mapManager;
    public TerrainManager terrainManager;
    
    [Header("Player Management")]
    public GameObject[] playerCapitals;
    
    void Start()
    {
        // Load game settings from the setup
        if (GameSettingsManager.CurrentGameSettings != null)
        {
            currentGameSettings = GameSettingsManager.CurrentGameSettings;
            InitializeGameFromSettings();
        }
        else
        {
            // Default single-player setup
            currentGameSettings = new GameSettings();
            InitializeDefaultGame();
        }
        
        InitializeComponents();
    }
    void InitializeComponents()
    {
        // Initialize game components
        if (gameCamera == null)
            gameCamera = Camera.main;
            
        if (cameraController == null && gameCamera != null)
            cameraController = gameCamera.GetComponent<CameraController>();
            
        // Find managers
        if (mapManager == null)
            mapManager = FindFirstObjectByType<MapManager>();
            
        if (terrainManager == null)
            terrainManager = FindFirstObjectByType<TerrainManager>();
        
        Debug.Log("Game Manager initialized!");
        Debug.Log($"Game Settings: {currentGameSettings != null}");
        Debug.Log($"Map Manager: {mapManager != null}");
        Debug.Log($"Camera controller: {cameraController != null}");
    }
    
    void InitializeGameFromSettings()
    {
        Debug.Log($"Initializing game with settings - Mode: {currentGameSettings.gameMode}");
        Debug.Log($"Map: {currentGameSettings.mapName}, Max Players: {currentGameSettings.maxPlayers}");
        
        // Setup players and spawn capitals
        SetupPlayersAndCapitals();
    }
    
    void InitializeDefaultGame()
    {
        Debug.Log("Initializing default single-player game");
        
        // Find existing capital if any
        if (capitalBuilding == null)
        {
            var capital = GameObject.Find("Capital");
            if (capital != null)
                capitalBuilding = capital;
        }
    }
    
    void SetupPlayersAndCapitals()
    {
        if (mapManager == null) return;
        
        // Assign random spawn points
        mapManager.AssignRandomSpawnPoints(currentGameSettings);
        
        // Create capitals for active players
        playerCapitals = new GameObject[currentGameSettings.maxPlayers];
        
        for (int i = 0; i < currentGameSettings.players.Length; i++)
        {
            var player = currentGameSettings.players[i];
            if (player.isActive && player.playerType != PlayerType.Disabled)
            {
                CreatePlayerCapital(i, player);
            }
        }
    }
    
    void CreatePlayerCapital(int playerIndex, PlayerSetup playerSetup)
    {
        Vector3 spawnPosition = mapManager.GetSpawnPosition(playerIndex);
        
        // Look for existing capital prefab in the scene first
        var existingCapital = GameObject.Find("Capital");
        if (existingCapital != null && playerIndex == 0) // Use existing capital for first player
        {
            existingCapital.transform.position = spawnPosition + Vector3.up * 5f;
            existingCapital.name = $"Capital_Player_{playerIndex + 1}";
            
            // Set player color
            var renderer = existingCapital.GetComponent<Renderer>();
            if (renderer != null)
            {
                var material = new Material(Shader.Find("Standard"));
                material.color = playerSetup.playerColor;
                renderer.material = material;
            }
            
            playerCapitals[playerIndex] = existingCapital;
            
            if (playerSetup.playerType == PlayerType.Human)
            {
                capitalBuilding = existingCapital;
            }
        }
        else
        {
            // Create a simple placeholder capital
            GameObject capital = GameObject.CreatePrimitive(PrimitiveType.Cube);
            capital.name = $"Capital_Player_{playerIndex + 1}";
            capital.transform.position = spawnPosition + Vector3.up * 5f;
            capital.transform.localScale = new Vector3(10, 10, 10);
            
            // Set player color
            var material = new Material(Shader.Find("Standard"));
            material.color = playerSetup.playerColor;
            capital.GetComponent<Renderer>().material = material;
            
            playerCapitals[playerIndex] = capital;
            
            // If this is the human player, set as the main capital for camera focus
            if (playerSetup.playerType == PlayerType.Human)
            {
                capitalBuilding = capital;
            }
        }
        
        Debug.Log($"Created capital for Player {playerIndex + 1} ({playerSetup.playerType}) at {spawnPosition}");
    }
    
    void Update()
    {
        // Handle pause with Escape key using new Input System
        Keyboard keyboard = Keyboard.current;
        if (keyboard != null && keyboard.escapeKey.wasPressedThisFrame)
        {
            TogglePause();
        }
    }
    
    public void TogglePause()
    {
        isPaused = !isPaused;
        Time.timeScale = isPaused ? 0f : 1f;
        Debug.Log($"Game {(isPaused ? "Paused" : "Resumed")}");
    }
    
    public void ReturnToMainMenu()
    {
        Time.timeScale = 1f; // Reset time scale
        UnityEngine.SceneManagement.SceneManager.LoadScene("MainMenu");
    }
    
    // Called from UI buttons
    public void OnPauseButtonClicked()
    {
        TogglePause();
    }
    
    public void OnMainMenuButtonClicked()
    {
        ReturnToMainMenu();
    }
}
