using UnityEngine;

public class DialogueManager : MonoBehaviour
{
    public DialogueBox dialogueBox;

    GameManager _gameManager;
    int _currentLine = -1;

    (string text, string speaker)[] _dialogue;

    void Start()
    {
        _gameManager = GetComponent<GameManager>();
    }

    void Update()
    {
        if ((Input.GetMouseButtonDown(0) || Input.GetKeyDown(KeyCode.Space)) && !_gameManager.isPaused && _gameManager.storyMode)
            // if (isAnimating)
            //     SkipAnimation();
            if (_currentLine < _dialogue.Length - 1)
                DisplayNextLine();
            else
            {
                dialogueBox.Hide();
                _gameManager.storyMode = false;
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
