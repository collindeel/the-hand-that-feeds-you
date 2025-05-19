using UnityEngine;

public class FootSteps : MonoBehaviour
{
    [SerializeField]
    private AudioClip[] clips;

    private AudioSource audioSource;

    private void Awake()
    {
        audioSource = GetComponent<AudioSource>();
    }

    private void Step()
    {
        AudioClip clip = GetRandomClip();
        audioSource.PlayOneShot(clip);
    }

    private AudioClip GetRandomClip()
    {
        return clips[Random.RandomRange(0, clips.Length)]; 
    }
}