
using UnityEngine;
using UnityEngine.InputSystem;

public class LieFlat : MonoBehaviour
{
    [Header("Offset where lying")]
    [SerializeField]
    float offsetX;
    [SerializeField]
    float offsetZ;

    [SerializeField]
    float absoluteY;

    Vector3 initPos;

    public Transform player;

    void OnEnable() => EpisodeEvents.OnEpisodeChanged += HandleEpisodeChanged;

    void Start()
    {
        initPos = transform.position;
    }
    void Update()
    {
#if UNITY_EDITOR
        if (Keyboard.current != null && Keyboard.current.oKey.wasPressedThisFrame)
        {
            SetCoordinatesToReflectRelative();
        }
        if (Keyboard.current != null && Keyboard.current.pKey.wasPressedThisFrame)
        {
            PreviewPosition();
        }
#endif
    }

    void HandleEpisodeChanged(EpisodeChangedArgs args)
    {
        if (args.episode == 3)
        {
            transform.rotation = Quaternion.Euler(-90f, 0f, 0f);
            transform.position += new Vector3(offsetX, 0f, offsetZ);
        }
    }
#if UNITY_EDITOR

    [ContextMenu("Set coordinates to player")]
    void SetCoordinatesToReflectRelative()
    {
        offsetX = player.position.x - transform.position.x;
        offsetZ = player.position.z - transform.position.z;
        absoluteY = player.position.y;
    }

    [ContextMenu("Preview pos")]
    void PreviewPosition()
    {
        transform.position = initPos;
        transform.rotation = Quaternion.Euler(-90f, 0f, 0f);
        transform.position = new Vector3(transform.position.x + offsetX, absoluteY, transform.position.z + offsetZ);
    }
#endif
}
