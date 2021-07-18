using UnityEngine;
/// <summary>
/// This is a very basic pathfinder used for the manually programmed approach (non-ML).
/// </summary>
public static class BasicPathfinder
{
    public static float[] GetDirection(Vector3 origin, Vector3 destination)
    {
        float[] direction = new float[2] { 0f, 0f };

        // get X
        if ((int)(origin.x - destination.x) > 0)
        {
            direction[0] = -1f;
        }
        else if ((int)(origin.x - destination.x) < 0)
        {
            direction[0] = 1f;
        }

        // get Z
        if ((int)(origin.z - destination.z) > 0)
        {
            direction[1] = -1f;
        }
        else if ((int)(origin.z - destination.z) < 0)
        {
            direction[1] = 1f;
        }

        return direction;
    }
}
