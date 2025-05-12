using UnityEngine;

public class SpeechBubbleController : MonoBehaviour
{
    public Canvas speechBubblePrefab;

    void Start()
    {
        CreateSpeechBubble("Test test test");
    }

    void Update()
    {

    }

    public void CreateSpeechBubble(string text)
    {
        var speechBubbleCanvas = Instantiate(speechBubblePrefab, transform);
        var speechBubble = speechBubbleCanvas.GetComponent<SpeechBubble>();
        speechBubble.SetText(text);
    }
}
