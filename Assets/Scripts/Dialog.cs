using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class Dialog : MonoBehaviour
{
    public TextMeshProUGUI dialogTitle;
    public TextMeshProUGUI dialogText;
    public Button okButton;

    GameObject previousSelectedGameObject;
    InputAction _navigateAction;
    InputAction _pointAction;

    void Start()
    {
        tag = "DialogBox";

        _navigateAction = InputSystem.actions.FindAction("Navigate");
        _pointAction = InputSystem.actions.FindAction("Point");

        previousSelectedGameObject = EventSystem.current.currentSelectedGameObject;
        EventSystem.current.SetSelectedGameObject(okButton.gameObject);
    }

    void Update()
    {
        if (_navigateAction.WasPressedThisFrame())
        {
            Cursor.visible = false;
            EventSystem.current.SetSelectedGameObject(okButton.gameObject);
        }
        else if (_pointAction.WasPerformedThisFrame())
        {
            Cursor.visible = true;
            EventSystem.current.SetSelectedGameObject(null);
        }
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
