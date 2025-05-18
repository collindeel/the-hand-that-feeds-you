using UnityEngine;
using System.Collections;

public class HealthPopupController : MonoBehaviour
{
    private Coroutine currentFadeRoutine;
    public CanvasGroup canvasGroup;
    public float displayTime = 2f;
    public float fadeOutDuration = 1f;
    public float fadeInDuration = .2f;

    void Start()
    {
        canvasGroup.alpha = 0f;
    }

    public void ShowPopup()
    {
        if (currentFadeRoutine != null)
            StopCoroutine(currentFadeRoutine);

        currentFadeRoutine = StartCoroutine(ShowAndFade());
    }

    private IEnumerator ShowAndFade()
    {
        float elapsed = 0f;
        while (elapsed < fadeInDuration)
        {
            canvasGroup.alpha = Mathf.Lerp(0f, 1f, elapsed / fadeInDuration);
            elapsed += Time.deltaTime;
            yield return null;
        }

        yield return new WaitForSeconds(displayTime);

        elapsed = 0f;
        while (elapsed < fadeOutDuration)
        {
            canvasGroup.alpha = Mathf.Lerp(1f, 0f, elapsed / fadeOutDuration);
            elapsed += Time.deltaTime;
            yield return null;
        }

        canvasGroup.alpha = 0f;
    }
}
