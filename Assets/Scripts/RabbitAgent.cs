using Unity.MLAgents;
using Unity.MLAgents.Actuators;
using Unity.MLAgents.Sensors;
using UnityEngine.AI;
using UnityEngine;

[System.Serializable]
public class RabbitStats
{
    public float acceleration = 6f;
    public float moveDistance = 30f;
    public float stoppingDistance = .2f;
    public float speed = 8f;
}

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
    private PlayerBot playerBot;
    public RabbitStats timidStats;
    public RabbitStats mediumStats;
    public RabbitStats aggressiveStats;
    public RabbitModelSwitcher rms;

    // Start is too late for grabbing the NMA
    public override void Initialize()
    {
        base.Initialize();
        agent = GetComponent<NavMeshAgent>();
        animator = GetComponent<Animator>();
    }
    protected override void Awake()
    {
        base.Awake();
        rms = GetComponent<RabbitModelSwitcher>();
    }
    void Start()
    {
        ApplyStats(rms.level);

        GameObject playerObject = GameObject.FindGameObjectWithTag("Player");
        if (playerObject != null)
        {
            playerBot = playerObject.GetComponent<PlayerBot>();
        }
        else
        {
            Debug.LogError("Player object with PlayerBot not found in the scene.");
        }
        if (!enableObservations)
        {
            rms.HeuristicOnly();
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

    const float idleCutoff = 0.3f;
    const float runBlendStart = 0.6f;
    const float runNaturalSpeed = 12f / 11f;
    const float idleCutoffSq = idleCutoff * idleCutoff;
    const float runBlendRange = runNaturalSpeed - runBlendStart;

    void Update()
    {
        timeSinceLastMeal += Time.deltaTime;
        if (satiationTimeRemaining > 0f)
        {
            satiationTimeRemaining -= Time.deltaTime;
        }
        if (animator != null)
        {
            float sq = agent.velocity.sqrMagnitude;
            float v = sq > idleCutoffSq ? Mathf.Sqrt(sq) : 0f;

            float param = (v <= runBlendStart) ? 0f
                        : Mathf.Clamp01((v - runBlendStart) / runBlendRange);

            animator.SetFloat("Speed", param);

            if (v > runNaturalSpeed)
                animator.speed = v / runNaturalSpeed;
            else
                animator.speed = 1f;
        }

    }
    public void ApplyStats(RabbitBehaviorLevel level)
    {
        RabbitStats s = timidStats; // Also works for "heuristic"
        if (level == RabbitBehaviorLevel.Medium) s = mediumStats;
        else if (level == RabbitBehaviorLevel.Aggressive) s = aggressiveStats;

        agent.acceleration = s.acceleration;
        agent.speed = s.speed;
        agent.stoppingDistance = s.stoppingDistance;
        moveDistance = s.moveDistance;
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
        if (rms.isIdleOnly)
            discreteActionsOut[0] = 4;
        else
        {
            int choice = UnityEngine.Random.Range(0, 1);
            discreteActionsOut[0] = choice == 0 ? 3 : 4;
        }
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

    public void ActionMedium(int action)
    {
        ApplyStationaryPenalty(0.5f);

        if (satiationTimeRemaining <= 0f)
        {
            AddReward(hungerPenaltyRate * timeSinceLastMeal);
        }
        else
        {
            if (action == 1) // After taking carrot, really encouraging fleeing here
                AddReward(1.0f);
            else
                AddReward(-0.1f);
        }

        float distanceToPlayer = Vector3.Distance(transform.position, player.position);
        if (playerIsFeeding)
        {
            if (distanceToPlayer > 50f)
            {
                AddReward(-0.2f); // They're too far from the player
            }
            else if (distanceToPlayer <= 10f)
            {
                AddReward(1.0f);
            }
        }
        else
        {
            if (distanceToPlayer < 400f && distanceToPlayer > 50f) // Reasonably within sight or smell range, but too far away for reward
            {
                float reward = Mathf.Clamp01(1f - (distanceToPlayer / 400f)) * 0.1f;
                AddReward(-reward);
            }
            else if (distanceToPlayer <= 50f && distanceToPlayer > 10f)
            {
                float normalized = Mathf.InverseLerp(50f, 2f, distanceToPlayer);
                AddReward(normalized * 1f);
            }
        }

        if (nearestCarrot != null)
        {
            Vector3 dirToCarrot = (nearestCarrot.position - transform.position).normalized;
            float alignment = Vector3.Dot(transform.forward.normalized, dirToCarrot);
            AddReward(alignment * 0.0025f);

            float distance = Vector3.Distance(transform.position, nearestCarrot.position);
            if (distance < 50f)
            {
                float reward = Mathf.Clamp01(1f - (distance / 20f)) * 0.025f;
                AddReward(reward);
            }
        }

    }
    public void ActionAggressive(int action)
    {
        ApplyStationaryPenalty(1.0f);

        if (satiationTimeRemaining <= 0f)
        {
            AddReward(hungerPenaltyRate * timeSinceLastMeal);
        }
        else
        {
            if (action == 4 || action == 1) // Can flee or hold still, without this they'd be rabid
                AddReward(0.2f);
            else
                AddReward(-0.05f);
        }

        float distanceToPlayer = Vector3.Distance(transform.position, player.position);
        if (playerIsFeeding)
        {
            if (distanceToPlayer > 50f)
            {
                AddReward(-0.2f); // They're too far from the player
            }
            else if (distanceToPlayer <= 10f)
            {
                AddReward(1.0f);
            }
        }
        else
        {
            if (distanceToPlayer < 400f && distanceToPlayer > 50f) // Reasonably within sight or smell range, but too far away for reward
            {
                float reward = Mathf.Clamp01(1f - (distanceToPlayer / 400f)) * 0.1f;
                AddReward(-reward);
            }
            else if (distanceToPlayer <= 50f && distanceToPlayer > 10f)
            {
                float normalized = Mathf.InverseLerp(50f, 2f, distanceToPlayer);
                AddReward(normalized * 1f);
            }
        }

        if (nearestCarrot != null)
        {
            Vector3 dirToCarrot = (nearestCarrot.position - transform.position).normalized;
            float alignment = Vector3.Dot(transform.forward.normalized, dirToCarrot);
            AddReward(alignment * 0.0025f);

            float distance = Vector3.Distance(transform.position, nearestCarrot.position);
            if (distance < 50f)
            {
                float reward = Mathf.Clamp01(1f - (distance / 20f)) * 0.025f;
                AddReward(reward);
            }
        }

    }
    public void ActionTimid(int action)
    {
        ApplyStationaryPenalty(0.1f);

        if (satiationTimeRemaining <= 0f)
        {
            AddReward(hungerPenaltyRate * timeSinceLastMeal);
        }
        else
        {
            if (action == 4)
                AddReward(0.05f);
            else
                AddReward(-0.1f);
        }

        float distanceToPlayer = Vector3.Distance(transform.position, player.position);
        if (playerIsFeeding)
        {
            if (distanceToPlayer > 1.5f && distanceToPlayer < 6f)
            {
                AddReward(0.05f);  // Reward cautious approach
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

    // Decision requester calls this every N frames
    public override void OnActionReceived(ActionBuffers actions)
    {
        // This should all be common across all models
        if (player == null)
        {
            return;
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
        return; // Uncomment this for training
#if UNITY_EDITOR
        switch (rms.level)
        {
            case RabbitBehaviorLevel.Timid:
                ActionTimid(action);
                break;
            case RabbitBehaviorLevel.Medium:
                ActionMedium(action);
                break;
            case RabbitBehaviorLevel.Aggressive:
                ActionAggressive(action);
                break;
        }
#endif
    }

    private float stationaryTime = 0f;
    public float stationaryPenaltyRate = -0.01f;  // Penalty per second idle
    public float stationaryPenaltyRateAggressive = -0.10f;  // Penalty per second idle
    public float maxStationaryTime = 5f;          // Max allowed stationary time
    public float hardPenalty = -1.0f;

    private void ApplyStationaryPenalty(float minMag)
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

    public void CancelSmallDestination()
    {
        float minMoveDistance = 1.5f;
        Vector3 targetPosition = agent.destination;

        float distanceToTarget = Vector3.Distance(agent.transform.position, targetPosition);

        if (distanceToTarget < minMoveDistance)
        {
            agent.ResetPath();
            animator.SetBool("IsMoving", false);
        }
    }

    public void Move(Vector3 dest)
    {
        if (IsVectorValid(dest))
        {
            agent.SetDestination(dest);
            CancelSmallDestination();

            if (agent.velocity.sqrMagnitude > 0.1f) // ~.316 m/s
            {
                Quaternion lookRotation = Quaternion.LookRotation(agent.velocity.normalized);
                float maxRotationPerFrame = agent.angularSpeed * Time.deltaTime;
                transform.rotation = Quaternion.RotateTowards(transform.rotation, lookRotation, maxRotationPerFrame);
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
        animator.SetBool("IsMoving", false);
        if (isFrozen)
            return;

        if (agent.hasPath)
            agent.ResetPath();

        agent.isStopped = true;

        //Rigidbody rb = GetComponent<Rigidbody>();
        //rb.linearVelocity = Vector3.zero;
        //rb.angularVelocity = Vector3.zero;
        //rb.isKinematic = true;

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
    private void SatiateRabbit()
    {
        timeSinceLastMeal = 0f;
        satiationTimeRemaining = satiationDuration;
    }
    private bool TakeCarrot(RabbitFeeder rf)
    {
        return rf.TryFeedRabbit(false);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("ThrownCarrot"))
        {
            //print("A rabbit got a *thrown* carrot!");
#if UNITY_EDITOR
            AddReward(1.0f);
#endif
            Destroy(other.gameObject);
            SatiateRabbit();
        }
        else if (other.CompareTag("Player"))
        {
            if (rms.level == RabbitBehaviorLevel.Aggressive)
            {
                if (agent.velocity.sqrMagnitude < 0.01f) return; // sqrMagnitude for performance, do not alter
#if UNITY_EDITOR
                AddReward(10f);
#endif
                PlayerHealth playerHealth = other.GetComponent<PlayerHealth>();
                if (playerHealth != null && !playerHealth.IsImmune() && !playerBot.isEnabled)
                {
                    playerHealth.TakeDamage(10);
                    SatiateRabbit();
                }
            }
            else if (rms.level == RabbitBehaviorLevel.Medium)
            {
                if (agent.velocity.magnitude < 0.1f) return;
#if UNITY_EDITOR
                AddReward(2f);
#endif
                PlayerHealth playerHealth = other.GetComponent<PlayerHealth>();
                playerHealth.TakeDamage(0);
                bool wasCarrotTaken = TakeCarrot(other.GetComponent<RabbitFeeder>());
                if (wasCarrotTaken)
                {
                    GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
                    Transform playerHudTransform = playerObj.transform.Find("CameraRig/PlayerHUD");
                    if (playerHudTransform != null)
                    {
                        TookCarrotPopupController popupController = playerHudTransform.GetComponent<TookCarrotPopupController>();
                        if (popupController != null)
                        {
                            popupController.ShowPopup();
                        }
                        ScorePopupController scorePC = playerHudTransform.GetComponent<ScorePopupController>();
                        ScoreTracker.AddScore(-50);
                        if (!ScoreTracker.isScoreDisabled)
                            scorePC.ShowPopup(ScoreTracker.GetScore());
                    }
                    SatiateRabbit();
                }
            }
        }
    }

}
