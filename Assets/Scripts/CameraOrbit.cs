using UnityEngine;
public class CameraOrbit : MonoBehaviour
{
    public Transform target;
    public float distance = 5f;
    public float height = 2.5f;
    public float mouseSensitivity = 3f;
    public Vector2 pitchLimits = new Vector2(-30, 60);

    private float yaw;
    private float pitch = 15f;

    void LateUpdate()
    {
        yaw += Input.GetAxis("Mouse X") * mouseSensitivity;
        pitch -= Input.GetAxis("Mouse Y") * mouseSensitivity;
        pitch = Mathf.Clamp(pitch, pitchLimits.x, pitchLimits.y);

        Quaternion rotation = Quaternion.Euler(pitch, yaw, 0f);
        Vector3 offset = rotation * new Vector3(0f, height, -distance);

        transform.position = target.position + offset;
        transform.LookAt(target.position + Vector3.up * 1.5f);
    }
}
