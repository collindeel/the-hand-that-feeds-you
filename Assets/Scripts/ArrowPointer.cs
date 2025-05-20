using UnityEngine;

public class ArrowPointer : MonoBehaviour
{
    [Tooltip("What should the arrow point at?")]
    public Transform objective;

    [Tooltip("Should the arrow ignore vertical difference?")]
    public bool flatOnY = true;

    [Tooltip("Optional extra roll so the arrow stays flat")]
    public bool keepUpright = true;

    void LateUpdate()
    {
        if (objective == null) return;

        // direction from arrow to goal
        Vector3 dir = objective.position - transform.position;

        if (flatOnY) dir.y = 0f;          // remove pitch so arrow yaws only

        if (dir.sqrMagnitude < 0.01f) return;  // already on top → skip

        Quaternion lookRot = Quaternion.LookRotation(dir);

        if (keepUpright)
            lookRot = Quaternion.Euler(0f, lookRot.eulerAngles.y, 0f);

        transform.rotation = lookRot;
    }
}
