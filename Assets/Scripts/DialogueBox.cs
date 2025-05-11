using TMPro;
using UnityEngine;

public class DialogueBox : MonoBehaviour
{
    public GameObject textbox;
    public GameObject namebox;
    TextMeshProUGUI _textboxText;
    TextMeshProUGUI _nameboxText;

    void Start()
    {
        _nameboxText = namebox.GetComponentInChildren<TextMeshProUGUI>();
        _textboxText = textbox.GetComponentInChildren<TextMeshProUGUI>();
    }

    public void DisplayText(string text, string name = null)
    {
        textbox.SetActive(true);
        _textboxText.text = text;

        namebox.SetActive(name != null);
        _nameboxText.text = name ?? "";
    }

    public void Hide()
    {
        textbox.SetActive(false);
        namebox.SetActive(false);
    }
}
