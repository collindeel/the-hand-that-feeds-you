using UnityEngine;
using UnityEngine.InputSystem;

public class ObjectiveManager : MonoBehaviour
{
    public float triggerRadius = 2f;
    public LayerMask objectiveLayerMask;

    InputAction _interactAction;

    void Start()
    {
        _interactAction = InputSystem.actions.FindAction("Interact");
    }
    
    void Update()
    {
        if (_interactAction.WasPressedThisFrame())
        {
            Collider[] hits = Physics.OverlapSphere(transform.position, triggerRadius, objectiveLayerMask);
            
            if (hits.Length > 0)
            {
                var objective = hits[0].GetComponent<Objective>();
                objective.Trigger();
            }
        }
    }
}
