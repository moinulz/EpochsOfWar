using UnityEngine;
using UnityEngine.UI;

public class GameModeMenu : MonoBehaviour
{
    [Header("Main Buttons")]
    public Button campaignButton;
    public Button skirmishButton;
    public Button multiplayerButton;
    public Button backToMainButton;
    
    [Header("Panels")]
    public GameObject campaignPanel;
    public GameObject skirmishPanel;
    public GameObject multiplayerPanel;
    
    [Header("Campaign UI")]
    public Button newCampaignButton;
    public Button loadCampaignButton;
    
    void Start()
    {
        SetupButtons();
        ShowMainGameModeSelection();
    }
    
    void SetupButtons()
    {
        if (campaignButton) campaignButton.onClick.AddListener(() => ShowCampaignOptions());
        if (skirmishButton) skirmishButton.onClick.AddListener(() => ShowSkirmishSetup());
        if (multiplayerButton) multiplayerButton.onClick.AddListener(() => ShowMultiplayerOptions());
        if (backToMainButton) backToMainButton.onClick.AddListener(() => ReturnToMainMenu());
        
        if (newCampaignButton) newCampaignButton.onClick.AddListener(() => StartNewCampaign());
        if (loadCampaignButton) loadCampaignButton.onClick.AddListener(() => LoadCampaign());
    }
    
    public void ShowMainGameModeSelection()
    {
        HideAllPanels();
        // Show main game mode buttons (they should always be visible)
    }
    
    void ShowCampaignOptions()
    {
        HideAllPanels();
        if (campaignPanel) campaignPanel.SetActive(true);
    }
    
    void ShowSkirmishSetup()
    {
        HideAllPanels();
        if (skirmishPanel) skirmishPanel.SetActive(true);
        
        // Load the skirmish setup scene
        var skirmishSetup = FindFirstObjectByType<SkirmishSetup>();
        if (skirmishSetup != null)
        {
            skirmishSetup.InitializeSetup();
        }
    }
    
    void ShowMultiplayerOptions()
    {
        HideAllPanels();
        if (multiplayerPanel) multiplayerPanel.SetActive(true);
        
        // TODO: Implement multiplayer options
        Debug.Log("Multiplayer options - Coming Soon!");
    }
    
    void HideAllPanels()
    {
        if (campaignPanel) campaignPanel.SetActive(false);
        if (skirmishPanel) skirmishPanel.SetActive(false);
        if (multiplayerPanel) multiplayerPanel.SetActive(false);
    }
    
    void StartNewCampaign()
    {
        Debug.Log("Starting new campaign - Coming Soon!");
        // TODO: Implement campaign system
    }
    
    void LoadCampaign()
    {
        Debug.Log("Loading campaign - Coming Soon!");
        // TODO: Implement campaign loading
    }
    
    void ReturnToMainMenu()
    {
        UnityEngine.SceneManagement.SceneManager.LoadScene("MainMenu");
    }
}
