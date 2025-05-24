using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class MainMenuManager : MonoBehaviour
{
    public GameObject globalVariablesPrefab;
    public Image titleImage;
    public Sprite alternateTitleSprite;
    public Button startButton;

    GlobalVariables _globalVariables;

    void Start()
    {
        var globalVariablesObject = GameObject.FindWithTag("GlobalVariables");
        if (globalVariablesObject == null)
        {
            globalVariablesObject = Instantiate(globalVariablesPrefab);
            globalVariablesObject.tag = "GlobalVariables";
            DontDestroyOnLoad(globalVariablesObject);
        }
        _globalVariables = globalVariablesObject.GetComponent<GlobalVariables>();

        if (_globalVariables.gameCompleted) titleImage.sprite = alternateTitleSprite;

        EventSystem.current.SetSelectedGameObject(startButton.gameObject);

        if (!_globalVariables.showedContentWarning)
        {
            var dialogManager = GetComponent<DialogManager>();
            dialogManager.CreateDialog("Notice", "Trigger warning info available at the bottom right of the main menu.");

            _globalVariables.showedContentWarning = true;
        }
    }
}
