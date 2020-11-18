using System;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Represents a "source" in the environment - an object that holds other objects.
/// Sources hold a collection of resources. Taking items from the source removes it from the collection.
/// This is used to represent a physical transfer of objects and reduce chances of duplication bugs.
/// </summary>
public abstract class BaseSource : BaseTarget 
{
    /// <summary>
    /// The range (X and Z) at which the source can spawn on the map.
    /// </summary>
    [SerializeField] private float range;
    /// <summary>
    /// The amount of resources this source has when it spawns.
    /// </summary>
    [SerializeField] protected int resourceCount;
    public abstract int ResourceCount { get; }
    public abstract override bool IsValid { get; }
    public bool SourceHit { get; protected set; }    
    public abstract Type GetResourceType();
    public abstract BaseResource TakeResource();
    public abstract void SetResourceAmount(Dictionary<Type, int> resourceData);

    public virtual void Reset()
    {
        Location =
            new Vector3
            (
                UnityEngine.Random.Range(range * -1, range),
                1f,
                UnityEngine.Random.Range(range * -1, range)
            );

        SourceHit = false;
    }    
}

public class BaseSource<T> : BaseSource
    where T : new()
{
    protected ResourceCollection<T> Resources { get; set; }

    public override bool IsValid => Resources?.Count > 0;
    public override int ResourceCount => Resources.Count;

    private void Awake()
    {
        SourceHit = false;
        Resources = new ResourceCollection<T>(resourceCount);
    }

    protected void OnTriggerEnter(Collider other)
    {
        SourceHit = true;
    }

    public override Type GetResourceType()
    {
        return typeof(T);
    }

    public override BaseResource TakeResource()
    {
        return Resources.Take() as BaseResource;
    }

    public override void SetResourceAmount(Dictionary<Type, int> resourceData)
    {
        if (resourceData.ContainsKey(typeof(T)))
        {
            ResetCollection(resourceData[typeof(T)]);
        }
    }

    protected void ResetCollection(int amount)
    {
        Resources = new ResourceCollection<T>(amount);
    }
}
