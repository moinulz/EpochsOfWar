using UnityEngine;

public class ResourceNode : MonoBehaviour
{
    [Header("Resource Properties")]
    public ResourceType resourceType = ResourceType.Wood;
    public int totalResources = 1000;
    public int currentResources = 1000;
    public int harvestAmount = 10;
    
    [Header("Visual")]
    public GameObject depletedModel;
    
    public enum ResourceType
    {
        Wood,
        Stone,
        Iron,
        Gold,
        Food
    }
    
    void Start()
    {
        if (currentResources <= 0)
            currentResources = totalResources;
            
        UpdateVisuals();
    }
    
    public bool CanHarvest()
    {
        return currentResources > 0;
    }
    
    public int Harvest(int amount = -1)
    {
        if (!CanHarvest()) return 0;
        
        int harvestAmount = (amount > 0) ? Mathf.Min(amount, this.harvestAmount) : this.harvestAmount;
        int actualHarvest = Mathf.Min(harvestAmount, currentResources);
        
        currentResources -= actualHarvest;
        UpdateVisuals();
        
        Debug.Log($"Harvested {actualHarvest} {resourceType} from node. Remaining: {currentResources}");
        return actualHarvest;
    }
    
    void UpdateVisuals()
    {
        var renderer = GetComponent<Renderer>();
        if (renderer != null)
        {
            float resourcePercent = (float)currentResources / totalResources;
            Color baseColor = GetResourceColor();
            renderer.material.color = Color.Lerp(Color.gray, baseColor, resourcePercent);
        }
        
        if (depletedModel != null)
        {
            depletedModel.SetActive(currentResources <= 0);
        }
    }
    
    Color GetResourceColor()
    {
        switch (resourceType)
        {
            case ResourceType.Wood: return new Color(0.6f, 0.3f, 0.1f);
            case ResourceType.Stone: return Color.gray;
            case ResourceType.Iron: return new Color(0.4f, 0.4f, 0.5f);
            case ResourceType.Gold: return Color.yellow;
            case ResourceType.Food: return Color.green;
            default: return Color.white;
        }
    }
}
