using Unity.MLAgents;
using Unity.MLAgents.Actuators;
using Unity.MLAgents.Sensors;
using UnityEngine.AI;
using UnityEngine;
using Unity.MLAgents.Policies;
using Unity.VisualScripting;

public class RabbitAgent : Agent
{
    private Transform player;
    private NavMeshAgent agent;
    private Animator animator;
    private Transform nearestCarrot;
    private CarrotPlacement spawner; // yea i called it placement oops

    public bool enableObservations = false;

    public bool playerIsFeeding; // set externally when player feeds
    public Transform[] allRabbits;
    public float moveDistance = 30f;
    private float timeSinceLastMeal = 0f;
    public float hungerPenaltyRate = -0.005f;

    // Start is too late for grabbing the NMA
    public override void Initialize()
    {
        agent = GetComponent<NavMeshAgent>();
        animator = GetComponent<Animator>();
    }
    void Start()
    {
        if (!enableObservations)
        {
            BehaviorParameters behaviorParams = GetComponent<BehaviorParameters>();
            behaviorParams.BehaviorType = BehaviorType.HeuristicOnly;
        }
        if (player == null)
        {
            player = GameObject.FindGameObjectWithTag("Player").transform;
        }
        if (spawner == null)
        {
            spawner = FindFirstObjectByType<CarrotPlacement>();

            if (spawner == null)
            {
                Debug.LogError("RabbitAgent could not find a CarrotSpawner in the scene.");
            }
        }
    }

    private float satiationTimeRemaining = 0f;
    public float satiationDuration = 5f;

    void Update()
    {
        timeSinceLastMeal += Time.deltaTime;
        if (satiationTimeRemaining > 0f)
        {
            satiationTimeRemaining -= Time.deltaTime;
        }
        if (animator != null)
        {
            bool isMoving = agent.velocity.magnitude > 0.5f;
            animator.SetBool("IsMoving", isMoving);
        }
        //float distanceToPlayer = Vector3.Distance(transform.position, player.position);
        //Debug.Log($"Step {StepCount} | Distance to Player: {distanceToPlayer}");

    }
    public override void CollectObservations(VectorSensor sensor)
    {
        if (!enableObservations)
        {
            // Provide dummy data to maintain observation size
            for (int i = 0; i < 13; i++)
            {
                sensor.AddObservation(0f);
            }
            return;
        }
        if (player == null)
        {
            // Provide dummy data to keep observation size consistent
            sensor.AddObservation(Vector3.zero);  // Dummy direction
            sensor.AddObservation(999f);          // Arbitrarily large distance
            sensor.AddObservation(0f);            // Not feeding
            sensor.AddObservation(0f);            // No rabbits near player
            sensor.AddObservation(Vector3.zero);  // Dummy carrot direction
            sensor.AddObservation(999f);          // Arbitrarily large carrot distance
            sensor.AddObservation(Vector3.forward);  // Current forward direction
            return;
        }

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
            Vector3 dirToCarrot = (nearestCarrot.position - transform.position).normalized;
            float distanceToCarrot = Vector3.Distance(transform.position, nearestCarrot.position);
            sensor.AddObservation(dirToCarrot);
            sensor.AddObservation(distanceToCarrot);
        }
        else
        {
            // Provide dummy data if no carrot is found
            sensor.AddObservation(Vector3.zero);
            sensor.AddObservation(999f);
        }

        // 7. Current forward direction
        sensor.AddObservation(transform.forward);
    }

    public override void Heuristic(in ActionBuffers actionsOut)
    {
        var discreteActionsOut = actionsOut.DiscreteActions;
        discreteActionsOut[0] = 3; // Wander
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
        return count / 10f;
    }

    // Decision requester calls this every N frames
    public override void OnActionReceived(ActionBuffers actions)
    {
        if (player == null)
        {
            return;
        }
        if (satiationTimeRemaining <= 0f)
        {
            AddReward(hungerPenaltyRate * timeSinceLastMeal);
        }

        int action = actions.DiscreteActions[0];
        //print($"Action: {action}");
        switch (action)
        {
            case 0:
                MoveTowardPlayer();
                break;
            case 1:
                FleeFromPlayer();
                break;
            case 2:
                MoveTowardCarrot();
                break;
            case 3:
                WanderRandomly();
                break;
            case 4:
                StayStill();
                break;
        }

        ApplyStationaryPenalty();

        if (satiationDuration > 0f)
        {
            if (action == 4)
                AddReward(0.05f);
            else
                AddReward(-0.1f);
        }

        float distanceToPlayer = Vector3.Distance(transform.position, player.position);
        if (playerIsFeeding)
        {
            if (distanceToPlayer > 1.5f && distanceToPlayer < 4f)
            {
                AddReward(-0.005f);  // Reward cautious approach
            }
            else if (distanceToPlayer <= 1.5f)
            {
                AddReward(-0.1f);  // Penalize getting too close
            }
        }
        else
        {
            if (distanceToPlayer < 20f)
            {
                float reward = Mathf.Clamp01(1f - (distanceToPlayer / 20f)) * 0.1f;
                AddReward(-reward);
            }
        }

        if (nearestCarrot != null)
        {
            Vector3 dirToCarrot = (nearestCarrot.position - transform.position).normalized;
            float alignment = Vector3.Dot(transform.forward.normalized, dirToCarrot);
            AddReward(alignment * 0.01f);
            float distance = Vector3.Distance(transform.position, nearestCarrot.position);
            if (distance < 50f)
            {
                float reward = Mathf.Clamp01(1f - (distance / 20f)) * 0.1f;
                AddReward(reward);
            }
        }
    }

    private float stationaryTime = 0f;
    public float stationaryPenaltyRate = -0.01f;  // Penalty per second idle
    public float maxStationaryTime = 5f;          // Max allowed stationary time
    public float hardPenalty = -1.0f;

    private void ApplyStationaryPenalty()
    {
        if (agent.velocity.magnitude < 0.1f)
        {
            stationaryTime += Time.deltaTime;
            AddReward(stationaryPenaltyRate * Time.deltaTime);

            if (stationaryTime >= maxStationaryTime)
            {
                AddReward(hardPenalty);
                //EndEpisode();  // Optional
                stationaryTime = 0f;  // Reset to prevent repeat penalties
            }
        }
        else
        {
            stationaryTime = 0f;  // Reset if the agent moves
        }
    }


    private Transform GetNearestCarrot()
    {

        Transform nearest = null;
        float minDistance = Mathf.Infinity;

        GameObject[] thrownCarrots = GameObject.FindGameObjectsWithTag("ThrownCarrot");
        foreach (var carrotObj in thrownCarrots)
        {
            float distance = Vector3.Distance(transform.position, carrotObj.transform.position);
            if (distance < minDistance)
            {
                minDistance = distance;
                nearest = carrotObj.transform;
            }
        }

        return nearest;
    }

    public void Move(Vector3 dest)
    {
        if (IsVectorValid(dest))
        {
            agent.SetDestination(dest);
            if (agent.velocity.sqrMagnitude > 0.1f)
            {
                Quaternion lookRotation = Quaternion.LookRotation(agent.velocity.normalized);
                transform.rotation = Quaternion.Slerp(transform.rotation, lookRotation, agent.angularSpeed * Time.deltaTime);
            }

        }
    }

    bool IsVectorValid(Vector3 v)
    {
        return !(float.IsInfinity(v.x) || float.IsInfinity(v.y) || float.IsInfinity(v.z));
    }

    public void MoveTowardCarrot()
    {
        if (nearestCarrot != null) // If null, it thought there was a carrot on the last tick that no longer exists
        {
            Vector3 dest = NavMeshUtils.GetPositionToward(transform.position, nearestCarrot.position, moveDistance);
            Move(dest);
        }
    }
    private bool isFrozen = false;

    public void StopAllMovement()
    {
        if (isFrozen)
            return;

        if (agent.hasPath)
            agent.ResetPath();

        agent.isStopped = true;

        Rigidbody rb = GetComponent<Rigidbody>();
        rb.linearVelocity = Vector3.zero;
        rb.angularVelocity = Vector3.zero;
        rb.isKinematic = true;

        isFrozen = true;
    }

    public void ResumeMovement()
    {
        if (isFrozen)
        {
            Rigidbody rb = GetComponent<Rigidbody>();
            rb.isKinematic = false;

            agent.isStopped = false;
            isFrozen = false;
        }
    }

    public void StayStill()
    {
        StopAllMovement();
    }

    public void MoveTowardPlayer()
    {
        ResumeMovement();
        Vector3 dest = NavMeshUtils.GetPositionToward(transform.position, player.position, moveDistance);
        Move(dest);
    }

    public void FleeFromPlayer()
    {
        ResumeMovement();
        Vector3 dest = NavMeshUtils.GetPositionAway(transform.position, player.position, moveDistance);
        Move(dest);
    }

    public void WanderRandomly()
    {
        ResumeMovement();
        Vector2 randomCircle = UnityEngine.Random.insideUnitCircle.normalized; // Scale for good measure, parity w dist for now        
        Vector3 randomOffset = new Vector3(randomCircle.x, 0f, randomCircle.y);
        //print($"Random offset is {randomOffset}");
        Vector3 candidate = transform.position + randomOffset;
        //print($"Position rabbit: {transform.position}, position toward: {candidate}");
        Vector3 dest = NavMeshUtils.GetPositionToward(transform.position, candidate, moveDistance);
        //print($"dest: {dest}");
        CommitTo(dest);
    }

    private Vector3? committedTarget = null;
    private float commitTimer = 0f;
    public float commitDuration = 5f;

    private void CommitTo(Vector3 target)
    {
        if (commitTimer <= 0f || committedTarget == null)
        {
            committedTarget = target;
            commitTimer = commitDuration;
            Move(committedTarget.Value);
        }
        else
            commitTimer -= Time.deltaTime;
    }


    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("ThrownCarrot"))
        {
            //print("A rabbit got a *thrown* carrot!");
            AddReward(1.0f);
            Destroy(other.gameObject);
            timeSinceLastMeal = 0f;
            satiationTimeRemaining = satiationDuration;
        }
    }

}
