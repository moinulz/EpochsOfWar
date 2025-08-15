using UnityEngine;
using UnityEngine.InputSystem;

public class UnitSpawner : MonoBehaviour
{
    [Header("Unit Prefabs")]
    public GameObject workerPrefab;
    public GameObject soldierPrefab;
    
    [Header("Spawn Settings")]
    public Transform spawnPoint;
    public int playerIndex = 0;
    public Color playerColor = Color.blue;
    
    [Header("Resources")]
    public int workerCost = 50;
    public int soldierCost = 100;
    
    private GameManager gameManager;
    private Camera playerCamera;
    
    void Start()
    {
        gameManager = FindFirstObjectByType<GameManager>();
        playerCamera = Camera.main;
        
        if (spawnPoint == null)
        {
            var spawnPointObj = transform.Find("SpawnPoint");
            if (spawnPointObj != null)
            {
                spawnPoint = spawnPointObj;
            }
            else
            {
                spawnPoint = transform;
            }
        }
    }
    
    void Update()
    {
        HandleInput();
    }
    
    void HandleInput()
    {
        Keyboard keyboard = Keyboard.current;
        if (keyboard != null)
        {
            if (keyboard.wKey.wasPressedThisFrame)
            {
                Debug.Log("W key pressed - spawning worker (testing mode)");
                SpawnWorker();
            }
            if (keyboard.sKey.wasPressedThisFrame)
            {
                Debug.Log("S key pressed - spawning soldier (testing mode)");
                SpawnSoldier();
            }
        }
        
    }
    
    public void SpawnWorker()
    {
        SpawnUnit(workerPrefab, "Worker", workerCost);
    }
    
    public void SpawnSoldier()
    {
        SpawnUnit(soldierPrefab, "Soldier", soldierCost);
    }
    
    void SpawnUnit(GameObject prefab, string unitType, int cost)
    {
        if (prefab == null)
        {
            CreatePlaceholderUnit(unitType);
            return;
        }
        
        Vector3 spawnPosition = spawnPoint.position + Vector3.forward * 2f;
        GameObject unit = Instantiate(prefab, spawnPosition, Quaternion.identity);
        
        var unitComponent = unit.GetComponent<Unit>();
        if (unitComponent != null)
        {
            unitComponent.playerIndex = playerIndex;
            unitComponent.playerColor = playerColor;
            unitComponent.SetPlayerColor();
        }
        
        Debug.Log($"Spawned {unitType} for Player {playerIndex + 1}");
    }
    
    void CreatePlaceholderUnit(string unitType)
    {
        GameObject unit = GameObject.CreatePrimitive(PrimitiveType.Cube);
        unit.name = $"{unitType}_Player_{playerIndex + 1}";
        unit.transform.position = spawnPoint.position + Vector3.forward * 2f;
        unit.transform.localScale = new Vector3(2f, 2f, 2f);
        
        if (unitType == "Worker")
        {
            unit.AddComponent<Worker>();
        }
        else if (unitType == "Soldier")
        {
            unit.AddComponent<Soldier>();
        }
        
        var unitComponent = unit.GetComponent<Unit>();
        if (unitComponent != null)
        {
            unitComponent.playerIndex = playerIndex;
            unitComponent.playerColor = playerColor;
            unitComponent.SetPlayerColor();
        }
        
        Debug.Log($"Created placeholder {unitType} for Player {playerIndex + 1}");
    }
}
