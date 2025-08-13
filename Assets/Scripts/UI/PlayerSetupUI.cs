using UnityEngine;
using UnityEngine.UI;

public class PlayerSetupUI : MonoBehaviour
{
    [Header("Player Info")]
    public Text playerLabel;
    public Dropdown playerTypeDropdown;
    public Dropdown difficultyDropdown;
    public Dropdown teamDropdown;
    public Image colorDisplay;
    public Button colorButton;
    
    [Header("Resources")]
    public Button woodMinusButton;
    public Button woodPlusButton;
    public Text woodText;
    public Button stoneMinusButton;
    public Button stonePlusButton;
    public Text stoneText;
    public Button ironMinusButton;
    public Button ironPlusButton;
    public Text ironText;
    public Button goldMinusButton;
    public Button goldPlusButton;
    public Text goldText;
    
    private int playerIndex;
    private PlayerSetup playerSetup;
    private SkirmishSetup skirmishSetup;
    private Color[] playerColors = { Color.blue, Color.red, Color.green, Color.yellow, Color.cyan, Color.magenta };
    
    public void Initialize(int index, PlayerSetup setup, SkirmishSetup parent)
    {
        playerIndex = index;
        playerSetup = setup;
        skirmishSetup = parent;
        
        SetupPlayerColor();
        SetupUI();
        RefreshUI();
    }
    
    void SetupPlayerColor()
    {
        if (playerIndex < playerColors.Length)
        {
            playerSetup.playerColor = playerColors[playerIndex];
        }
        else
        {
            playerSetup.playerColor = Random.ColorHSV();
        }
    }
    
    void SetupUI()
    {
        // Player label
        if (playerLabel)
            playerLabel.text = $"Player {playerIndex + 1}";
        
        // Player type dropdown
        if (playerTypeDropdown)
        {
            playerTypeDropdown.ClearOptions();
            playerTypeDropdown.AddOptions(new System.Collections.Generic.List<string> { "Human", "AI", "Disabled" });
            playerTypeDropdown.onValueChanged.AddListener(OnPlayerTypeChanged);
        }
        
        // Difficulty dropdown (for AI)
        if (difficultyDropdown)
        {
            difficultyDropdown.ClearOptions();
            difficultyDropdown.AddOptions(new System.Collections.Generic.List<string> { "Easy", "Medium", "Hard", "Extreme" });
            difficultyDropdown.onValueChanged.AddListener(OnDifficultyChanged);
        }
        
        // Team dropdown
        if (teamDropdown)
        {
            teamDropdown.ClearOptions();
            var teamOptions = new System.Collections.Generic.List<string>();
            for (int i = 1; i <= 4; i++)
            {
                teamOptions.Add($"Team {i}");
            }
            teamDropdown.AddOptions(teamOptions);
            teamDropdown.onValueChanged.AddListener(OnTeamChanged);
        }
        
        // Color button
        if (colorButton)
            colorButton.onClick.AddListener(CyclePlayerColor);
        
        // Resource buttons
        SetupResourceButtons();
    }
    
    void SetupResourceButtons()
    {
        // Wood
        if (woodMinusButton) woodMinusButton.onClick.AddListener(() => ChangeResource("wood", -100));
        if (woodPlusButton) woodPlusButton.onClick.AddListener(() => ChangeResource("wood", 100));
        
        // Stone
        if (stoneMinusButton) stoneMinusButton.onClick.AddListener(() => ChangeResource("stone", -100));
        if (stonePlusButton) stonePlusButton.onClick.AddListener(() => ChangeResource("stone", 100));
        
        // Iron
        if (ironMinusButton) ironMinusButton.onClick.AddListener(() => ChangeResource("iron", -100));
        if (ironPlusButton) ironPlusButton.onClick.AddListener(() => ChangeResource("iron", 100));
        
        // Gold
        if (goldMinusButton) goldMinusButton.onClick.AddListener(() => ChangeResource("gold", -100));
        if (goldPlusButton) goldPlusButton.onClick.AddListener(() => ChangeResource("gold", 100));
    }
    
    public void RefreshUI()
    {
        // Update player type
        if (playerTypeDropdown)
            playerTypeDropdown.value = (int)playerSetup.playerType;
        
        // Update difficulty
        if (difficultyDropdown)
        {
            difficultyDropdown.value = (int)playerSetup.difficulty;
            difficultyDropdown.interactable = playerSetup.playerType == PlayerType.AI;
        }
        
        // Update team
        if (teamDropdown)
            teamDropdown.value = playerSetup.team;
        
        // Update color display
        if (colorDisplay)
            colorDisplay.color = playerSetup.playerColor;
        
        // Update resource displays
        UpdateResourceDisplays();
        
        // Enable/disable UI based on player type
        bool isActive = playerSetup.playerType != PlayerType.Disabled;
        SetUIActive(isActive);
    }
    
    void UpdateResourceDisplays()
    {
        if (woodText) woodText.text = playerSetup.startingResources.wood.ToString();
        if (stoneText) stoneText.text = playerSetup.startingResources.stone.ToString();
        if (ironText) ironText.text = playerSetup.startingResources.iron.ToString();
        if (goldText) goldText.text = playerSetup.startingResources.gold.ToString();
    }
    
    void SetUIActive(bool active)
    {
        // Enable/disable resource controls based on whether this player slot is active
        var resourceButtons = GetComponentsInChildren<Button>();
        foreach (var button in resourceButtons)
        {
            if (button != colorButton && button != playerTypeDropdown.GetComponent<Button>())
            {
                button.interactable = active;
            }
        }
        
        if (teamDropdown) teamDropdown.interactable = active;
        if (difficultyDropdown) difficultyDropdown.interactable = active && playerSetup.playerType == PlayerType.AI;
    }
    
    void OnPlayerTypeChanged(int value)
    {
        playerSetup.playerType = (PlayerType)value;
        playerSetup.isActive = playerSetup.playerType != PlayerType.Disabled;
        
        RefreshUI();
        NotifyParent();
    }
    
    void OnDifficultyChanged(int value)
    {
        playerSetup.difficulty = (AIDifficulty)value;
        NotifyParent();
    }
    
    void OnTeamChanged(int value)
    {
        playerSetup.team = value;
        NotifyParent();
    }
    
    void CyclePlayerColor()
    {
        int currentIndex = System.Array.IndexOf(playerColors, playerSetup.playerColor);
        currentIndex = (currentIndex + 1) % playerColors.Length;
        playerSetup.playerColor = playerColors[currentIndex];
        
        RefreshUI();
        NotifyParent();
    }
    
    void ChangeResource(string resourceType, int amount)
    {
        switch (resourceType.ToLower())
        {
            case "wood":
                playerSetup.startingResources.wood = Mathf.Clamp(playerSetup.startingResources.wood + amount, 200, 10000);
                break;
            case "stone":
                playerSetup.startingResources.stone = Mathf.Clamp(playerSetup.startingResources.stone + amount, 200, 10000);
                break;
            case "iron":
                playerSetup.startingResources.iron = Mathf.Clamp(playerSetup.startingResources.iron + amount, 200, 10000);
                break;
            case "gold":
                playerSetup.startingResources.gold = Mathf.Clamp(playerSetup.startingResources.gold + amount, 200, 10000);
                break;
        }
        
        UpdateResourceDisplays();
        NotifyParent();
    }
    
    void NotifyParent()
    {
        if (skirmishSetup != null)
        {
            skirmishSetup.OnPlayerSettingsChanged(playerIndex, playerSetup);
        }
    }
}
