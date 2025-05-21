using TMPro;
using UnityEngine;
using System.Collections;

public class ObjectiveAndTimerController : MonoBehaviour
{
    public TMP_Text objective;
    public float displayTime = 2f;
    public float fadeDuration = 1f;

    void OnEnable() => EpisodeEvents.OnEpisodeChangeComplete += HandleEpisodeChangeComplete;
    void OnDisable() => EpisodeEvents.OnEpisodeChangeComplete -= HandleEpisodeChangeComplete;

    void HandleEpisodeChangeComplete(EpisodeChangedArgs args)
    {
        if (args.episode == 1)
        {
            ShowPopup();
        }

    }
    private Coroutine currentFadeRoutine;

    public void ShowPopup()
    {
        if (currentFadeRoutine != null)
            StopCoroutine(currentFadeRoutine);

        currentFadeRoutine = StartCoroutine(ShowAndFade());
    }
    private IEnumerator ShowAndFade()
    {
        objective.alpha = 1f;

        yield return new WaitForSeconds(displayTime);

        float elapsed = 0f;
        while (elapsed < fadeDuration)
        {
            objective.alpha = Mathf.Lerp(1f, 0f, elapsed / fadeDuration);
            elapsed += Time.deltaTime;
            yield return null;
        }
        objective.alpha = 0f;
    }
}
