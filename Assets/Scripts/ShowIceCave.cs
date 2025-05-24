using UnityEngine;

public class ShowIceCave : MonoBehaviour
{
    [SerializeField] GameObject caveIceParent;

    void OnEnable()  => EpisodeEvents.OnEpisodeChanged += HandleEpisode;
    void OnDisable() => EpisodeEvents.OnEpisodeChanged -= HandleEpisode;

    void HandleEpisode(EpisodeChangedArgs args)
    {
        if (args.episode != 4) return;
        caveIceParent.SetActive(true);
        EpisodeEvents.OnEpisodeChanged -= HandleEpisode;
    }
}
