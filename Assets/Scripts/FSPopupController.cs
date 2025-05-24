using UnityEngine;
using TMPro;
using System.Collections;

public class FSPopupController : MonoBehaviour
{
    public TMP_Text scoreText;
    public float displayTime = 2f;
    public float fadeDuration = 1f;

    private Coroutine currentFadeRoutine;

    public void ShowPopup(int score)
    {
        if (currentFadeRoutine != null)
            StopCoroutine(currentFadeRoutine);

        currentFadeRoutine = StartCoroutine(FadeIn(score));
    }

    private IEnumerator FadeIn(int score)
    {
        scoreText.text = score.ToString();
        scoreText.alpha = 0f;

        yield return new WaitForSeconds(displayTime);

        float elapsed = 0f;
        while (elapsed < fadeDuration)
        {
            scoreText.alpha = Mathf.Lerp(0f, 1f, elapsed / fadeDuration);
            elapsed += Time.deltaTime;
            yield return null;
        }

        scoreText.alpha = 1f;
    }
}
