using System;
using System.Collections.Generic;
using UnityEngine;

public abstract class BaseStructure : MonoBehaviour
{
    public Dictionary<string, float> locationLimits { get; set; }

    public virtual void Reset()
    {
        gameObject.transform.localPosition =
            new Vector3
            (
                UnityEngine.Random.Range(locationLimits["-X"], locationLimits["X"]),
                -0.3f,
                UnityEngine.Random.Range(locationLimits["-Z"], locationLimits["Z"])
            );
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

    /// <summary>
    /// Adds a resource to the structure and removes it from the source.
    /// </summary>
    /// <param name="resource"></param>
    public abstract void AddResource(ref BaseResource resource);

    /// <summary>
    /// Determines if the structure is complete/finished.
    /// </summary>
    public abstract bool IsComplete { get; }

    /// <summary>
    /// Returns a dictionary with the type of resource required and the amount necessary to finish completion.
    /// </summary>
    /// <returns>IDictionary<Type, int></returns>
    public abstract IDictionary<Type, int> GetResourcesRequired();
    //public abstract void SetResourceRequirements(IDictionary<Type, int> requirements);
}
