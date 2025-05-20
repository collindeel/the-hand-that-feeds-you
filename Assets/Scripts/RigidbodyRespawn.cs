using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class RigidbodyRespawn : MonoBehaviour
{
    public float killY = -10f;     // “void” threshold
    public float offset = 1.5f;     // how high to pop up
    public LayerMask groundLayers = ~0; // everything

    Rigidbody rb;
    Vector3 lastGrounded;

    void Awake()
    {
        rb = GetComponent<Rigidbody>();
        lastGrounded = transform.position;
    }

    void FixedUpdate()
    {
        if (IsGrounded())
            lastGrounded = transform.position;

        if (transform.position.y < killY)
            Respawn();
    }

    bool IsGrounded()
    {
        float radius = 0.45f;
        return Physics.SphereCast(transform.position + Vector3.up * 0.05f,
                                  radius,
                                  Vector3.down,
                                  out _,
                                  0.1f,
                                  groundLayers,
                                  QueryTriggerInteraction.Ignore);
    }

    void Respawn()
    {
        // find floor under the last safe X-Z
        Vector3 probe = lastGrounded + Vector3.up * 50f;
        if (Physics.Raycast(probe, Vector3.down, out var hit, 100f, groundLayers))
            lastGrounded = hit.point;

        Vector3 safe = lastGrounded + Vector3.up * offset;

        rb.position = safe;
        rb.linearVelocity = Vector3.zero;
        rb.angularVelocity = Vector3.zero;
    }
}
