using UnityEngine;

public class Soldier : Unit
{
    [Header("Soldier Specific")]
    public float patrolRadius = 10f;
    
    protected override void Start()
    {
        base.Start();
        unitName = "Soldier";
        health = 100;
        maxHealth = 100;
        moveSpeed = 6f;
        attackDamage = 25;
        attackRange = 3f;
    }
    
    public void Attack(Unit target)
    {
        Debug.Log($"Soldier attacking {target.unitName}");
    }
}
