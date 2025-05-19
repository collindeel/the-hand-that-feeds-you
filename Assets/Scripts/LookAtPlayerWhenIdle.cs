using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(NavMeshAgent))]
public class LookAtPlayerWhenIdle : MonoBehaviour
{
    [Tooltip("Graphic to rotate (drag the Rabbit child here). " +
             "Leave empty to rotate this GameObject.")]
    public Transform visual;

    [Tooltip("Optional: rotate just a sub-object (e.g. Eyes) for extra creepiness")]
    public Transform eyePivot;

    [Tooltip("How fast to yaw toward the player (deg / sec)")]
    public float turnSpeed = 360f;

    [Tooltip("Velocity below which the agent is considered idle")]
    public float idleThreshold = 0.2f;

    Transform player;
    NavMeshAgent agent;
    RabbitModelSwitcher rms;

    void Awake()
    {
        agent  = GetComponent<NavMeshAgent>();
        player = GameObject.FindGameObjectWithTag("Player")?.transform;
        rms    = GetComponent<RabbitModelSwitcher>();

        if (visual == null) visual = transform; // default to whole body
    }

    void LateUpdate()   // after navmesh has updated this frame
    {
        if (player == null) return;
        if (rms != null && rms.level != RabbitBehaviorLevel.Aggressive) return;

        // Is the agent basically stopped?
        if (agent.velocity.sqrMagnitude > idleThreshold * idleThreshold) return;

        // Where to face (yaw only)
        Vector3 target = player.position;
        target.y = visual.position.y;

        Quaternion targetRot = Quaternion.LookRotation(target - visual.position);

        visual.rotation = Quaternion.RotateTowards(
                            visual.rotation,
                            targetRot,
                            turnSpeed * Time.deltaTime);

        // OPTIONALLY rotate only the eyes a bit further for a “creepy stare”
        if (eyePivot != null)
        {
            Vector3 eyeTarget = player.position;
            eyeTarget.y = eyePivot.position.y;
            eyePivot.rotation = Quaternion.RotateTowards(
                                    eyePivot.rotation,
                                    Quaternion.LookRotation(eyeTarget - eyePivot.position),
                                    turnSpeed * 2f * Time.deltaTime);
        }
    }
}
