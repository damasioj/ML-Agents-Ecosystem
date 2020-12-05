using System;
using System.Collections.Generic;

public class HouseStructure : BaseStructure
{
    protected ResourceCollection<WoodResource> woodResources; 
    protected ResourceCollection<StoneResource> stoneResources;

    public int woodRequired;
    public int stoneRequired;
    
    protected void Awake()
    {
        woodResources = new ResourceCollection<WoodResource>();
        stoneResources = new ResourceCollection<StoneResource>();
    }

    public override void Reset()
    {
        woodResources = new ResourceCollection<WoodResource>();
        stoneResources = new ResourceCollection<StoneResource>();
    }

    public override bool IsComplete
    {
        get
        {
            return woodResources?.Count >= woodRequired
                && stoneResources?.Count >= stoneRequired;
        }
    }

    public override IDictionary<Type, int> GetResourcesRequired()
    {
        return new Dictionary<Type, int>
        {
            [typeof(WoodResource)] = woodRequired - woodResources.Count,
            [typeof(StoneResource)] = stoneRequired - stoneResources.Count
        };
    }    

    public override void AddResource(ref BaseResource resource) 
    {
        if (resource is WoodResource wood)
        {
            AddResource(ref wood);
        }
        else if (resource is StoneResource stone)
        {
            AddResource(ref stone);
        }

        // Double check the source object is null to avoid duplication
        if (resource is object)
        {
            resource = null;
        }
    }

    protected void AddResource(ref WoodResource wood)
    {
        woodResources.Add(ref wood);
    }

    protected void AddResource(ref StoneResource stone)
    {
        stoneResources.Add(ref stone);
    }
}
