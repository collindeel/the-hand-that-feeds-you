using UnityEngine;

public class AudioControllerScript : MonoBehaviour
{
    public AudioClip happyClip;
    public AudioClip activeClip;
    public AudioClip darkClip;
    public AudioClip actionClip;
    AudioSource music;
    void Awake() => music = GetComponent<AudioSource>();
    void OnEnable() =>
        EpisodeEvents.OnEpisodeChanged += e =>
        {
            music.clip = e.episode switch
            {
                0 => null,
                1 => happyClip,
                2 => activeClip,
                3 => darkClip,
                4 => actionClip,
                _ => null
            };
            if (music.clip == null)
                music.Stop();
            else
                music.Play();
        };
}
