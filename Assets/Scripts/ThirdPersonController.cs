using UnityEngine;

public class ThirdPersonController : MonoBehaviour
{
    public Transform cameraRig;
    public float speed = 5f;
    public float sprintMultiplier = 2f;
    public float jumpForce = 5f;

    private Rigidbody rb;
    private bool isGrounded = true;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
    }

    void Update()
    {
        float moveV = Input.GetAxis("Vertical");
        float rotate = Input.GetAxis("Horizontal");

        transform.Rotate(Vector3.up, rotate * 100f * Time.deltaTime);

        Vector3 move = transform.forward * moveV;

        if (move.magnitude > 0.1f)
        {
            float currentSpeed = speed * (Input.GetKey(KeyCode.LeftShift) ? sprintMultiplier : 1f);
            transform.position += move * currentSpeed * Time.deltaTime;

        }

        if (Input.GetKey(KeyCode.Space) && isGrounded)
        {
            rb.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
            isGrounded = false;
        }
    }

    void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Floor"))
        {
            isGrounded = true;
        }
    }
}
