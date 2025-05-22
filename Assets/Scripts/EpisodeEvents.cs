using System;

public static class EpisodeEvents
{
    public static event Action<EpisodeChangedArgs> OnEpisodeChanged;
    public static event Action<EpisodeChangedArgs> OnEpisodeChangeComplete;
    public static event Action OnRabbitFed;

    public static event Action OnCarrotCollected;
    public static event Action OnCarrotThrown;
    public static void RaiseCarrotThrown()
    {
        OnCarrotThrown?.Invoke();
    }
    public static void RaiseCarrotCollected()
    {
        OnCarrotCollected?.Invoke();
    }
    public static void RaiseRabbitFed()
    {
        OnRabbitFed?.Invoke();
    }
    public static void RaiseEpisodeChanged(int episode,
                                           RabbitBehaviorLevel level)
    {
        OnEpisodeChanged?.Invoke(new EpisodeChangedArgs(episode, level));
    }
    public static void RaiseEpisodeChangeComplete(int episode, RabbitBehaviorLevel level)
    {
        OnEpisodeChangeComplete?.Invoke(new EpisodeChangedArgs(episode, level));
    }
}

public readonly struct EpisodeChangedArgs
{
    public readonly int episode;
    public readonly RabbitBehaviorLevel level;

    public EpisodeChangedArgs(int ep, RabbitBehaviorLevel lvl)
    {
        episode = ep;
        level = lvl;
    }
}

/*void OnEnable()  => EpisodeEvents.OnEpisodeChanged += HandleEpisodeChange;
void OnDisable() => EpisodeEvents.OnEpisodeChanged -= HandleEpisodeChange;

void HandleEpisodeChange(EpisodeChangedArgs args)
{
    if (args.level == RabbitBehaviorLevel.Aggressive) 
         blah blah blah
    if (args.episode == 3)
        blah blah blah
}*/
