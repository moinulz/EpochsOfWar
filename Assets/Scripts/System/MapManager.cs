using UnityEngine;

[System.Serializable]
public class SpawnPoint
{
    public Vector3 position;
    public int playerIndex;
    public bool isOccupied = false;
    
    public SpawnPoint(Vector3 pos, int index)
    {
        position = pos;
        playerIndex = index;
    }
}

public class MapManager : MonoBehaviour
{
    [Header("Map Information")]
    public string mapName = "Default Map";
    public int maxPlayers = 4;
    public Vector2 mapSize = new Vector2(100, 100);
    
    [Header("Spawn Points")]
    public SpawnPoint[] spawnPoints = new SpawnPoint[4];
    
    [Header("Map Features")]
    public GameObject[] resourceNodes;
    public GameObject[] neutralBuildings;
    
    void Start()
    {
        InitializeSpawnPoints();
    }
    
    void InitializeSpawnPoints()
    {
        if (spawnPoints == null || spawnPoints.Length == 0)
        {
            CreateDefaultSpawnPoints();
        }
    }
    
    void CreateDefaultSpawnPoints()
    {
        spawnPoints = new SpawnPoint[maxPlayers];
        
        // Create spawn points in corners and middle edges for 4 players
        switch (maxPlayers)
        {
            case 2:
                spawnPoints[0] = new SpawnPoint(new Vector3(-30, 0, 0), 0);
                spawnPoints[1] = new SpawnPoint(new Vector3(30, 0, 0), 1);
                break;
                
            case 4:
                spawnPoints[0] = new SpawnPoint(new Vector3(-30, 0, -30), 0); // Bottom-left
                spawnPoints[1] = new SpawnPoint(new Vector3(30, 0, -30), 1);  // Bottom-right
                spawnPoints[2] = new SpawnPoint(new Vector3(-30, 0, 30), 2);  // Top-left
                spawnPoints[3] = new SpawnPoint(new Vector3(30, 0, 30), 3);   // Top-right
                break;
                
            default:
                // For other player counts, distribute evenly in a circle
                float angleStep = 360f / maxPlayers;
                float radius = 35f;
                
                for (int i = 0; i < maxPlayers; i++)
                {
                    float angle = i * angleStep * Mathf.Deg2Rad;
                    Vector3 pos = new Vector3(
                        Mathf.Cos(angle) * radius,
                        0,
                        Mathf.Sin(angle) * radius
                    );
                    spawnPoints[i] = new SpawnPoint(pos, i);
                }
                break;
        }
    }
    
    public Vector3 GetSpawnPosition(int playerIndex)
    {
        if (playerIndex >= 0 && playerIndex < spawnPoints.Length)
        {
            return spawnPoints[playerIndex].position;
        }
        
        return Vector3.zero;
    }
    
    public void AssignRandomSpawnPoints(GameSettings gameSettings)
    {
        // Get list of active players
        var activePlayers = new System.Collections.Generic.List<int>();
        for (int i = 0; i < gameSettings.players.Length; i++)
        {
            if (gameSettings.players[i].isActive && gameSettings.players[i].playerType != PlayerType.Disabled)
            {
                activePlayers.Add(i);
            }
        }
        
        // Randomly assign spawn points
        var availableSpawns = new System.Collections.Generic.List<int>();
        for (int i = 0; i < spawnPoints.Length; i++)
        {
            availableSpawns.Add(i);
        }
        
        foreach (int playerIndex in activePlayers)
        {
            if (availableSpawns.Count > 0)
            {
                int randomSpawnIndex = Random.Range(0, availableSpawns.Count);
                int spawnPointIndex = availableSpawns[randomSpawnIndex];
                
                spawnPoints[spawnPointIndex].playerIndex = playerIndex;
                spawnPoints[spawnPointIndex].isOccupied = true;
                availableSpawns.RemoveAt(randomSpawnIndex);
            }
        }
    }
    
    void OnDrawGizmos()
    {
        if (spawnPoints != null)
        {
            for (int i = 0; i < spawnPoints.Length; i++)
            {
                if (spawnPoints[i] != null)
                {
                    Gizmos.color = spawnPoints[i].isOccupied ? Color.red : Color.green;
                    Gizmos.DrawWireSphere(spawnPoints[i].position, 3f);
                    Gizmos.DrawWireCube(spawnPoints[i].position + Vector3.up * 5f, Vector3.one * 2f);
                }
            }
        }
        
        // Draw map bounds
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireCube(Vector3.zero, new Vector3(mapSize.x, 1, mapSize.y));
    }
}
