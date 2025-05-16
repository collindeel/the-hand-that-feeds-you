using UnityEngine;
using UnityEngine.InputSystem;

public class GameManager : MonoBehaviour
{

    void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
    }

    void Update()
    {
        if (Keyboard.current.escapeKey.wasPressedThisFrame) TogglePauseGame();
    }

    public Canvas pauseMenu;
    public Canvas optionsMenu;
    public bool isPaused = false;
    public bool storyMode = false;

    public void TogglePauseGame() {
        isPaused ^= true;
        Time.timeScale = isPaused ? 0f : 1f;
        pauseMenu.enabled = isPaused;
        optionsMenu.enabled = false;
        Cursor.lockState = isPaused ? CursorLockMode.None : CursorLockMode.Locked;
    }
}
