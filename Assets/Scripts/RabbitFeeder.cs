using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class RabbitFeeder : MonoBehaviour
{
    public float feedRadius = 3f;
    public LayerMask rabbitLayer;
    public CarrotInventory inventory;
    public RabbitAgent[] allRabbits;
    public GameObject carrotPrefab;
    public PlayerBot bot;
    public ScorePopupController scorePC;
    private AudioSource audioSource;
    public AudioClip tossClip;
    public AudioClip twinkleClip;
    public AudioClip takeClip;
    bool trainingMode = false;
    public float thrownCarrotScale = 0.25f;
    public float feedRange = 2f;
    public float tossForce = 5f;

    void Start()
    {
        bot = GetComponent<PlayerBot>();
        audioSource = GetComponent<AudioSource>();
        trainingMode = bot.isEnabled;
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.F) || (Gamepad.current?.buttonWest.wasPressedThisFrame ?? false))
        {
            bool wasFed = TryFeedRabbit();
            if (wasFed)
            {
                ScoreTracker.AddScore(50);
                if (!ScoreTracker.isScoreDisabled)
                    scorePC.ShowPopup(ScoreTracker.GetScore());
            }
        }
    }

    public bool TryFeedRabbit()
    {
        return TryFeedRabbit(true);
    }

    public bool TryFeedRabbit(bool doThrow)
    {
        if (!trainingMode && inventory.CarrotCount <= 0)
        {
            //Debug.Log("No carrots to feed.");
            return false;
        }

        // Limit by range or LOS?
        foreach (RabbitAgent rabbit in allRabbits)
        {
            rabbit.playerIsFeeding = true;
        }

        // Optionally reset after some time
        StartCoroutine(ResetFeedingSignalAfterDelay(5f));

        Collider[] rabbits = Physics.OverlapSphere(transform.position, feedRadius, rabbitLayer);

        if (rabbits.Length > 0)
        {
            //print($"in range, length {rabbits.Length}");
            if (!trainingMode) inventory.RemoveCarrot();
            audioSource.clip = doThrow ? twinkleClip : takeClip;
            audioSource.Play();
            EpisodeEvents.RaiseRabbitFed();
            RabbitReaction reaction = rabbits[0].GetComponent<RabbitReaction>();
            if (reaction != null)
            {
                reaction.ReactToFeeding();
            }
            return true;
        }
        else if (doThrow)
        {
            //Debug.Log("No rabbits nearby... tossing a carrot.");
            if (!trainingMode) inventory.RemoveCarrot();
            ThrowCarrot();
            audioSource.clip = tossClip;
            audioSource.Play();
            return false;
        }
        return false;
    }
    [SerializeField] float domeDisplayRad = .5f;
    void ThrowCarrot()
    {
        Vector3 spawnPosition = transform.position + transform.forward * 1.5f + Vector3.up * 1.5f;
        GameObject carrotInstance = Instantiate(carrotPrefab, spawnPosition, Quaternion.identity);
        carrotInstance.tag = "ThrownCarrot";
        carrotInstance.layer = LayerMask.NameToLayer("ThrownCarrot");
        carrotInstance.transform.localScale *= thrownCarrotScale;
        SphereCollider col = carrotInstance.GetComponent<SphereCollider>();
        if (col != null)
        {
            col.isTrigger = false;
        }
        else
        {
            col = carrotInstance.AddComponent<SphereCollider>();
            col.radius = 0.085f;
            col.isTrigger = false;
            col.center = Vector3.zero;
        }
        Rigidbody rb = carrotInstance.GetComponent<Rigidbody>();
        if (rb == null)
        {
            rb = carrotInstance.AddComponent<Rigidbody>();
        }
        rb.useGravity = true;
        rb.linearDamping = 0.5f;
        rb.angularDamping = 2.0f;
        rb.isKinematic = false;
        EpisodeEvents.RaiseCarrotThrown(spawnPosition);

        var dome = GameObject.CreatePrimitive(PrimitiveType.Sphere);
        var rend = dome.GetComponent<MeshRenderer>();
        rend.enabled = false;
        Destroy(dome.GetComponent<Collider>());
        dome.transform.SetParent(carrotInstance.transform, false);
        dome.transform.localScale = Vector3.one * domeDisplayRad * 2f;
        dome.transform.localPosition = Vector3.zero;

        var domeMesh = dome.GetComponent<MeshFilter>().mesh;
        Vector3[] verts = domeMesh.vertices;
        for (int i = 0; i < verts.Length; i++)
            if (verts[i].y < 0) verts[i].y = 0;
        domeMesh.vertices = verts;
        domeMesh.RecalculateNormals();

        var mat = new Material(Shader.Find("Universal Render Pipeline/Unlit"));

        mat.SetFloat("_Surface", 1);                               // 1 = Transparent
        mat.EnableKeyword("_SURFACE_TYPE_TRANSPARENT");            // keyword gate
        mat.SetInt("_SrcBlend", (int)UnityEngine.Rendering.BlendMode.SrcAlpha);
        mat.SetInt("_DstBlend", (int)UnityEngine.Rendering.BlendMode.OneMinusSrcAlpha);
        mat.SetInt("_ZWrite", 0);                            // no depth write
        mat.renderQueue = (int)UnityEngine.Rendering.RenderQueue.Transparent;
        mat.SetColor("_BaseColor", new Color(0f, 0.5f, 1f, 0.2f));
        rend.sharedMaterial = mat;
        rend.enabled = true; 

        dome.AddComponent<FollowPosition>().Init(carrotInstance.transform);

        // auto-destroy dome when carrot expires (optional)
        Destroy(dome, 5f);
        rb.AddForce(transform.forward * tossForce, ForceMode.Impulse);
    }


    IEnumerator ResetFeedingSignalAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);

        foreach (RabbitAgent rabbit in allRabbits)
        {
            rabbit.playerIsFeeding = false;
        }
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, feedRadius);
    }
}
public class FollowPosition : MonoBehaviour
{
    Transform target;
    public void Init(Transform t) => target = t;

    void LateUpdate()
    {
        if (target)
        {
            transform.position = target.position + new Vector3(0, -.5f, 0);
            transform.rotation = Quaternion.identity;
        }
        else Destroy(gameObject);        // carrot got destroyed
    }
}
