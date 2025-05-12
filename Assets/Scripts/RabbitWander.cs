using UnityEngine;
using UnityEngine.AI;
using System.Collections;

public class RabbitWander : MonoBehaviour
{
    public float wanderRadius = 5f;
    public float waitTime = 2f;

    private NavMeshAgent agent;
    private Vector3 origin;
    private Animator animator;

    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        origin = transform.position;
        animator = GetComponent<Animator>();
        StartCoroutine(WanderRoutine());
    }

    IEnumerator WanderRoutine()
    {
        while (true)
        {
            Vector3 newPos = RandomNavSphere(origin, wanderRadius);
            agent.SetDestination(newPos);
            yield return new WaitForSeconds(waitTime);
        }
    }
    public static Vector3 RandomNavSphere(Vector3 origin, float dist)
    {
        Vector3 randomDirection = Random.insideUnitSphere * dist;
        randomDirection += origin;
        NavMeshHit navHit;
        NavMesh.SamplePosition(randomDirection, out navHit, dist, NavMesh.AllAreas);
        return navHit.position;
    }

    void Update()
    {
        if (animator != null)
        {
            bool isMoving = agent.velocity.magnitude > 0.1f;
            animator.SetBool("IsMoving", isMoving);
        }
    }
}
