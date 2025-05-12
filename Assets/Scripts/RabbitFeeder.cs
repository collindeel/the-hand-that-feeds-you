using UnityEngine;

public class RabbitFeeder : MonoBehaviour
{
    public float feedRadius = 3f;
    public LayerMask rabbitLayer;
    public CarrotInventory inventory;

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.E))
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
            Debug.Log("No rabbits nearby.");
        }
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, feedRadius);
    }
}
