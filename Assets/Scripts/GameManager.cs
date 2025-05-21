using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    public Canvas pauseMenu;
    public Canvas optionsMenu;
    public Button resumeButton;
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

    public void TogglePauseGame()
    {
        isPaused ^= true;

        if (isPaused)
        {
            Time.timeScale = 0f;
            pauseMenu.gameObject.SetActive(true);
            optionsMenu.gameObject.SetActive(false);
            Cursor.lockState = CursorLockMode.None;

            EventSystem.current.SetSelectedGameObject(resumeButton.gameObject);
        }
        else
        {
            Time.timeScale = 1f;
            pauseMenu.gameObject.SetActive(false);
            optionsMenu.gameObject.SetActive(false);
            Cursor.lockState = CursorLockMode.Locked;

            EventSystem.current.SetSelectedGameObject(null);
        }
        
    }
}
