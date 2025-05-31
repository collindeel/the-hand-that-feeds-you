using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class MainMenuManager : MonoBehaviour
{
    public Canvas mainCanvas;
    public Canvas optionsCanvas;
    public Canvas creditsCanvas;
    public GameObject globalVariablesPrefab;
    public Image titleImage;
    public Sprite alternateTitleSprite;
    public Button startButton;
    public Button creditsBackButton;

    GlobalVariables _globalVariables;
    InputAction _navigateAction;
    InputAction _pointAction;

    void Awake()
    {
        var globalVariablesObject = GameObject.FindWithTag("GlobalVariables");
        if (globalVariablesObject == null)
        {
            globalVariablesObject = Instantiate(globalVariablesPrefab);
            globalVariablesObject.tag = "GlobalVariables";
            DontDestroyOnLoad(globalVariablesObject);
        }
        _globalVariables = globalVariablesObject.GetComponent<GlobalVariables>();
        if (_globalVariables.gameCompleted)
            titleImage.sprite = alternateTitleSprite;

        EventSystem.current.SetSelectedGameObject(startButton.gameObject);
    }

    void Start()
    {
        var menuManager = GetComponent<MenuManager>();
        var dialogManager = GetComponent<DialogManager>();

        if (!_globalVariables.showedContentWarning)
        {
            dialogManager.CreateDialog("Notice", "Trigger warning info available at the bottom right of the main menu.", () => { _globalVariables.showedContentWarning = true; });

            startButton.onClick.AddListener(() =>
            {
                dialogManager.CreateDialog("Player Name", "Please enter your name.", "Misaki", (name) =>
                {
                    _globalVariables.playerName = name == "" ? "Misaki" : name;
                    menuManager.StartGame();
                });
            });
        }
        else startButton.onClick.AddListener(menuManager.StartGame);

        _navigateAction = InputSystem.actions.FindAction("Navigate");
        _pointAction = InputSystem.actions.FindAction("Point");
    }

    void Update()
    {
        if (!optionsCanvas.gameObject.activeSelf && _navigateAction.WasPressedThisFrame())
        {
            if (GameObject.FindWithTag("DialogBox") == null)
            {
                Cursor.lockState = CursorLockMode.Locked;
                if (EventSystem.current.currentSelectedGameObject == null)
                    if (mainCanvas.gameObject.activeSelf) EventSystem.current.SetSelectedGameObject(startButton.gameObject);
                    else if (creditsCanvas.gameObject.activeSelf) EventSystem.current.SetSelectedGameObject(creditsBackButton.gameObject);
            }
        }
        else if (!optionsCanvas.gameObject.activeSelf && _pointAction.WasPerformedThisFrame())
        {
            if (GameObject.FindWithTag("DialogBox") == null)
            {
                Cursor.lockState = CursorLockMode.None;
                EventSystem.current.SetSelectedGameObject(null);
            }
        }
    }
}
