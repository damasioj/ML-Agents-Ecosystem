using System;
using System.Collections.Generic;
using UnityEngine;

public abstract class BaseSource : BaseTarget 
{
    public override bool IsValid { get => ResourceCount > 0; }
    public bool SourceHit { get; protected set; }
    public Dictionary<string, float> BoundaryLimits { get; set; } // TODO : maybe refactor this
    public int ResourceCount { get; }
    public abstract Type GetResourceType();
    public abstract BaseResource GetResource();
    public abstract void SetResourceAmount(Dictionary<Type, int> resourceData);

    public virtual void Reset()
    {
        Location =
            new Vector3
            (
                UnityEngine.Random.Range(BoundaryLimits["-X"], BoundaryLimits["X"]),
                1f,
                UnityEngine.Random.Range(BoundaryLimits["-Z"], BoundaryLimits["Z"])
            );

        SourceHit = false;
    }    
}

public class BaseSource<T> : BaseSource
    where T : new()
{
    protected ResourceCollection<T> Resources { get; set; }    

    private void Awake()
    {
        SourceHit = false;
    }

    private void OnTriggerEnter(Collider other)
    {
        SourceHit = true;
    }

    public override Type GetResourceType()
    {
        return typeof(T);
    }

    public override BaseResource GetResource()
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

    private void ResetCollection(int amount)
    {
        Resources = new ResourceCollection<T>(amount);
    }
}
