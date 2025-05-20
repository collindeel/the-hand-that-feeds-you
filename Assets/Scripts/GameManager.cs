using UnityEngine;
using UnityEngine.InputSystem;

public class GameManager : MonoBehaviour
{
    public Canvas pauseMenu;
    public Canvas optionsMenu;
    public bool isPaused = false;
    public bool storyMode = false;

    public GameObject objectivePrefab;
    public GameObject unityChan;
    public GameObject speechBubblePrefab;
    public bool doesEscapeTriggerMenu = true;

    void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;

        var dialogueManager = GetComponent<DialogueManager>();
        Tutorial.Initialize(objectivePrefab, unityChan, speechBubblePrefab, dialogueManager);
    }

    void Update()
    {
        if (doesEscapeTriggerMenu && Keyboard.current.escapeKey.wasPressedThisFrame) TogglePauseGame();
    }

    public void TogglePauseGame() {
        isPaused ^= true;
        Time.timeScale = isPaused ? 0f : 1f;
        pauseMenu.enabled = isPaused;
        optionsMenu.enabled = false;
        Cursor.lockState = isPaused ? CursorLockMode.None : CursorLockMode.Locked;
    }
}
