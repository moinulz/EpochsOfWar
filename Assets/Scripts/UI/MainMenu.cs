using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using EpochsOfWar.UI;

namespace EpochsOfWar.UI
{
    /// <summary>
    /// Main menu controller with proper scaling and reliable scene transitions
    /// </summary>
    public class MainMenu : MonoBehaviour
    {
        [Header("Main Buttons")]
        [SerializeField] private Button playButton;
        [SerializeField] private Button settingsButton;
        [SerializeField] private Button quitButton;
        [SerializeField] private Button quickSkirmishButton;

        [Header("Settings Panel")]
        [SerializeField] private GameObject settingsPanel;
        [SerializeField] private Button closeSettingsButton;

        [Header("Scene Transition")]
        [SerializeField] private SceneLoader sceneLoader;

        [Header("Auto-Setup")]
        [SerializeField] private bool autoFindComponents = true;

        void Awake()
        {
            if (autoFindComponents)
            {
                AutoFindComponents();
            }
        }

        void Start()
        {
            SetupButtonListeners();
            EnsureSettingsPanel();
            HideSettings();
            Debug.Log("MainMenu initialized with proper scaling");
        }

        void AutoFindComponents()
        {
            // Auto-find buttons if not assigned
            if (!playButton) playButton = FindButtonByName("PlayButton");
            if (!settingsButton) settingsButton = FindButtonByName("SettingsButton");
            if (!quitButton) quitButton = FindButtonByName("QuitButton");
            if (!quickSkirmishButton) quickSkirmishButton = FindButtonByName("QuickSkirmishButton");

            // Auto-find settings panel
            if (!settingsPanel) settingsPanel = GameObject.Find("SettingsPanel");
            if (!closeSettingsButton) closeSettingsButton = FindButtonByName("CloseButton");

            // Auto-find scene loader
            if (!sceneLoader) sceneLoader = FindObjectOfType<SceneLoader>();
        }

        Button FindButtonByName(string buttonName)
        {
            var buttonObj = GameObject.Find(buttonName);
            return buttonObj ? buttonObj.GetComponent<Button>() : null;
        }

        void SetupButtonListeners()
        {
            // Main menu buttons
            if (playButton) 
            {
                playButton.onClick.RemoveAllListeners();
                playButton.onClick.AddListener(OnPlay);
            }

            if (settingsButton) 
            {
                settingsButton.onClick.RemoveAllListeners();
                settingsButton.onClick.AddListener(ShowSettings);
            }

            if (quitButton) 
            {
                quitButton.onClick.RemoveAllListeners();
                quitButton.onClick.AddListener(OnQuit);
                
                // Hide quit button on mobile platforms
                #if UNITY_ANDROID || UNITY_IOS
                quitButton.gameObject.SetActive(false);
                #endif
            }

            if (quickSkirmishButton) 
            {
                quickSkirmishButton.onClick.RemoveAllListeners();
                quickSkirmishButton.onClick.AddListener(OnQuickSkirmish);
            }

            // Settings panel buttons
            if (closeSettingsButton)
            {
                closeSettingsButton.onClick.RemoveAllListeners();
                closeSettingsButton.onClick.AddListener(HideSettings);
            }
        }

        void EnsureSettingsPanel()
        {
            if (!settingsPanel)
            {
                CreateBasicSettingsPanel();
            }
        }

        void OnPlay()
        {
            Debug.Log("Loading Game Mode Selection...");
            LoadScene("GameModeSelection");
        }

        void OnQuickSkirmish()
        {
            Debug.Log("Starting Quick Skirmish (1v1 Human vs AI)...");
            
            // Set up quick skirmish game settings
            var gameSettings = new GameSettings 
            { 
                gameMode = GameMode.Skirmish, 
                mapName = "DefaultMap", 
                maxPlayers = 2 
            };
            
            gameSettings.players = new PlayerSetup[2] 
            {
                new PlayerSetup { playerType = PlayerType.Human, isActive = true, team = 0 },
                new PlayerSetup { playerType = PlayerType.AI, isActive = true, team = 1 }
            };
            
            // Store settings for the game scene
            if (GameSettingsManager.Instance != null)
            {
                GameSettingsManager.CurrentGameSettings = gameSettings;
            }
            
            LoadScene("Game");
        }

        void ShowSettings()
        {
            Debug.Log("Showing settings panel");
            if (settingsPanel) 
            {
                settingsPanel.SetActive(true);
            }
        }

        void HideSettings()
        {
            if (settingsPanel) 
            {
                settingsPanel.SetActive(false);
            }
        }

        void OnQuit()
        {
            Debug.Log("Quitting application");
            
            #if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
            #else
            Application.Quit();
            #endif
        }

        void LoadScene(string sceneName)
        {
            if (sceneLoader != null)
            {
                sceneLoader.LoadSceneAsync(sceneName);
            }
            else
            {
                SceneManager.LoadScene(sceneName);
            }
        }

        void CreateBasicSettingsPanel()
        {
            var canvas = FindObjectOfType<Canvas>();
            if (!canvas) return;

            // Create settings panel
            settingsPanel = new GameObject("SettingsPanel", typeof(RectTransform), typeof(Image));
            settingsPanel.transform.SetParent(canvas.transform, false);
            
            var image = settingsPanel.GetComponent<Image>();
            image.color = new Color(0, 0, 0, 0.8f);
            
            var rect = settingsPanel.GetComponent<RectTransform>();
            rect.anchorMin = Vector2.zero;
            rect.anchorMax = Vector2.one;
            rect.offsetMin = Vector2.zero;
            rect.offsetMax = Vector2.zero;

            // Create content panel
            var contentPanel = new GameObject("Content", typeof(RectTransform), typeof(Image));
            contentPanel.transform.SetParent(settingsPanel.transform, false);
            
            var contentImage = contentPanel.GetComponent<Image>();
            contentImage.color = new Color(0.2f, 0.2f, 0.3f, 1f);
            
            var contentRect = contentPanel.GetComponent<RectTransform>();
            contentRect.anchorMin = new Vector2(0.2f, 0.2f);
            contentRect.anchorMax = new Vector2(0.8f, 0.8f);
            contentRect.offsetMin = Vector2.zero;
            contentRect.offsetMax = Vector2.zero;

            // Add title
            var title = MenuBootstrapper.CreateScaledText(contentPanel.transform, "Title", "SETTINGS", 32);
            var titleRect = title.GetComponent<RectTransform>();
            titleRect.anchorMin = new Vector2(0, 0.8f);
            titleRect.anchorMax = new Vector2(1, 0.95f);
            titleRect.offsetMin = Vector2.zero;
            titleRect.offsetMax = Vector2.zero;

            // Add close button
            closeSettingsButton = MenuBootstrapper.CreateScaledButton(contentPanel.transform, "CloseButton", "CLOSE", 24);
            var closeRect = closeSettingsButton.GetComponent<RectTransform>();
            closeRect.anchorMin = new Vector2(0.3f, 0.1f);
            closeRect.anchorMax = new Vector2(0.7f, 0.25f);
            closeRect.offsetMin = Vector2.zero;
            closeRect.offsetMax = Vector2.zero;
            
            closeSettingsButton.onClick.AddListener(HideSettings);

            // Add coming soon text
            var comingSoon = MenuBootstrapper.CreateScaledText(contentPanel.transform, "ComingSoon", "Settings Coming Soon!", 20);
            var comingSoonRect = comingSoon.GetComponent<RectTransform>();
            comingSoonRect.anchorMin = new Vector2(0, 0.4f);
            comingSoonRect.anchorMax = new Vector2(1, 0.6f);
            comingSoonRect.offsetMin = Vector2.zero;
            comingSoonRect.offsetMax = Vector2.zero;
            
            settingsPanel.SetActive(false);
        }
    }

    // Supporting classes - ensure these exist or are compatible
    [System.Serializable]
    public class GameSettings
    {
        public GameMode gameMode;
        public string mapName;
        public int maxPlayers;
        public PlayerSetup[] players;
    }

    [System.Serializable]
    public class PlayerSetup
    {
        public PlayerType playerType;
        public bool isActive;
        public int team;
    }

    public enum GameMode
    {
        Skirmish,
        Campaign,
        Multiplayer
    }

    public enum PlayerType
    {
        Human,
        AI
    }

    // Simple GameSettingsManager if it doesn't exist
    public static class GameSettingsManager
    {
        public static GameSettings CurrentGameSettings { get; set; }
        public static GameSettingsManager Instance => null; // For compatibility
    }
}
