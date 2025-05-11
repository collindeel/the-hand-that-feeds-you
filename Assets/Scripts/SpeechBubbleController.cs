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

    public void CreateSpeechBubble(string text) {
        Instantiate(speechBubblePrefab, transform);
        var speechBubble = speechBubblePrefab.GetComponentInChildren<SpeechBubble>();
        speechBubble.SetText(text);
    }
}
