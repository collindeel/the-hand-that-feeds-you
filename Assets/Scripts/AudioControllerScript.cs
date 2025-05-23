using UnityEngine;

public class AudioControllerScript : MonoBehaviour
{
    public AudioClip happyClip;
    public AudioClip actionClip;
    public AudioClip darkClip;
    AudioSource music;
    void Awake() => music = GetComponent<AudioSource>();
    void OnEnable() =>
        EpisodeEvents.OnEpisodeChanged += e =>
        {
            music.clip = e.episode switch
            {
                0 => null,
                1 => happyClip,
                2 => actionClip,
                3 => darkClip,
                _ => null
            };
            if (music.clip == null)
                music.Stop();
            else
                music.Play();
        };
}
