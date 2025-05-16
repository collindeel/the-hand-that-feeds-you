using Unity.MLAgents;
using Unity.MLAgents.Actuators;
using Unity.MLAgents.Sensors;
using UnityEngine.AI;
using UnityEngine;
using System.Collections;

public class RabbitAgent : Agent
{
    private int[] counts;
    private int tick = 0;
    private Transform player;
    private NavMeshAgent agent;
    private Animator animator;
    private Transform nearestCarrot;
    private CarrotPlacement spawner; // yea i called it placement oops

    public bool playerIsFeeding; // set externally when player feeds
    public Transform[] allRabbits;
    public float moveDistance = 2f;
    private float timeSinceLastMeal = 0f;
    public float hungerPenaltyRate = -0.005f;

    // Start is too late for grabbing the NMA
    public override void Initialize()
    {
        agent = GetComponent<NavMeshAgent>();
        animator = GetComponent<Animator>();

        counts = new int[5];
    }
    void Start()
    {
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
        tick++;
        timeSinceLastMeal += Time.deltaTime;
        if (satiationTimeRemaining > 0f)
        {
            satiationTimeRemaining -= Time.deltaTime;
        }
        if (animator != null)
        {
            bool isMoving = agent.velocity.magnitude > 0.1f;
            animator.SetBool("IsMoving", isMoving);
        }
        //float distanceToPlayer = Vector3.Distance(transform.position, player.position);
        //Debug.Log($"Step {StepCount} | Distance to Player: {distanceToPlayer}");

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
            Vector3 dirToCarrot = (nearestCarrot.position - transform.position).normalized;
            float distanceToCarrot = Vector3.Distance(transform.position, nearestCarrot.position);
            sensor.AddObservation(dirToCarrot);
            sensor.AddObservation(distanceToCarrot);
        }
        else
        {
            // Provide dummy data if no carrot is found
            sensor.AddObservation(Vector3.zero);
            sensor.AddObservation(9999f);
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
        if (satiationTimeRemaining <= 0f)
        {
            AddReward(hungerPenaltyRate * timeSinceLastMeal);
        }

        int action = actions.DiscreteActions[0];
        //foreach (int a in actions.DiscreteActions)
        //    print($"Action: {a}");

        /*counts[action]++;
        print($"Tick: {tick}");
        print($"To p: {counts[0]}");
        print($"From p: {counts[1]}");
        print($"To c: {counts[2]}");
        print($"Wander: {counts[3]}");
        print($"Idle: {counts[4]}");*/
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
    public LayerMask carrotLayer;

    private Transform GetNearestCarrot()
    {
        Transform nearest = null;
        float minDistance = Mathf.Infinity;

        foreach (var carrot in spawner.spawnedCarrots)
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
        if (IsVectorValid(dest))
        {
            agent.SetDestination(dest);
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

    public void StayStill()
    {
        if (agent.hasPath)
            agent.ResetPath();
    }

    public void MoveTowardPlayer()
    {
        Vector3 dest = NavMeshUtils.GetPositionToward(transform.position, player.position, moveDistance);
        Move(dest);
    }

    public void FleeFromPlayer()
    {
        Vector3 dest = NavMeshUtils.GetPositionAway(transform.position, player.position, moveDistance);
        Move(dest);
    }

    public void WanderRandomly()
    {
        Vector3 randomTargetPoint = Random.onUnitSphere + transform.position;
        Vector3 dest = NavMeshUtils.GetPositionToward(transform.position, randomTargetPoint, moveDistance);
        Move(dest);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("ThrownCarrot"))
        {
            print("A rabbit got a *thrown* carrot!");
            AddReward(1.0f);
            spawner.spawnedCarrots.Remove(other.transform);
            Destroy(other.gameObject);
            timeSinceLastMeal = 0f;
            satiationTimeRemaining = satiationDuration;
        }
    }

}
