using System;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// The structure class represent all constructed objects in the scene.
/// This class is typically used as the goal for task-oriented agents, but could also be the target.
/// </summary>
public abstract class BaseStructure : MonoBehaviour
{
    public Dictionary<string, float> locationLimits;

    public virtual void Reset()
    {
        gameObject.transform.position =
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
            return gameObject.transform.localPosition;
        }
        private set
        {
            gameObject.transform.localPosition = value;
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
}
