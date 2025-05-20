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
            music.clip = e.level switch
            {
                RabbitBehaviorLevel.Heuristic => null,
                RabbitBehaviorLevel.Timid => happyClip,
                RabbitBehaviorLevel.Medium => actionClip,
                RabbitBehaviorLevel.Aggressive => darkClip,
                _ => null
            };
            if (music.clip == null)
                music.Stop();
            else
                music.Play();
        };
}
