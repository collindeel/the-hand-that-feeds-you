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

    public static AudioControllerScript instance { get; private set; }
    void Awake()
    {
        music = GetComponent<AudioSource>(); 
        if (instance != null && instance != this)
        {
            Destroy(gameObject);
            return;
        }
        instance = this;
        DontDestroyOnLoad(gameObject);
    }

    void OnEnable() => EpisodeEvents.OnEpisodeChanged += HandleEp;
    void OnDisable() => EpisodeEvents.OnEpisodeChanged -= HandleEp;

    void HandleEp(EpisodeChangedArgs e)
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

        if (music.clip == null) music.Stop();
        else music.Play();
    }
    public void Halt()
    {
        if (music)
            music.Stop();
    }
    public void PlayEndDied() => PlayClip(endDiedClip);
    public void PlayEndWon() => PlayClip(endWonClip);

    void PlayClip(AudioClip c)
    {
        music.clip = c;
        music.Play();
    }
}
