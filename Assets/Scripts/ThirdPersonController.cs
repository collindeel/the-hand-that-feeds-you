using System.Collections;
using UnityEngine;

public class ThirdPersonController : MonoBehaviour
{
    public Transform cameraRig;
    public float speed = 5f;
    public float sprintMultiplier = 2f;
    public float jumpForce = 5f;

    private Rigidbody rb;
    private bool isGrounded = true;

    private Animator animator;
    void Start()
    {
        rb = GetComponent<Rigidbody>();
        rb = GetComponent<Rigidbody>();
        animator = GetComponent<Animator>();
    }

    private bool isJumping = false;
    void Update()
    {
        float moveV = Input.GetAxis("Vertical");
        float rotate = Input.GetAxis("Horizontal");

        transform.Rotate(Vector3.up, rotate * 100f * Time.deltaTime);

        Vector3 move = transform.forward * moveV;

        float currentSpeed = speed * (Input.GetKey(KeyCode.LeftShift) ? sprintMultiplier : 1f);
        if (move.magnitude > 0.1f)
        {
            //transform.position += move * currentSpeed * Time.deltaTime;
            //float animSpeed = moveV * (Input.GetKey(KeyCode.LeftShift) ? sprintMultiplier : 1f);
            rb.MovePosition(rb.position + move * currentSpeed * Time.deltaTime);
            animator.SetFloat("Speed", moveV * currentSpeed);
        }
        else
        {
            animator.SetFloat("Speed", 0f);
        }

        if (Input.GetKey(KeyCode.Space) && isGrounded && !isJumping)
        {
            animator.SetTrigger("Jump");    
            StartCoroutine(DelayedJump(0.3f));
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
