using UnityEngine;
using UnityEngine.AI;

public class PlayerBot : MonoBehaviour
{
    public RabbitFeeder rabbitFeeder;
    public ThirdPersonController controller;
    public float moveRadius = 50f;
    public float moveSpeed = 5f;
    public float feedCooldown = 5f;

    private float feedTimer = 0f;
    private Vector3 targetPosition;
    private bool hasTarget = false;
    public bool isEnabled = true;

    void Start()
    {
        if (!isEnabled) return;
        PickNewTarget();
    }

    bool IsTrueNPercentOfTime(int n)
    {
        return Random.Range(0, 100) < n;
    }


    private bool isSprinting = false;
    private float sprintTimer = 0f;
    public float sprintDuration = 1f;  // Sprint lasts for 1 second

    void FixedUpdate()
    {
        if (!isEnabled) return;
        if (sprintTimer <= 0f && IsTrueNPercentOfTime(5))
        {
            isSprinting = true;
            sprintTimer = sprintDuration;
        }

        if (sprintTimer > 0f)
        {
            sprintTimer -= Time.fixedDeltaTime;
        }
        else
        {
            isSprinting = false;
        }
        Vector3 direction;
        Vector3 localDir;
        float horizontal;
        float vertical;
        if (!hasTarget)
        {
            // As if it were driven by the controller 
            Vector2 inputAxes = GetRandomInputAxes();
            direction = new Vector3(inputAxes.x, 0f, inputAxes.y);
            localDir = controller.cameraTransform.InverseTransformDirection(direction);
            horizontal = localDir.x;
            vertical = localDir.z;

            controller.Move(horizontal, vertical, isSprinting);
            CheckStuck();

            return;
        }

        direction = (targetPosition - transform.position).normalized;
        localDir = controller.cameraTransform.InverseTransformDirection(direction);
        horizontal = localDir.x;
        vertical = localDir.z;

        controller.Move(horizontal, vertical, isSprinting);
        CheckStuck();

        float distance = Vector3.Distance(transform.position, targetPosition);
        //print($"Distance between current pos {transform.position} and target pos {targetPosition} is {distance}");
        if (distance < 0.5f)
        {
            //print("Picking new target.");
            PickNewTarget();
        }
        feedTimer -= Time.fixedDeltaTime;
        if (feedTimer <= 0f)
        {
            rabbitFeeder.TryFeedRabbit();
            feedTimer = feedCooldown;
        }
    }
    private Vector3 lastPosition;
    private float stuckTimer = 0f;

    private void CheckStuck()
    {
        float distanceMoved = Vector3.Distance(transform.position, lastPosition);
        lastPosition = transform.position;

        if (distanceMoved < 0.1f)
        {
            stuckTimer += Time.deltaTime;
        }
        else
        {
            stuckTimer = 0f;
        }

        if (stuckTimer > 1f)  // Stuck for 1 second
        {
            Debug.Log("Detected stuck state, picking new target.");
            PickNewTarget();
            stuckTimer = 0f;
        }
    }
    private void SnapToGround()
    {
        Ray ray = new Ray(transform.position + Vector3.up * 0.5f, Vector3.down);

        if (Physics.Raycast(ray, out RaycastHit hit, 2f))
        {
            float groundY = hit.point.y;

            // Only correct if falling _below_ the ground
            if (transform.position.y < groundY - 0.1f)
            {
                Vector3 correctedPosition = new Vector3(transform.position.x, groundY, transform.position.z);
                transform.position = correctedPosition;
            }
        }
    }
    void LateUpdate()
    {
        SnapToGround();
    }




    Vector2 GetRandomInputAxes()
    {
        // Random direction on X/Z plane, simulating Horizontal/Vertical input
        Vector2 randomDirection = Random.insideUnitCircle.normalized;
        return randomDirection;  // x = Horizontal, y = Vertical
    }

    void PickNewTarget()
    {
        bool found = false;
        int attempts = 0;
        while (!found && attempts < 50)  // Prevent infinite loops
        {
            attempts++;
            Vector2 randomDirection = Random.insideUnitCircle.normalized;
            Vector2 edgePoint = randomDirection * moveRadius;
            Vector3 candidate = new(transform.position.x + edgePoint.x, transform.position.y, transform.position.z + edgePoint.y);
            /*print($"Generated randomDirection {randomDirection}.");
            print($"Edge point at {edgePoint}.");
            print($"Candidate is {candidate}.");*/

            /*if (NavMesh.SamplePosition(candidate, out NavMeshHit navHit, moveRadius, NavMesh.AllAreas))
            {
                targetPosition = navHit.position;
                hasTarget = true;
                return;
            }*/

            Vector3 rayOrigin = new(candidate.x, 100f, candidate.z);
            //print($"Trying candidate destination: {rayOrigin}");
            if (Physics.Raycast(rayOrigin, Vector3.down, out RaycastHit hit, 200f))
            {
                targetPosition = hit.point;
                hasTarget = true;
                return;
            }
        }

        // If still nothing after several tries, fallback to current position
        Debug.LogWarning("Could not find valid target after multiple attempts, staying still.");
        targetPosition = transform.position;
        hasTarget = true;
    }

}
