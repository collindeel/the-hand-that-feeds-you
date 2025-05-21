using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class Dialog : MonoBehaviour
{
    public TextMeshProUGUI dialogTitle;
    public TextMeshProUGUI dialogText;
    public Button okButton;

    GameObject previousSelectedGameObject;

    void Start()
    {
        previousSelectedGameObject = EventSystem.current.currentSelectedGameObject;
        EventSystem.current.SetSelectedGameObject(okButton.gameObject);
    }

    void Update()
    {
        if (EventSystem.current.currentSelectedGameObject != okButton.gameObject)
            EventSystem.current.SetSelectedGameObject(okButton.gameObject);
    }

    public void SetContent(string title, string text)
    {
        dialogTitle.text = title;
        dialogText.text = text;

        dialogTitle.GetComponent<TextSettings>().ReinitializeDefaultText();
        dialogText.GetComponent<TextSettings>().ReinitializeDefaultText();
    }

    public void Destroy()
    {
        EventSystem.current.SetSelectedGameObject(null);
        EventSystem.current.SetSelectedGameObject(previousSelectedGameObject);

        Destroy(gameObject);
    }
}
