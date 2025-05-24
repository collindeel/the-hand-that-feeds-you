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

    public GameObject questPrefab;
    public GameObject unityChan;
    public DialogueBox dialogueBox;
    public EpisodeController episodeController;
    public bool doesEscapeTriggerMenu = true;
    public GameObject globalVariablesPrefab;

    void OnEnable()
    {
        CountdownTimer.OnTimerFinished += HandleTimerFinished;
        EpisodeEvents.OnEpisodeChangeComplete += HandleEpisodeChangeComplete;
    }

    void Awake()
    {
        var globalVariablesObject = GameObject.FindWithTag("GlobalVariables");
        if (globalVariablesObject == null)
        {
            globalVariablesObject = Instantiate(globalVariablesPrefab);
            globalVariablesObject.tag = "GlobalVariables";
            DontDestroyOnLoad(globalVariablesObject);
        }
    }

    void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;

        var dialogueManager = GetComponent<DialogueManager>();
        Tutorial.Initialize(questPrefab, unityChan, dialogueManager);
    }

    void HandleEpisodeChangeComplete(EpisodeChangedArgs args)
    {
        if (args.episode == 3)
        {
            var dialogueManager = GetComponent<DialogueManager>();
            Episode3AfterStart.Initialize(questPrefab, unityChan, dialogueManager);
        }

    }
    void HandleTimerFinished()
    {
        if (episodeController.GetEpisode() == 1)
        {
            var dialogueManager = GetComponent<DialogueManager>();
            Episode2Prelude.Initialize(questPrefab, unityChan, dialogueManager);
        }
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

            if (storyMode) dialogueBox.BroadcastMessage("UpdateTextSettings");
        }
        
    }
}
