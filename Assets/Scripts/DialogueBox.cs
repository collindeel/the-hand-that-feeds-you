using TMPro;
using UnityEngine;

public class DialogueBox : MonoBehaviour
{
    public GameObject textbox;
    public GameObject namebox;
    TextMeshProUGUI _textboxText;
    TextMeshProUGUI _nameboxText;
    TextSettings _textboxTextSettings;
    TextSettings _nameboxTextSettings;

    void Awake()
    {
        _nameboxText = namebox.GetComponentInChildren<TextMeshProUGUI>();
        _nameboxTextSettings = _nameboxText.GetComponent<TextSettings>();
        _textboxText = textbox.GetComponentInChildren<TextMeshProUGUI>();
        _textboxTextSettings = _textboxText.GetComponent<TextSettings>();
    }

    public void DisplayText(string text, string speaker = null)
    {
        textbox.SetActive(true);
        _textboxText.text = text;
        _textboxTextSettings.ReinitializeDefaultText();

        if (speaker != null)
        {
            namebox.SetActive(true);
            _nameboxText.text = speaker;
            _nameboxTextSettings.ReinitializeDefaultText();
        }

        UpdateTextSettings();
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

    void UpdateTextSettings()
    {
        if (PlayerPrefs.GetInt("Dyslexia-friendly Font", 0) == 1)
        {
            ((RectTransform)textbox.transform).sizeDelta = new Vector2(1225f, 255f);
            ((RectTransform)namebox.transform).position = new Vector3(450f, 263f, 0f);
            ((RectTransform)_textboxText.transform).offsetMin = new Vector2(42f, 30f);
            ((RectTransform)_textboxText.transform).offsetMax = new Vector2(-42f, -30f);
        }
        else
        {

            ((RectTransform)textbox.transform).sizeDelta = new Vector2(1225f, 220f);
            ((RectTransform)namebox.transform).position = new Vector3(450f, 228f, 0f);
            ((RectTransform)_textboxText.transform).offsetMin = new Vector2(56f, 40f);
            ((RectTransform)_textboxText.transform).offsetMax = new Vector2(-56f, -40f);
        }
    }
}
