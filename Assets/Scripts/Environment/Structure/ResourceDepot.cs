using System;
using System.Collections.Generic;

public class ResourceDepot : HouseStructure
{
    protected ResourceCollection<AppleResource> appleResources;

    public int applesRequired;

    new protected void Awake()
    {
        appleResources = new ResourceCollection<AppleResource>();
        base.Awake();
    }

    public override void Reset()
    {
        appleResources = new ResourceCollection<AppleResource>();
        base.Reset();
    }

    public override bool IsComplete
    {
        get
        {
            return woodResources?.Count >= woodRequired
                && stoneResources?.Count >= stoneRequired
                && appleResources?.Count >= applesRequired;
        }
    }

    public override IDictionary<Type, int> GetResourcesRequired()
    {
        return new Dictionary<Type, int>
        {
            [typeof(WoodResource)] = woodRequired - woodResources.Count,
            [typeof(StoneResource)] = stoneRequired - stoneResources.Count,
            [typeof(AppleResource)] = applesRequired - appleResources.Count
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
        else if (resource is AppleResource apple)
        {
            AddResource(ref apple);
        }

        // Double check the source object is null to avoid duplication
        if (resource is object)
        {
            resource = null;
        }
    }

    protected void AddResource(ref AppleResource apple)
    {
        appleResources.Add(ref apple);
    }
}
