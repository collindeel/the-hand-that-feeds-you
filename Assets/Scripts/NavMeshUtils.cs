using UnityEngine;
using UnityEngine.AI;
public static class NavMeshUtils
{

    /// <summary>
    /// Gets a destination position,
    /// snapping to the nearest NavMesh point if possible.
    /// </summary>
    /// <param name="from">Tail of vector.</param>
    /// <param name="to">Head of vector.</param>
    /// <param name="distance">The range to sample.</param>
    /// <returns>The nearest valid NavMesh position or if none found, positive infinity.</returns>
    public static Vector3 GetTargetPosition(Vector3 from, Vector3 to, float distance)
    {
        Vector3 direction = (to - from).normalized;
        Vector3 destination = from + direction * distance;

        if (NavMesh.SamplePosition(destination, out NavMeshHit navHit, distance, NavMesh.AllAreas))
        {
            return navHit.position;
        }
        else
        {
            return Vector3.positiveInfinity;
        }
    }
}
