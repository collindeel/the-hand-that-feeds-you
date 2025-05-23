using UnityEngine;
using UnityEngine.InputSystem;
using TMPro;

public enum RabbitBehaviorLevel { Heuristic, Timid, Medium, Aggressive }
public class EpisodeController : MonoBehaviour
{
    [Header("UI")]
    [SerializeField] CanvasGroup overlay;
    [SerializeField] TMP_Text label;
    [Header("Fade timings (seconds)")]
    [SerializeField] float fadeInDuration = 4f;
    [SerializeField] float fadeOutTextDuration = 2f;
    [SerializeField] float fadeOutCanvasDuration = 3f;
    [SerializeField] float holdDuration = 3f;
    [SerializeField] float preholdDuration = 1f;
    [SerializeField] float postholdDuration = 1.5f;

    public ArrowPointer arrowPointer;

    [Header("Rabbit Switching")]
    int episode = 0;
    bool busy = false;

    const Key nextKey = Key.Period;
    const Key skipKey = Key.Space;
    Coroutine currentRoutine;

    void Start() => currentRoutine = StartCoroutine(EpisodeRoutine());
    void Update()
    {
        var kb = Keyboard.current;
        if (kb == null) return;
        if (kb[skipKey].wasPressedThisFrame && overlay.gameObject.activeSelf)
        {
            KillOverlayRoutine();
            HideOverlayInstant();
            EpisodeEvents.RaiseEpisodeChangeComplete(episode, getLevelByEpisode());
            busy = false;
            return;
        }
        if (busy) return;

        if (kb[nextKey].wasPressedThisFrame)
        {
            StartNextEpisode();
        }
    }
    public int GetEpisode()
    {
        return episode;
    }
    RabbitBehaviorLevel getLevelByEpisode()
    {
        RabbitBehaviorLevel level = RabbitBehaviorLevel.Heuristic; // ep "0" but no overlay
        if (episode == 1) level = RabbitBehaviorLevel.Timid;
        else if (episode == 2) level = RabbitBehaviorLevel.Medium;
        else if (episode == 3) level = RabbitBehaviorLevel.Heuristic; // Start ep 3 with rabbits at idle
        else if (episode == 4) level = RabbitBehaviorLevel.Aggressive; // ep "4" but no overlay
        return level;
    }
    public void StartNextEpisodeIfApplicable()
    {
        if (episode == 0 || episode == 1 || episode == 3)
            StartNextEpisode();
    }
    public void StartNextEpisode()
    {
        if(episode == 1)
            arrowPointer.gameObject.SetActive(false);
        if (!busy)
        {
            episode++;
            KillOverlayRoutine();
            currentRoutine = StartCoroutine(EpisodeRoutine());
        }
    }
    void HideOverlayInstant()
    {
        Time.timeScale = 1f;

        label.alpha = 0f;
        overlay.alpha = 0f;
        overlay.gameObject.SetActive(false);
    }
    void KillOverlayRoutine()
    {
        if (currentRoutine != null)
            StopCoroutine(currentRoutine);
        StopCoroutine(nameof(FadeInText));
        StopCoroutine(nameof(FadeOutText));
        StopCoroutine(nameof(FadeOutCanvas));
    }
    System.Collections.IEnumerator EpisodeRoutine()
    {
        busy = true;

        RabbitBehaviorLevel level = getLevelByEpisode();

        var rabbits = FindObjectsByType<RabbitModelSwitcher>(FindObjectsSortMode.None);

        foreach (var r in rabbits)
        {
            if (level == RabbitBehaviorLevel.Heuristic)
            {
                r.HeuristicOnly();
                if (episode == 3)
                    r.isIdleOnly = true;
            }
            else
                r.HeuristicOnly(false);
            r.SetLevel(level);
        }

        EpisodeEvents.RaiseEpisodeChanged(episode, level);

        // For all "episodes" where we don't want overlay
        if (episode == 0 || episode > 3)
        {
            HideOverlayInstant();
            busy = false;
            //EpisodeEvents.RaiseEpisodeChangeComplete(episode, level); // Forget why this was here but it breaks the tutorial
            yield break;
        }

        string ft;
        switch (episode)
        {
            case 1:
                ft = "Give carrots to the sweet rabbits";
                ScoreTracker.isScoreDisabled = false;
                break;
            case 2:
                ft = "Don't let the rabbits take your carrots!";
                break;
            case 3:
            default:
                ft = "...";
                break;
        }
        label.text = $"Episode {episode}\n\n{ft}";

        yield return FadeInText();
        yield return new WaitForSecondsRealtime(holdDuration);
        yield return FadeOutText();
        yield return new WaitForSecondsRealtime(postholdDuration);
        yield return FadeOutCanvas();
        EpisodeEvents.RaiseEpisodeChangeComplete(episode, level);
        busy = false;
    }

    System.Collections.IEnumerator FadeOutText()
    {
        label.alpha = 1f;
        float t = 0f;
        while (t < fadeOutTextDuration)
        {
            t += Time.unscaledDeltaTime;
            label.alpha = Mathf.InverseLerp(1f, 0f, t / fadeOutTextDuration);
            yield return null;
        }
        label.alpha = 0f;
    }
    System.Collections.IEnumerator FadeOutCanvas()
    {
        Time.timeScale = 1f;
        overlay.alpha = 1f;
        float t = 0f;
        while (t < fadeOutCanvasDuration)
        {
            t += Time.unscaledDeltaTime;
            overlay.alpha = Mathf.InverseLerp(1f, 0f, t / fadeOutCanvasDuration);
            yield return null;
        }
        overlay.alpha = 0f;
        overlay.gameObject.SetActive(false);
    }
    System.Collections.IEnumerator FadeInText()
    {
        label.alpha = 0f;
        overlay.gameObject.SetActive(true);
        overlay.alpha = 1f;
        yield return new WaitForSecondsRealtime(preholdDuration);
        Time.timeScale = 0f;

        float t = 0f;
        while (t < fadeInDuration)
        {
            t += Time.unscaledDeltaTime;
            label.alpha = Mathf.Lerp(0f, 1f, t / fadeInDuration);
            yield return null;
        }
        label.alpha = 1f;
    }
}
