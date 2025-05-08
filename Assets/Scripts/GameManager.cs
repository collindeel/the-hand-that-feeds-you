using UnityEngine;
using UnityEngine.InputSystem;

public class GameManager : MonoBehaviour
{

    void Start()
    {
        
    }

    void Update()
    {
        if (Keyboard.current.escapeKey.wasPressedThisFrame) TogglePauseGame();
    }

    public Canvas pauseMenu;
    public Canvas optionsMenu;
    bool _isPaused;
    

    public void TogglePauseGame() {
        _isPaused ^= true;
        Time.timeScale = _isPaused ? 0f : 1f;
        pauseMenu.enabled = _isPaused;
        optionsMenu.enabled = false;
    }
}
