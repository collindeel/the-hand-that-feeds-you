using UnityEngine;
using TMPro;
using System.Collections;

public class CarrotPopupController : MonoBehaviour
{
    public CanvasGroup canvasGroup;
    public TextMeshProUGUI countText;
    TextSettings _countTextSettings;
    public float displayTime = 2f;
    public float fadeDuration = 1f;

    private Coroutine currentFadeRoutine;

    void Start()
    {
        _countTextSettings = countText.GetComponent<TextSettings>();
    }

    public void ShowPopup(int carrotCount)
    {
        countText.text = "x" + carrotCount;
        _countTextSettings.ReinitializeDefaultText();
        if (currentFadeRoutine != null)
            StopCoroutine(currentFadeRoutine);

        currentFadeRoutine = StartCoroutine(ShowAndFade());
    }

    private IEnumerator ShowAndFade()
    {
        canvasGroup.BroadcastMessage("UpdateTextSettings");
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
