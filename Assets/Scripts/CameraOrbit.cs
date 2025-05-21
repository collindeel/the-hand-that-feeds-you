using UnityEngine;
using UnityEngine.InputSystem;

public class CameraOrbit : MonoBehaviour
{
    public Transform target;

    public float distance = 5f;
    public float height = 2.5f;
    public float mouseSensitivity = 2f;
    public float controllerSensitivity = 3f;

    public Vector2 pitchLimits = new Vector2(-20, 60);
    public float extraDownMargin = 5f;

    public LayerMask groundLayer = 1 << 0;

    float yaw;
    float pitch = 15f;

    void LateUpdate()
    {
        /* --- read input ---------------------------------------------------- */
        yaw += Input.GetAxis("Mouse X") * mouseSensitivity
               + (Gamepad.current?.rightStick.x.ReadValue() ?? 0f) * controllerSensitivity;

        pitch -= Input.GetAxis("Mouse Y") * mouseSensitivity
               + (Gamepad.current?.rightStick.y.ReadValue() ?? 0f) * controllerSensitivity;

        /* --- find ground slope under the player --------------------------- */
        Vector3 groundNormal = Vector3.up;   // fallback
        if (Physics.Raycast(target.position + Vector3.up * 0.3f,
                            Vector3.down, out RaycastHit hit, 2f, groundLayer))
            groundNormal = hit.normal;

        /* --- build a local frame tied to that slope ------------------------ */
        // tangent (right) axis:
        Vector3 slopeRight = Vector3.Cross(groundNormal, Vector3.forward);
        if (slopeRight.sqrMagnitude < 0.01f)   // forward nearly parallel? use X
            slopeRight = Vector3.Cross(groundNormal, Vector3.right);
        slopeRight.Normalize();
        // forward on the slope plane (used only for pitch clamping reference)
        Vector3 slopeForward = Vector3.Cross(slopeRight, groundNormal);

        /* --- calculate slope-relative pitch limits ------------------------- */
        float slopeAngleDown = Vector3.Angle(slopeForward, Vector3.ProjectOnPlane(Vector3.down, groundNormal));
        float minPitchRelSlope = -(slopeAngleDown + extraDownMargin); // e.g., -35°
        float minPitchWorld = Mathf.Max(pitchLimits.x, minPitchRelSlope);

        pitch = Mathf.Clamp(pitch, minPitchWorld, pitchLimits.y);

        /* --- apply orbit --------------------------------------------------- */
        Quaternion rotation = Quaternion.Euler(pitch, yaw, 0f);
        Vector3 offset = rotation * new Vector3(0f, height, -distance);

        transform.position = target.position + offset;
        transform.LookAt(target.position + Vector3.up * 1.5f);
    }
}
