using UnityEngine;

public class GameManager : MonoBehaviour
{
    [Header("Game Objects")]
    public GameObject capitalBuilding;
    public Camera gameCamera;
    public CameraController cameraController;
    
    [Header("Game Settings")]
    public bool isPaused = false;
    
    void Start()
    {
        // Initialize game
        if (gameCamera == null)
            gameCamera = Camera.main;
            
        if (cameraController == null && gameCamera != null)
            cameraController = gameCamera.GetComponent<CameraController>();
            
        // Find capital if not assigned
        if (capitalBuilding == null)
        {
            var capital = GameObject.Find("Capital");
            if (capital != null)
                capitalBuilding = capital;
        }
        
        Debug.Log("Game Manager initialized!");
        Debug.Log($"Capital found: {capitalBuilding != null}");
        Debug.Log($"Camera controller: {cameraController != null}");
    }
    
    void Update()
    {
        // Handle pause with Escape key
        if (Input.GetKeyDown(KeyCode.Escape))
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
