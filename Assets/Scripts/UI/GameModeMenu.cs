using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using EpochsOfWar.UI;

namespace EpochsOfWar.UI
{
    /// <summary>
    /// Manages the game mode selection screen with proper scaling and reliable navigation
    /// </summary>
    public class GameModeMenu : MonoBehaviour
    {
        [Header("Main Game Mode Buttons")]
        [SerializeField] private Button campaignButton;
        [SerializeField] private Button skirmishButton;
        [SerializeField] private Button multiplayerButton;
        [SerializeField] private Button backToMainButton;

        [Header("Mode Panels")]
        [SerializeField] private GameObject campaignPanel;
        [SerializeField] private GameObject skirmishPanel;
        [SerializeField] private GameObject multiplayerPanel;

        [Header("Campaign Buttons")]
        [SerializeField] private Button newCampaignButton;
        [SerializeField] private Button loadCampaignButton;

        [Header("Auto-Setup")]
        [SerializeField] private bool autoFindComponents = true;

        private SkirmishSetup skirmishSetup;

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
            ShowMainGameModeSelection();
            Debug.Log("GameModeMenu initialized with proper scaling");
        }

        void AutoFindComponents()
        {
            // Auto-find buttons if not assigned
            if (!campaignButton) campaignButton = FindButtonByName("CampaignButton");
            if (!skirmishButton) skirmishButton = FindButtonByName("SkirmishButton");
            if (!multiplayerButton) multiplayerButton = FindButtonByName("MultiplayerButton");
            if (!backToMainButton) backToMainButton = FindButtonByName("BackButton");

            // Auto-find panels if not assigned
            if (!campaignPanel) campaignPanel = GameObject.Find("CampaignPanel");
            if (!skirmishPanel) skirmishPanel = GameObject.Find("SkirmishPanel");
            if (!multiplayerPanel) multiplayerPanel = GameObject.Find("MultiplayerPanel");

            // Auto-find campaign buttons
            if (!newCampaignButton) newCampaignButton = FindButtonByName("NewCampaignButton");
            if (!loadCampaignButton) loadCampaignButton = FindButtonByName("LoadCampaignButton");
        }

        Button FindButtonByName(string buttonName)
        {
            var buttonObj = GameObject.Find(buttonName);
            return buttonObj ? buttonObj.GetComponent<Button>() : null;
        }

        void SetupButtonListeners()
        {
            // Main mode selection buttons
            if (campaignButton) 
            {
                campaignButton.onClick.RemoveAllListeners();
                campaignButton.onClick.AddListener(ShowCampaignOptions);
            }

            if (skirmishButton) 
            {
                skirmishButton.onClick.RemoveAllListeners();
                skirmishButton.onClick.AddListener(ShowSkirmishSetup);
            }

            if (multiplayerButton) 
            {
                multiplayerButton.onClick.RemoveAllListeners();
                multiplayerButton.onClick.AddListener(ShowMultiplayerOptions);
            }

            if (backToMainButton) 
            {
                backToMainButton.onClick.RemoveAllListeners();
                backToMainButton.onClick.AddListener(ReturnToMainMenu);
            }

            // Campaign buttons
            if (newCampaignButton) 
            {
                newCampaignButton.onClick.RemoveAllListeners();
                newCampaignButton.onClick.AddListener(StartNewCampaign);
            }

            if (loadCampaignButton) 
            {
                loadCampaignButton.onClick.RemoveAllListeners();
                loadCampaignButton.onClick.AddListener(LoadCampaign);
            }
        }

        public void ShowMainGameModeSelection()
        {
            HideAllPanels();
            SetMainButtonsActive(true);
            Debug.Log("Showing main game mode selection");
        }

        void ShowCampaignOptions()
        {
            HideAllPanels();
            SetMainButtonsActive(false);
            
            if (campaignPanel) 
            {
                campaignPanel.SetActive(true);
                Debug.Log("Showing campaign options");
            }
            else
            {
                Debug.LogWarning("Campaign panel not found! Creating basic panel...");
                CreateBasicCampaignPanel();
            }
        }

        void ShowSkirmishSetup()
        {
            HideAllPanels();
            SetMainButtonsActive(false);
            
            if (skirmishPanel) 
            {
                skirmishPanel.SetActive(true);
                
                // Ensure SkirmishSetup component exists and is properly initialized
                if (!skirmishSetup)
                {
                    skirmishSetup = skirmishPanel.GetComponentInChildren<SkirmishSetup>(true);
                    if (!skirmishSetup)
                    {
                        skirmishSetup = skirmishPanel.AddComponent<SkirmishSetup>();
                        Debug.Log("Added SkirmishSetup component to skirmish panel");
                    }
                }
                
                // Initialize skirmish setup
                if (skirmishSetup)
                {
                    skirmishSetup.InitializeSetup();
                    Debug.Log("Skirmish setup initialized");
                }
                
                Debug.Log("Showing skirmish setup");
            }
            else
            {
                Debug.LogWarning("Skirmish panel not found! Creating basic panel...");
                CreateBasicSkirmishPanel();
            }
        }

        void ShowMultiplayerOptions()
        {
            HideAllPanels();
            SetMainButtonsActive(false);
            
            if (multiplayerPanel) 
            {
                multiplayerPanel.SetActive(true);
            }
            else
            {
                Debug.LogWarning("Multiplayer panel not found! Creating basic panel...");
                CreateBasicMultiplayerPanel();
            }
            
            Debug.Log("Showing multiplayer options (Coming Soon!)");
        }

        void HideAllPanels()
        {
            if (campaignPanel) campaignPanel.SetActive(false);
            if (skirmishPanel) skirmishPanel.SetActive(false);
            if (multiplayerPanel) multiplayerPanel.SetActive(false);
        }

        void SetMainButtonsActive(bool active)
        {
            if (campaignButton) campaignButton.gameObject.SetActive(active);
            if (skirmishButton) skirmishButton.gameObject.SetActive(active);
            if (multiplayerButton) multiplayerButton.gameObject.SetActive(active);
        }

        void StartNewCampaign() 
        { 
            Debug.Log("Starting new campaign - Feature coming soon!");
            // TODO: Load campaign scene
        }

        void LoadCampaign() 
        { 
            Debug.Log("Loading saved campaign - Feature coming soon!");
            // TODO: Show load dialog
        }

        void ReturnToMainMenu()
        {
            Debug.Log("Returning to Main Menu");
            SceneManager.LoadScene("MainMenu");
        }

        // Fallback panel creation methods in case panels are missing
        void CreateBasicCampaignPanel()
        {
            var canvas = FindObjectOfType<Canvas>();
            if (!canvas) return;

            campaignPanel = new GameObject("CampaignPanel", typeof(RectTransform), typeof(Image));
            campaignPanel.transform.SetParent(canvas.transform, false);
            
            var image = campaignPanel.GetComponent<Image>();
            image.color = new Color(0, 0, 0, 0.8f);
            
            var rect = campaignPanel.GetComponent<RectTransform>();
            rect.anchorMin = Vector2.zero;
            rect.anchorMax = Vector2.one;
            rect.offsetMin = Vector2.zero;
            rect.offsetMax = Vector2.zero;

            var title = MenuBootstrapper.CreateScaledText(campaignPanel.transform, "Title", "CAMPAIGN MODE", 32);
            var titleRect = title.GetComponent<RectTransform>();
            titleRect.anchorMin = new Vector2(0, 0.7f);
            titleRect.anchorMax = new Vector2(1, 0.9f);
            titleRect.offsetMin = Vector2.zero;
            titleRect.offsetMax = Vector2.zero;

            var backBtn = MenuBootstrapper.CreateScaledButton(campaignPanel.transform, "BackButton", "BACK", 24);
            var backRect = backBtn.GetComponent<RectTransform>();
            backRect.anchorMin = new Vector2(0.4f, 0.1f);
            backRect.anchorMax = new Vector2(0.6f, 0.2f);
            backRect.offsetMin = Vector2.zero;
            backRect.offsetMax = Vector2.zero;
            
            backBtn.onClick.AddListener(ShowMainGameModeSelection);
            
            campaignPanel.SetActive(true);
        }

        void CreateBasicSkirmishPanel()
        {
            var canvas = FindObjectOfType<Canvas>();
            if (!canvas) return;

            skirmishPanel = new GameObject("SkirmishPanel", typeof(RectTransform), typeof(Image));
            skirmishPanel.transform.SetParent(canvas.transform, false);
            
            var image = skirmishPanel.GetComponent<Image>();
            image.color = new Color(0, 0, 0, 0.8f);
            
            var rect = skirmishPanel.GetComponent<RectTransform>();
            rect.anchorMin = Vector2.zero;
            rect.anchorMax = Vector2.one;
            rect.offsetMin = Vector2.zero;
            rect.offsetMax = Vector2.zero;

            var title = MenuBootstrapper.CreateScaledText(skirmishPanel.transform, "Title", "SKIRMISH SETUP", 32);
            var titleRect = title.GetComponent<RectTransform>();
            titleRect.anchorMin = new Vector2(0, 0.8f);
            titleRect.anchorMax = new Vector2(1, 0.9f);
            titleRect.offsetMin = Vector2.zero;
            titleRect.offsetMax = Vector2.zero;

            var startBtn = MenuBootstrapper.CreateScaledButton(skirmishPanel.transform, "StartButton", "START GAME", 24);
            var startRect = startBtn.GetComponent<RectTransform>();
            startRect.anchorMin = new Vector2(0.2f, 0.2f);
            startRect.anchorMax = new Vector2(0.4f, 0.3f);
            startRect.offsetMin = Vector2.zero;
            startRect.offsetMax = Vector2.zero;
            
            startBtn.onClick.AddListener(() => {
                Debug.Log("Starting skirmish game...");
                SceneManager.LoadScene("Game");
            });

            var backBtn = MenuBootstrapper.CreateScaledButton(skirmishPanel.transform, "BackButton", "BACK", 24);
            var backRect = backBtn.GetComponent<RectTransform>();
            backRect.anchorMin = new Vector2(0.6f, 0.2f);
            backRect.anchorMax = new Vector2(0.8f, 0.3f);
            backRect.offsetMin = Vector2.zero;
            backRect.offsetMax = Vector2.zero;
            
            backBtn.onClick.AddListener(ShowMainGameModeSelection);

            // Add SkirmishSetup component
            skirmishSetup = skirmishPanel.AddComponent<SkirmishSetup>();
            
            skirmishPanel.SetActive(true);
        }

        void CreateBasicMultiplayerPanel()
        {
            var canvas = FindObjectOfType<Canvas>();
            if (!canvas) return;

            multiplayerPanel = new GameObject("MultiplayerPanel", typeof(RectTransform), typeof(Image));
            multiplayerPanel.transform.SetParent(canvas.transform, false);
            
            var image = multiplayerPanel.GetComponent<Image>();
            image.color = new Color(0, 0, 0, 0.8f);
            
            var rect = multiplayerPanel.GetComponent<RectTransform>();
            rect.anchorMin = Vector2.zero;
            rect.anchorMax = Vector2.one;
            rect.offsetMin = Vector2.zero;
            rect.offsetMax = Vector2.zero;

            var title = MenuBootstrapper.CreateScaledText(multiplayerPanel.transform, "Title", "MULTIPLAYER", 32);
            var titleRect = title.GetComponent<RectTransform>();
            titleRect.anchorMin = new Vector2(0, 0.7f);
            titleRect.anchorMax = new Vector2(1, 0.9f);
            titleRect.offsetMin = Vector2.zero;
            titleRect.offsetMax = Vector2.zero;

            var comingSoon = MenuBootstrapper.CreateScaledText(multiplayerPanel.transform, "ComingSoon", "COMING SOON!", 24);
            var comingSoonRect = comingSoon.GetComponent<RectTransform>();
            comingSoonRect.anchorMin = new Vector2(0, 0.4f);
            comingSoonRect.anchorMax = new Vector2(1, 0.6f);
            comingSoonRect.offsetMin = Vector2.zero;
            comingSoonRect.offsetMax = Vector2.zero;

            var backBtn = MenuBootstrapper.CreateScaledButton(multiplayerPanel.transform, "BackButton", "BACK", 24);
            var backRect = backBtn.GetComponent<RectTransform>();
            backRect.anchorMin = new Vector2(0.4f, 0.1f);
            backRect.anchorMax = new Vector2(0.6f, 0.2f);
            backRect.offsetMin = Vector2.zero;
            backRect.offsetMax = Vector2.zero;
            
            backBtn.onClick.AddListener(ShowMainGameModeSelection);
            
            multiplayerPanel.SetActive(true);
        }
    }
}
