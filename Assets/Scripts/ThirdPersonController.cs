using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

public class ThirdPersonController : MonoBehaviour
{
    [Header("Motion")]
    public float speed = 5f;
    public float rotationSpeed = 10f;
    public float sprintMultiplier = 2f;
    public float jumpForce = 5f;

    [Header("Refs")]
    public Transform cameraTransform;

    Rigidbody rb;
    PlayerBot bot;

    Animator animator;

    InputAction _sprintAction;
    InputAction _jumpAction;
    // Cached input
    float cachedH, cachedV;
    bool cachedSprint;
    bool jumpRequested;

    bool isGrounded = true;
    bool isJumping = false;
    Vector3 groundNormal = Vector3.up;
    [SerializeField] float maxAcceptableDistToGround = 1.5f;
    [SerializeField] LayerMask groundLayer = 1 << 8;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        animator = GetComponent<Animator>();
        bot = GetComponent<PlayerBot>();
        _sprintAction = InputSystem.actions.FindAction("Sprint");
        _jumpAction = InputSystem.actions.FindAction("Jump");
    }

    void Update()
    {
        if (bot.isEnabled) return;

        cachedH = Input.GetAxisRaw("Horizontal");
        cachedV = Input.GetAxisRaw("Vertical");
        cachedSprint = _sprintAction.IsPressed();

        if (_jumpAction.WasPressedThisFrame() && isGrounded && !isJumping)
        {
            jumpRequested = true;
            animator.SetTrigger("Jump");
        }
    }

    void FixedUpdate()
    {
        Vector3 feetPos = rb.position + Vector3.up;
        bool hitGround = Physics.Raycast(
                            feetPos, Vector3.down,
                            out RaycastHit hit, maxAcceptableDistToGround,
                            groundLayer, QueryTriggerInteraction.Ignore);

        isGrounded = hitGround;
        groundNormal = hitGround ? hit.normal : Vector3.up;
        animator.SetBool("Grounded", isGrounded);

        Move(cachedH, cachedV, cachedSprint);

        if (jumpRequested)
        {
            StartCoroutine(DelayedJump(0.3f));
            jumpRequested = false;
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

        Vector3 rawMove = camForward * vertical + camRight * horizontal;

        Vector3 moveDir = Vector3.ProjectOnPlane(rawMove, groundNormal).normalized;
        //Vector3 moveDir = camForward * vertical + camRight * horizontal;
        float currentSpeed = speed * (isSprint ? sprintMultiplier : 1f);
        if (moveDir.magnitude > 0.1f)
        {
            Quaternion targetRotation = Quaternion.LookRotation(moveDir);
            rb.MoveRotation(Quaternion.Slerp(rb.rotation, targetRotation, rotationSpeed * Time.fixedDeltaTime));
            rb.MovePosition(rb.position + moveDir * currentSpeed * Time.fixedDeltaTime);
            animator.SetFloat("Speed", moveDir.magnitude * currentSpeed);
        }
        else
        {
            animator.SetFloat("Speed", 0f);

            if (isGrounded)
                rb.linearVelocity = Vector3.Project(rb.linearVelocity, groundNormal);
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
    void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.layer == LayerMask.NameToLayer("Foliage"))
        {
            Collider playerCollider = GetComponent<Collider>();
            Physics.IgnoreCollision(playerCollider, other, true);
        }
    }
}
