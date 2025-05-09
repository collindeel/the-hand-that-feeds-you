using TMPro;
using UnityEngine;

public class Dialog : MonoBehaviour
{
    public TextMeshProUGUI dialogTitle;
    public TextMeshProUGUI dialogText;

    public void SetContent(string title, string text)
    {
        dialogTitle.text = title;
        dialogText.text = text;
    }

    public void Destroy()
    {
        Destroy(gameObject);
    }
}
