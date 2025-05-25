using UnityEngine;
using UnityEngine.InputSystem;

public class CarrotCollector : MonoBehaviour
{
    public float pickupRadius = 2f;
    public LayerMask carrotLayerMask;
    public CarrotInventory inventory;
    public AudioClip collectClip;
    public CountdownTimer countdownTimer;
    public EpisodeController episodeController;
    public ObjectiveAndTimerController otc;
    AudioSource audioSource;

    InputAction _interactAction;

    void Start()
    {
        _interactAction = InputSystem.actions.FindAction("Interact");
        audioSource = GetComponent<AudioSource>();
    }

    void Update()
    {
        if (_interactAction.WasPressedThisFrame() &&
            (countdownTimer.IsTimerRunning() ||
              episodeController.GetEpisode() == 4 ||
              otc.IsTutorialCarrot()
            )
        )
        {
            GameObject[] carrots = GameObject.FindGameObjectsWithTag("Carrot");

            foreach (GameObject carrot in carrots)
            {
                float distance = Vector3.Distance(transform.position, carrot.transform.position);
                if (distance <= pickupRadius)
                {
                    inventory.AddCarrot();
                    EpisodeEvents.RaiseCarrotCollected();
                    audioSource.clip = collectClip;
                    audioSource.Play();
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
