using UnityEngine;
using TMPro;
using System.Collections;

public class FSPopupController : MonoBehaviour
{
    public TextMeshProUGUI scoreText;
    TextSettings _scoreTextSettings;
    public float beforeShown = .5f;
    public float fadeDuration = 1f;

    private Coroutine currentFadeRoutine;

    void Start()
    {
        _scoreTextSettings = scoreText.GetComponent<TextSettings>();
    }

    public void ShowPopup(int score)
    {
        if (currentFadeRoutine != null)
            StopCoroutine(currentFadeRoutine);

        currentFadeRoutine = StartCoroutine(FadeIn(score));
    }

    private IEnumerator FadeIn(int score)
    {
        scoreText.text = score.ToString();
        _scoreTextSettings.ReinitializeDefaultText();
        scoreText.fontSize = 48f;
        scoreText.alpha = 0f;

        yield return new WaitForSecondsRealtime(beforeShown);

        float elapsed = 0f;
        while (elapsed < fadeDuration)
        {
            scoreText.alpha = Mathf.Lerp(0f, 1f, elapsed / fadeDuration);
            scoreText.fontSize = Mathf.Lerp(48f, 120f, elapsed / fadeDuration);
            elapsed += Time.unscaledDeltaTime;
            yield return null;
        }

        scoreText.alpha = 1f;
    }
}
