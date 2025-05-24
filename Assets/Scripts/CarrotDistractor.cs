using UnityEngine;
using UnityEngine.AI;
using Unity.MLAgents;
using Unity.MLAgents.Policies;

public class CarrotDistractor : MonoBehaviour
{
    [SerializeField] float distractionRadius = 20f;
    [SerializeField] float distractionTime   = 2f;

    NavMeshAgent nav;
    Agent mlAgent;
    BehaviorParameters bp;
    static Vector3 carrotPos;
    static bool carrotActive;

    void Awake()
    {
        nav      = GetComponent<NavMeshAgent>();
        mlAgent  = GetComponent<Agent>();
        bp       = GetComponent<BehaviorParameters>();
        nav.enabled = false;                    // let ML drive by default
    }

    void OnEnable()  => EpisodeEvents.OnCarrotThrown += HandleCarrot;
    void OnDisable() => EpisodeEvents.OnCarrotThrown -= HandleCarrot;

    void HandleCarrot(Vector3 pos)
    {
        if (Vector3.SqrMagnitude(pos - transform.position) > distractionRadius * distractionRadius)
            return;

        carrotPos    = pos;
        carrotActive = true;
        StartCoroutine(Distract());
    }

    System.Collections.IEnumerator Distract()
    {
        //-------------------------------------------------
        // 1. hand control to NavMesh
        //-------------------------------------------------
        mlAgent.enabled       = false;          // stop OnActionReceived
        nav.enabled           = true;
        nav.ResetPath();
        nav.SetDestination(carrotPos);

        float t = 0f;
        while (t < distractionTime && carrotActive)
        {
            if (!nav.pathPending && nav.remainingDistance < 0.2f) carrotActive = false;
            t += Time.deltaTime;
            yield return null;
        }

        //-------------------------------------------------
        // 2. return control to the ONNX policy
        //-------------------------------------------------
        nav.enabled      = false;
        mlAgent.enabled  = true;
        bp.BehaviorType  = BehaviorType.InferenceOnly;   // ensure policy drives
    }
}
