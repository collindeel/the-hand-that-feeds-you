using UnityEngine;
using TMPro;
using System.Collections;

public class FloatingScoreDelta : MonoBehaviour
{
    [SerializeField] float floatSpeed = 50f;
    [SerializeField] float duration = 1f;
    [SerializeField] Vector2 floatDirection = Vector2.up;
    [SerializeField] private TextMeshProUGUI deltaText;
    private Color originalColor;

    public void Play(int delta)
    {
        deltaText.text = (delta > 0 ? "+" : "") + delta.ToString();
        originalColor = delta > 0 ? Color.green : Color.red;
        deltaText.color = originalColor;
        StopAllCoroutines(); // prevent overlap if retriggered quickly
        StartCoroutine(Animate());
    }

    private IEnumerator Animate()
    {
        float elapsed = 0f;
        Vector2 startPos = deltaText.rectTransform.anchoredPosition;

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float t = elapsed / duration;

            // Move upward
            deltaText.rectTransform.anchoredPosition = startPos + floatDirection * floatSpeed * t;

            // Fade out
            Color c = originalColor;
            c.a = Mathf.Lerp(1f, 0f, t);
            deltaText.color = c;

            yield return null;
        }

        deltaText.text = "";
    }
}
