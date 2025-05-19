using UnityEngine;
using TMPro;
using System.Collections;

public class ScorePopupController : MonoBehaviour
{
    public TMP_Text scoreText;
    public float displayTime = 2f;
    public float fadeDuration = 1f;

    private Coroutine currentFadeRoutine;

    public void ShowPopup(int score)
    {
        if (currentFadeRoutine != null)
            StopCoroutine(currentFadeRoutine);

        currentFadeRoutine = StartCoroutine(ShowAndFade(score));
    }

    private IEnumerator ShowAndFade(int score)
    {
        scoreText.text = "Score: " + score;
        scoreText.alpha = 1f;

        yield return new WaitForSeconds(displayTime);

        float elapsed = 0f;
        while (elapsed < fadeDuration)
        {
            scoreText.alpha = Mathf.Lerp(1f, 0f, elapsed / fadeDuration);
            elapsed += Time.deltaTime;
            yield return null;
        }

        scoreText.alpha = 0f;
    }
}
