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

    public static float EvaluateProximity(ref float previousDistance, GameObject firstObject, GameObject secondObject)
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
}
