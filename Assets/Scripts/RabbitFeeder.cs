using System.Collections;
using UnityEngine;

public class RabbitFeeder : MonoBehaviour
{
    public float feedRadius = 3f;
    public LayerMask rabbitLayer;
    public CarrotInventory inventory;
    public RabbitAgent[] allRabbits;
    public GameObject carrotPrefab;
    public float thrownCarrotScale = 0.25f;
    public float feedRange = 2f;
    public float tossForce = 5f;

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.F))
        {
            TryFeedRabbit();
        }
    }

    public bool trainingMode = true;  // Set to false in real gameplay

    public void TryFeedRabbit()
    {
        if (!trainingMode && inventory.CarrotCount <= 0)
        {
            Debug.Log("No carrots to feed.");
            return;
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
            print($"in range, length {rabbits.Length}");
            if (!trainingMode) inventory.RemoveCarrot();
            RabbitReaction reaction = rabbits[0].GetComponent<RabbitReaction>();
            if (reaction != null)
            {
                reaction.ReactToFeeding();
            }
            // Later: Trigger rabbit reaction here, e.g. rabbits[0].GetComponent<RabbitAI>().ReactToFeeding();
        }
        else
        {
            Debug.Log("No rabbits nearby... tossing a carrot.");
            if (!trainingMode) inventory.RemoveCarrot();
            ThrowCarrot();
        }
    }
    void ThrowCarrot()
    {
        Vector3 spawnPosition = transform.position + transform.forward * 1.5f + Vector3.up * 1.5f;
        GameObject carrotInstance = Instantiate(carrotPrefab, spawnPosition, Quaternion.Euler(0f, 0f, 0f));
        carrotInstance.tag = "ThrownCarrot";
        carrotInstance.layer = LayerMask.NameToLayer("ThrownCarrot");
        carrotInstance.transform.localScale *= thrownCarrotScale;
        Collider col = carrotInstance.GetComponent<Collider>();
        if (col != null)
        {
            col.isTrigger = false;
        }
        else
        {
            Debug.LogWarning("No collider found on thrown carrot!");
        }
        Rigidbody rb = carrotInstance.GetComponent<Rigidbody>();
        if (rb == null)
        {
            rb = carrotInstance.AddComponent<Rigidbody>();
        }
        rb.useGravity = true;
        rb.linearDamping = 0.5f;
        rb.angularDamping = 2.0f;
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
