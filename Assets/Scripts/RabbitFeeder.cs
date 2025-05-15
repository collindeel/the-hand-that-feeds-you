using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

public class RabbitFeeder : MonoBehaviour
{
    public float feedRadius = 3f;
    public LayerMask rabbitLayer;
    public CarrotInventory inventory;
    public RabbitAgent[] allRabbits;
    public GameObject carrotPrefab;
    public float feedRange = 2f;
    public float tossForce = 5f;
    
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.F) || (Gamepad.current?.buttonWest.wasPressedThisFrame ?? false))
        {
            TryFeedRabbit();
        }
    }

    void TryFeedRabbit()
    {
        if (inventory.CarrotCount <= 0)
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
            inventory.RemoveCarrot();
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
            inventory.RemoveCarrot();
            Vector3 spawnPosition = transform.position + transform.forward * 1.5f + Vector3.up * 0.5f;
            GameObject carrot = Instantiate(carrotPrefab, spawnPosition, Quaternion.identity);
            Rigidbody rb = carrot.GetComponent<Rigidbody>();
            if (rb != null)
            {
                rb.AddForce(transform.forward * tossForce, ForceMode.VelocityChange);
            }
        }
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
