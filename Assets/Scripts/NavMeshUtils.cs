using UnityEngine;
using UnityEngine.AI;

public static class NavMeshUtils
{
    public static bool enableDebugLogs = false;
    /// <summary>
    /// Gets a destination in a direction, snapping to nearest NavMesh if possible.
    /// </summary>
    /// <param name="origin">Start position.</param>
    /// <param name="direction">Direction to move (should be normalized).</param>
    /// <param name="distance">How far to move.</param>
    /// <returns>NavMesh position or positive infinity if invalid.</returns>
    public static Vector3 GetDirectionalTarget(Vector3 origin, Vector3 direction, float distance)
    {
        Vector3 destination = origin + direction * distance;
#if UNITY_EDITOR
        if (enableDebugLogs)
        {
            Debug.Log($"Distance scale of {distance}, scaled to {direction * distance}");
            Debug.Log($"Scalar distance: {Mathf.Sqrt((destination.x - origin.x) * (destination.x - origin.x) + (destination.y - origin.y) * (destination.y - origin.y))}");
        }
#endif
        if (NavMesh.SamplePosition(destination, out NavMeshHit navHit, distance, NavMesh.AllAreas))
        {
            return navHit.position;
        }
        else
        {
            return Vector3.positiveInfinity;
        }
    }

    /// <summary>
    /// Gets a position *toward* a target, snapping to nearest NavMesh.
    /// </summary>
    public static Vector3 GetPositionToward(Vector3 from, Vector3 to, float distance)
    {
        Vector3 direction = (to - from).normalized;
#if UNITY_EDITOR
        if (enableDebugLogs)
        {
            Debug.Log($"Normalized vector toward: {direction}");
        }
#endif
        return GetDirectionalTarget(from, direction, distance);
    }

    /// <summary>
    /// Gets a position *away from* a target, snapping to nearest NavMesh.
    /// </summary>
    public static Vector3 GetPositionAway(Vector3 from, Vector3 to, float distance)
    {
        Vector3 direction = (from - to).normalized;
        return GetDirectionalTarget(from, direction, distance);
    }
}
