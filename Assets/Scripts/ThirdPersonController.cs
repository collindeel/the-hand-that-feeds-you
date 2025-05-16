using System.Collections;
using UnityEngine;

public class ThirdPersonController : MonoBehaviour
{
    public float speed = 5f;
    public float rotationSpeed = 10f;
    public float sprintMultiplier = 2f;
    public float jumpForce = 5f;

    private Rigidbody rb;
    private bool isGrounded = true;
    public Transform cameraTransform;
    public PlayerBot bot;

    private Animator animator;
    void Start()
    {
        rb = GetComponent<Rigidbody>();
        animator = GetComponent<Animator>();
        bot = GetComponent<PlayerBot>();
    }

    private bool isJumping = false;
    void Update()
    {
        if (bot.isEnabled) return;
        float horizontal = Input.GetAxis("Horizontal");
        float vertical = Input.GetAxis("Vertical");

        Move(horizontal, vertical, Input.GetKey(KeyCode.LeftShift));

        if (Input.GetKey(KeyCode.Space) && isGrounded && !isJumping)
        {
            animator.SetTrigger("Jump");
            StartCoroutine(DelayedJump(0.3f));
        }
    }

    public void Move(float horizontal, float vertical, bool isSprint)
    {
        // Get camera-relative direction
        Vector3 camForward = cameraTransform.forward;
        Vector3 camRight = cameraTransform.right;

        // Flatten the y-axis so movement stays level
        camForward.y = 0f;
        camRight.y = 0f;
        camForward.Normalize();
        camRight.Normalize();

        Vector3 moveDir = camForward * vertical + camRight * horizontal;
        float currentSpeed = speed * (isSprint ? sprintMultiplier : 1f);
        if (moveDir.magnitude > 0.1f)
        {
            Quaternion targetRotation = Quaternion.LookRotation(moveDir);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);
            rb.MovePosition(rb.position + moveDir * currentSpeed * Time.deltaTime);
            animator.SetFloat("Speed", moveDir.magnitude * currentSpeed);
        }
        else
        {
            animator.SetFloat("Speed", 0f);
        }

    }

    IEnumerator DelayedJump(float delay)
    {
        isJumping = true;
        yield return new WaitForSeconds(delay);
        rb.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
        isGrounded = false;
        isJumping = false;
    }

    void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Floor"))
        {
            isGrounded = true;
        }
    }
}
