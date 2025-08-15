using UnityEngine;

public abstract class Unit : MonoBehaviour
{
    [Header("Unit Properties")]
    public string unitName = "Basic Unit";
    public int health = 100;
    public int maxHealth = 100;
    public float moveSpeed = 5f;
    public int playerIndex = 0;
    public Color playerColor = Color.blue;
    
    [Header("Combat")]
    public int attackDamage = 10;
    public float attackRange = 2f;
    public float attackCooldown = 1f;
    
    protected bool isSelected = false;
    protected Vector3 targetPosition;
    protected bool isMoving = false;
    protected GameObject selectionIndicator;
    
    protected virtual void Start()
    {
        targetPosition = transform.position;
        SetPlayerColor();
        CreateSelectionIndicator();
    }
    
    protected virtual void Update()
    {
        if (isMoving)
        {
            MoveToTarget();
        }
    }
    
    public virtual void SetPlayerColor()
    {
        var renderer = GetComponent<Renderer>();
        if (renderer != null)
        {
            var material = new Material(Shader.Find("Standard"));
            material.color = playerColor;
            renderer.material = material;
        }
    }
    
    public virtual void MoveTo(Vector3 position)
    {
        targetPosition = position;
        isMoving = true;
    }
    
    protected virtual void MoveToTarget()
    {
        float distance = Vector3.Distance(transform.position, targetPosition);
        if (distance > 0.1f)
        {
            Vector3 direction = (targetPosition - transform.position).normalized;
            transform.position += direction * moveSpeed * Time.deltaTime;
        }
        else
        {
            isMoving = false;
        }
    }
    
    void CreateSelectionIndicator()
    {
        selectionIndicator = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
        selectionIndicator.name = "SelectionRing";
        selectionIndicator.transform.SetParent(transform);
        selectionIndicator.transform.localPosition = Vector3.zero;
        selectionIndicator.transform.localScale = new Vector3(4f, 0.1f, 4f);
        
        var renderer = selectionIndicator.GetComponent<Renderer>();
        var material = new Material(Shader.Find("Standard"));
        material.color = new Color(0, 1, 0, 0.5f);
        renderer.material = material;
        
        var collider = selectionIndicator.GetComponent<Collider>();
        if (collider != null) Destroy(collider);
        
        selectionIndicator.SetActive(false);
    }
    
    public virtual void SetSelected(bool selected)
    {
        isSelected = selected;
        if (selectionIndicator != null)
        {
            selectionIndicator.SetActive(selected);
        }
    }
}
