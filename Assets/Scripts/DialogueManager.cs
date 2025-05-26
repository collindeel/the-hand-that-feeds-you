using UnityEngine;
using UnityEngine.InputSystem;

public class DialogueManager : MonoBehaviour
{
    public DialogueBox dialogueBox;
    public EpisodeController episodeController;

    GameManager _gameManager;
    InputAction _interactAction;
    InputAction _jumpAction;
    int _currentLine = -1;

    (string text, string speaker)[] _dialogue;

    void Start()
    {
        _gameManager = GetComponent<GameManager>();
        _interactAction = InputSystem.actions.FindAction("Interact");
        _jumpAction = InputSystem.actions.FindAction("Jump");
    }

    void Update()
    {
        if ((_interactAction.WasPressedThisFrame() || _jumpAction.WasPressedThisFrame()) && !_gameManager.isPaused && _gameManager.storyMode)
            if (_currentLine < _dialogue.Length - 1)
                DisplayNextLine();
            else
            {
                dialogueBox.Hide();
                _gameManager.storyMode = false;
                _currentLine = -1;
                episodeController.StartNextEpisodeIfApplicable();
            }
    }

    void DisplayNextLine()
    {
        _currentLine++;
        dialogueBox.DisplayText(_dialogue[_currentLine]);
    }

    public void PlayDialogue((string text, string speaker)[] dialogue)
    {
        _dialogue = dialogue;
        _gameManager.storyMode = true;
        DisplayNextLine();
    }
}
