using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class PauseMenuManager : MonoBehaviour
{
    public GameManager gameManager;
    public Canvas pauseMenuCanvas;
    public Canvas optionsCanvas;
    public Button resumeButton;
    public DialogueBox dialogueBox;

    InputAction _navigateAction;
    InputAction _pointAction;

    // Pausing music
    public AudioControllerScript audioController;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        _navigateAction = InputSystem.actions.FindAction("Navigate");
        _pointAction = InputSystem.actions.FindAction("Point");
    }

    // Update is called once per frame
    void Update()
    {
        if (gameManager.doesEscapeTriggerMenu && (Keyboard.current.escapeKey.wasPressedThisFrame || (Gamepad.current?.startButton.wasPressedThisFrame ?? false)))
            TogglePauseGame();

        if (gameManager.isPaused && GameObject.FindWithTag("DialogBox") == null)
            if (pauseMenuCanvas.gameObject.activeSelf && _navigateAction.WasPressedThisFrame())
            {
                Cursor.lockState = CursorLockMode.Locked;
                if (EventSystem.current.currentSelectedGameObject == null)
                    EventSystem.current.SetSelectedGameObject(resumeButton.gameObject);
            }
            else if (pauseMenuCanvas.gameObject.activeSelf && _pointAction.WasPerformedThisFrame())
            {
                Cursor.lockState = CursorLockMode.None;
                EventSystem.current.SetSelectedGameObject(null);
            }
    }

    public void TogglePauseGame()
    {
        gameManager.isPaused ^= true;

        if (gameManager.isPaused)
        {
            Time.timeScale = 0f;
            pauseMenuCanvas.gameObject.SetActive(true);
            optionsCanvas.gameObject.SetActive(false);
            Cursor.lockState = CursorLockMode.None;

            EventSystem.current.SetSelectedGameObject(resumeButton.gameObject);

            // Pause the music
            audioController.GetComponent<AudioSource>().Pause();
        }
        else
        {
            Time.timeScale = 1f;
            pauseMenuCanvas.gameObject.SetActive(false);
            optionsCanvas.gameObject.SetActive(false);
            Cursor.lockState = CursorLockMode.Locked;

            EventSystem.current.SetSelectedGameObject(null);

            if (gameManager.storyMode) dialogueBox.BroadcastMessage("UpdateTextSettings");

            // Resume the music
            audioController.GetComponent<AudioSource>().UnPause();
        }

    }
}
