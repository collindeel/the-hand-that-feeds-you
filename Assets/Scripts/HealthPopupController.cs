using UnityEngine;
using System.Collections;

public class HealthPopupController : MonoBehaviour
{
    private Coroutine currentFadeRoutine;
    public CanvasGroup canvasGroup;
    public float displayTime = 2f;
    public float fadeDuration = 1f;

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
        canvasGroup.alpha = 1f;

        yield return new WaitForSeconds(displayTime);

        float elapsed = 0f;
        while (elapsed < fadeDuration)
        {
            canvasGroup.alpha = Mathf.Lerp(1f, 0f, elapsed / fadeDuration);
            elapsed += Time.deltaTime;
            yield return null;
        }

        canvasGroup.alpha = 0f;
    }
}
