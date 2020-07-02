using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Collection for managing environment resources.
/// </summary>
/// <typeparam name="T">Resource type</typeparam>
public class ResourceCollection<T> : ICollection
    where T : new()
{
    List<T> collection;

    public ResourceCollection()
    {
        collection = new List<T>();
    }

    public ResourceCollection(int count)
    {
        collection = new List<T>();

        for (int i = 0; i < count; i++)
        {
            collection.Add(new T());
        }
    }

    public int Count => collection.Count;

    public bool IsSynchronized => false;

    public object SyncRoot => null;

    public void CopyTo(Array array, int index)
    {
        collection.ToArray().CopyTo(array, index);
    }

    public IEnumerator GetEnumerator()
    {
        return collection.GetEnumerator();
    }

    /// <summary>
    /// Returns the resource and removes it from the collection.
    /// </summary>
    /// <returns></returns>
    public T Take()
    {
        if (Count > 0)
        {
            T resource = collection[0];
            collection.RemoveAt(0);
            return resource;
        }

        return default;
    }

    public void Add(ref T resource)
    {
        var newResource = resource;
        collection.Add(newResource);
        resource = default;
    }
}
