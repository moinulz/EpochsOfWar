using UnityEngine;

public class Worker : Unit
{
    [Header("Worker Specific")]
    public int resourceCapacity = 10;
    public int currentResources = 0;
    public string resourceType = "Wood";
    
    protected override void Start()
    {
        base.Start();
        unitName = "Worker";
        health = 50;
        maxHealth = 50;
        moveSpeed = 4f;
        attackDamage = 5;
    }
    
    public void CollectResources()
    {
        Debug.Log($"Worker collecting {resourceType}");
    }
    
    public void DepositResources()
    {
        Debug.Log($"Worker depositing {currentResources} {resourceType}");
        currentResources = 0;
    }
}
