using UnityEngine;

public class AudioControllerScript : MonoBehaviour
{
    public AudioClip forestClip;
    public AudioClip happyClip;
    public AudioClip activeClip;
    public AudioClip darkClip;
    public AudioClip actionClip;
    public AudioClip endDiedClip;
    public AudioClip endWonClip;
    AudioSource music;
    void Awake() => music = GetComponent<AudioSource>();
    void OnEnable() =>
        EpisodeEvents.OnEpisodeChanged += e =>
        {
            music.clip = e.episode switch
            {
                0 => forestClip,
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
    public void Halt() => music.Stop();
    public void PlayEndDied()
    {
        music.clip = endDiedClip;
        music.Play();
    }
    public void PlayEndWon()
    {
        music.clip = endWonClip;
        music.Play();
    }
}
