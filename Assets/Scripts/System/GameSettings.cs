using UnityEngine;

[System.Serializable]
public class GameSettings
{
    [Header("Game Mode")]
    public GameMode gameMode = GameMode.Skirmish;
    
    [Header("Map Settings")]
    public string mapName = "DefaultMap";
    public int maxPlayers = 4;
    
    [Header("Players")]
    public PlayerSetup[] players = new PlayerSetup[4];
    
    [Header("Victory Conditions")]
    public VictoryCondition victoryCondition = VictoryCondition.AllCapitalsDestroyed;
    
    [Header("Resources")]
    public bool customResources = false;
    
    public GameSettings()
    {
        // Initialize default player setups
        for (int i = 0; i < players.Length; i++)
        {
            players[i] = new PlayerSetup();
            players[i].playerType = (i == 0) ? PlayerType.Human : PlayerType.AI;
            players[i].team = i; // Default: everyone on different teams
            players[i].difficulty = AIDifficulty.Medium;
            players[i].startingResources = new Resources(500, 500, 500, 500);
        }
    }
}

[System.Serializable]
public class PlayerSetup
{
    public PlayerType playerType = PlayerType.AI;
    public AIDifficulty difficulty = AIDifficulty.Medium;
    public int team = 0;
    public Resources startingResources = new Resources(500, 500, 500, 500);
    public Color playerColor = Color.blue;
    public bool isActive = true;
}

[System.Serializable]
public class Resources
{
    public int wood = 500;
    public int stone = 500;
    public int iron = 500;
    public int gold = 500;
    
    public Resources(int w, int s, int i, int g)
    {
        wood = w; stone = s; iron = i; gold = g;
    }
}

public enum GameMode
{
    Campaign,
    Skirmish,
    Multiplayer
}

public enum PlayerType
{
    Human,
    AI,
    Disabled
}

public enum AIDifficulty
{
    Easy,
    Medium,
    Hard,
    Extreme
}

public enum VictoryCondition
{
    AllUnitsDestroyed,
    AllCapitalsDestroyed,
    AllOpponentsSurrender,
    Custom
}
