using Unity.MLAgents;
using Unity.MLAgents.Actuators;
using Unity.MLAgents.Sensors;
using UnityEngine.AI;
using UnityEngine;

public class RabbitAgent : Agent
{
    public Transform player;
    private NavMeshAgent agent;
    private Animator animator;
    public Transform[] carrots;
    public Transform nearestCarrot;
    public bool playerIsFeeding; // set externally when player feeds
    public Transform[] allRabbits;
    public float moveDistance = 2f;
    private float timeSinceLastMeal = 0f;
    public float hungerPenaltyRate = -0.01f;

    // Start is too late for grabbing the NMA
    public override void Initialize()
    {
        agent = GetComponent<NavMeshAgent>();
        animator = GetComponent<Animator>();
    }

    void Update()
    {
        timeSinceLastMeal += Time.deltaTime;
        if (animator != null)
        {
            bool isMoving = agent.velocity.magnitude > 0.1f;
            animator.SetBool("IsMoving", isMoving);
        }
    }
    public override void CollectObservations(VectorSensor sensor)
    {
        // 1. Direction toward player
        // 2. Distance to player
        Vector3 toPlayer = (player.position - transform.position).normalized;
        float distanceToPlayer = Vector3.Distance(transform.position, player.position);
        sensor.AddObservation(toPlayer);
        sensor.AddObservation(distanceToPlayer);

        // 3. Feeding signal
        sensor.AddObservation(playerIsFeeding ? 1f : 0f);

        // 4. Nearby rabbits
        float nearbyRabbits = CountRabbitsNearPlayer();
        sensor.AddObservation(nearbyRabbits);

        // 5. Direction to nearest carrot
        // 6. Distance to nearest carrot
        nearestCarrot = GetNearestCarrot();

        if (nearestCarrot != null)
        {
            Vector3 toCarrot = (nearestCarrot.position - transform.position).normalized;
            float distanceToCarrot = Vector3.Distance(transform.position, nearestCarrot.position);

            sensor.AddObservation(toCarrot);
            sensor.AddObservation(distanceToCarrot);
        }
        else
        {
            // Provide dummy data if no carrot is found
            sensor.AddObservation(Vector3.zero);
            sensor.AddObservation(0f);
        }

        // 7. Current forward direction
        sensor.AddObservation(transform.forward);
    }

    public override void Heuristic(in ActionBuffers actionsOut)
    {
        var discreteActionsOut = actionsOut.DiscreteActions;
        discreteActionsOut[0] = 4; // "StayStill" action
    }

    private float CountRabbitsNearPlayer()
    {
        int count = 0;
        foreach (Transform rabbit in allRabbits)
        {
            if (rabbit == transform) continue;  // Don't count self
            if (Vector3.Distance(rabbit.position, player.position) < 5f)
                count++;
        }
        return count / 10f;  // Normalize if needed
    }

    // Decision requester calls this every N frames
    public override void OnActionReceived(ActionBuffers actions)
    {
        AddReward(hungerPenaltyRate * timeSinceLastMeal);
        int action = actions.DiscreteActions[0];

        switch (action)
        {
            case 0: MoveTowardPlayer(); break;
            case 1: FleeFromPlayer(); break;
            case 2: MoveTowardCarrot(); break;
            case 3: WanderRandomly(); break;
            case 4: StayStill(); break;
        }

        // Constant step penalty to encourage efficiency
        AddReward(-0.001f);

        if (playerIsFeeding)
        {
            float distanceToPlayer = Vector3.Distance(transform.position, player.position);

            if (distanceToPlayer > 1.5f && distanceToPlayer < 4f)
            {
                AddReward(0.1f);  // Reward cautious approach
            }
            else if (distanceToPlayer <= 1.5f)
            {
                AddReward(-0.1f);  // Penalize getting too close
            }
        }

        if (IsTouchingCarrot())
        {
            AddReward(1.0f);
            timeSinceLastMeal = 0f;
        }
    }

    public float carrotTouchRadius = 0.5f;
    public LayerMask carrotLayer;

    private bool IsTouchingCarrot()
    {
        Collider[] hits = Physics.OverlapSphere(transform.position, carrotTouchRadius, carrotLayer);
        return hits.Length > 0;
    }

    private Transform GetNearestCarrot()
    {
        Transform nearest = null;
        float minDistance = Mathf.Infinity;

        foreach (Transform carrot in carrots)
        {
            float distance = Vector3.Distance(transform.position, carrot.position);
            if (distance < minDistance)
            {
                minDistance = distance;
                nearest = carrot;
            }
        }

        return nearest;
    }

    public void Move(Vector3 dest)
    {
        if (dest != Vector3.positiveInfinity)
            agent.SetDestination(dest);
    }

    public void MoveTowardCarrot()
    {
        if (nearestCarrot != null) // If null, it thought there was a carrot on the last tick that no longer exists
        {
            Vector3 dest = NavMeshUtils.GetTargetPosition(transform.position, nearestCarrot.position, moveDistance);
            Move(dest);
        }
    }

    public void StayStill()
    {
        if (agent.hasPath)
            agent.ResetPath();
    }


    public void MoveTowardPlayer()
    {
        Vector3 dest = NavMeshUtils.GetTargetPosition(transform.position, player.position, moveDistance);
        Move(dest);
    }

    // Give the player a false sense of security mwahaha
    public void FleeFromPlayer()
    {
        Vector3 dest = NavMeshUtils.GetTargetPosition(player.position, transform.position, moveDistance);
        Move(dest);
    }

    public void WanderRandomly()
    {
        Vector3 randomTargetPoint = Random.onUnitSphere + transform.position;
        Vector3 dest = NavMeshUtils.GetTargetPosition(transform.position, randomTargetPoint, moveDistance);
        Move(dest);
    }

}
