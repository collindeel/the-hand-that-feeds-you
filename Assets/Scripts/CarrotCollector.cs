using UnityEngine;

public class CarrotCollector : MonoBehaviour
{
    public float pickupRadius = 2f;
    public LayerMask carrotLayerMask;
    public CarrotInventory inventory;

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.E))
        {
            Collider[] hits = Physics.OverlapSphere(transform.position, pickupRadius, carrotLayerMask);

            if (hits.Length > 0)
            {
                // Pick the first found carrot
                GameObject carrot = hits[0].gameObject;
                inventory.AddCarrot();
                Destroy(carrot);
            }
        }
    }

    void OnDrawGizmosSelected()
    {
        // Visualize pickup radius
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(transform.position, pickupRadius);
    }
}
