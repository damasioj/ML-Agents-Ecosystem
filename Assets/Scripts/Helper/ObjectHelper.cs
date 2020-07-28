using UnityEngine;

public static class ObjectHelper
{
    public static Vector3 GetDimensions(GameObject gameObject)
    {
        if (gameObject is object)
        {
            var collider = gameObject.GetComponent<Collider>();

            if (collider is object)
            {
                return collider.bounds.size;
            }
        }

        return Vector3.zero;
    }

    /// <summary>
    /// Returns the distance difference based on the current and a previous distance of two objects.
    /// </summary>
    /// <param name="previousDistance">Last distance recorded</param>
    /// <param name="firstObject">First object to compare</param>
    /// <param name="secondObject">Second object to compare</param>
    /// <returns></returns>
    public static float GetDistanceDelta(ref float previousDistance, GameObject firstObject, GameObject secondObject)
    {
        if (previousDistance == 0f)
        {
            previousDistance = Vector3.Distance(firstObject.transform.position, secondObject.transform.position);
            return 0f;
        }

        var distance = Vector3.Distance(firstObject.transform.position, secondObject.transform.position);

        if (previousDistance - distance > 0)
        {
            previousDistance = distance;
            return distance;
        }

        return 0f;
    }

    /// <summary>
    /// Gets the distance between two game objects.
    /// </summary>
    /// <param name="firstObject">First object to compare</param>
    /// <param name="secondObject">Second object to compare</param>
    /// <returns></returns>
    public static float GetDistance(GameObject firstObject, GameObject secondObject)
    {
        return Vector3.Distance(firstObject.transform.position, secondObject.transform.position);
    }
}
