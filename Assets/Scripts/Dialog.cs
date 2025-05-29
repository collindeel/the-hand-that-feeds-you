using System;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class Dialog : MonoBehaviour
{
    public TextMeshProUGUI dialogTitle;
    public TextMeshProUGUI dialogText;
    public TMP_InputField dialogInputField;
    public Button dialogButton;

    GameObject previousSelectedGameObject;
    InputAction _navigateAction;
    InputAction _pointAction;

    void Start()
    {
        tag = "DialogBox";

        _navigateAction = InputSystem.actions.FindAction("Navigate");
        _pointAction = InputSystem.actions.FindAction("Point");

        previousSelectedGameObject = EventSystem.current.currentSelectedGameObject;
    }

    void Update()
    {
        if (EventSystem.current.currentSelectedGameObject == dialogInputField.gameObject)
        {
            if (Keyboard.current.tabKey.wasPressedThisFrame)
                EventSystem.current.SetSelectedGameObject(dialogButton.gameObject);
            else if (Keyboard.current.enterKey.wasPressedThisFrame)
                dialogButton.OnPointerClick(new PointerEventData(EventSystem.current));
        }
        else if (dialogInputField.isActiveAndEnabled
            && EventSystem.current.currentSelectedGameObject == dialogButton.gameObject
            && Keyboard.current.shiftKey.isPressed
            && Keyboard.current.tabKey.wasPressedThisFrame)
        {
            EventSystem.current.SetSelectedGameObject(dialogInputField.gameObject);
        }
        else if (_navigateAction.WasPressedThisFrame())
        {
            Cursor.lockState = CursorLockMode.Locked;
            if (EventSystem.current.currentSelectedGameObject == null)
                if (dialogInputField.isActiveAndEnabled)
                    EventSystem.current.SetSelectedGameObject(dialogInputField.gameObject);
                else
                    EventSystem.current.SetSelectedGameObject(dialogButton.gameObject);
        }
        else if (_pointAction.WasPerformedThisFrame())
        {
            Cursor.lockState = CursorLockMode.None;
            if (!dialogInputField.isFocused)
                EventSystem.current.SetSelectedGameObject(null);
        }
    }

    public void SetContent(string title, string text, Action callback = null)
    {
        dialogTitle.text = title;
        dialogText.text = text;

        dialogTitle.GetComponent<TextSettings>().ReinitializeDefaultText();
        dialogText.GetComponent<TextSettings>().ReinitializeDefaultText();

        if (callback != null) dialogButton.onClick.AddListener(() => callback());
        EventSystem.current.SetSelectedGameObject(dialogButton.gameObject);
    }

    public void SetContent(string title, string text, string inputPlaceholder, Action<string> callback)
    {
        dialogTitle.text = title;
        dialogText.text = text;

        dialogTitle.GetComponent<TextSettings>().ReinitializeDefaultText();
        dialogText.GetComponent<TextSettings>().ReinitializeDefaultText();

        dialogInputField.gameObject.SetActive(true);
        dialogInputField.placeholder.GetComponent<TextMeshProUGUI>().text = inputPlaceholder;
        dialogButton.onClick.AddListener(() => callback(dialogInputField.text));

        dialogInputField.ActivateInputField();
    }

    public void Destroy()
    {
        EventSystem.current.SetSelectedGameObject(previousSelectedGameObject);

        Destroy(gameObject);
    }
}
