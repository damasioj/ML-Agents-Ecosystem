using System;
using System.Collections.Generic;
using UnityEngine;

public abstract class BaseSource : MonoBehaviour 
{
    public bool SourceHit { get; protected set; }
    public Dictionary<string, float> BoundaryLimits { get; set; }
    public int ResourceCount { get; }
    public abstract Type GetResourceType();
    public abstract BaseResource GetResource();
    public abstract void SetResourceAmount(Dictionary<Type, int> resourceData);

    public virtual void Reset()
    {
        gameObject.transform.localPosition =
            new Vector3
            (
                UnityEngine.Random.Range(BoundaryLimits["-X"], BoundaryLimits["X"]),
                1f,
                UnityEngine.Random.Range(BoundaryLimits["-Z"], BoundaryLimits["Z"])
            );

        SourceHit = false;
    }

    public virtual Vector3 Location
    {
        get
        {
            return gameObject.transform.position;
        }
        private set
        {
            gameObject.transform.position = value;
        }
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
