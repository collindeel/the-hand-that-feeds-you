using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class MainMenuManager : MonoBehaviour
{
    public Button startButton;

    void Start()
    {
        EventSystem.current.SetSelectedGameObject(startButton.gameObject);

        var dialogManager = GetComponent<DialogManager>();
        if (dialogManager != null)
        {
            dialogManager.CreateDialog("Notice", "Trigger warning info available at the bottom right of the main menu.");
        }
    }
}
