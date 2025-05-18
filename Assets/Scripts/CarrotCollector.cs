using UnityEngine;
using UnityEngine.InputSystem;

public class CarrotCollector : MonoBehaviour
{
    public float pickupRadius = 2f;
    public LayerMask carrotLayerMask;
    public CarrotInventory inventory;

    InputAction _interactAction;

    void Start()
    {
        _interactAction = InputSystem.actions.FindAction("Interact");
    }

    void Update()
    {
        if (_interactAction.WasPressedThisFrame())
        {
            GameObject[] carrots = GameObject.FindGameObjectsWithTag("Carrot");

            foreach (GameObject carrot in carrots)
            {
                float distance = Vector3.Distance(transform.position, carrot.transform.position);
                if (distance <= pickupRadius)
                {
                    inventory.AddCarrot();
                    Destroy(carrot);
                }
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
