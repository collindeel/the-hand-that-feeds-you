using TMPro;
using UnityEngine;

public class DialogueBox : MonoBehaviour
{
    public GameObject textbox;
    public GameObject namebox;
    TextMeshProUGUI _textboxText;
    TextMeshProUGUI _nameboxText;

    void Awake()
    {
        _nameboxText = namebox.GetComponentInChildren<TextMeshProUGUI>();
        _textboxText = textbox.GetComponentInChildren<TextMeshProUGUI>();
    }

    public void DisplayText(string text, string speaker = null)
    {
        textbox.SetActive(true);
        _textboxText.text = text;

        namebox.SetActive(speaker != null);
        _nameboxText.text = speaker ?? "";
    }

    public void DisplayText((string text, string speaker) line)
    {
        DisplayText(line.text, line.speaker);
    }

    public void Hide()
    {
        textbox.SetActive(false);
        namebox.SetActive(false);
    }
}
