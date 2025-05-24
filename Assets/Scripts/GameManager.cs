using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    public bool isPaused = false;
    public bool storyMode = false;

    public GameObject questPrefab;
    public GameObject unityChan;
    public EpisodeController episodeController;
    public bool doesEscapeTriggerMenu = true;
    public GameObject globalVariablesPrefab;

    void OnEnable()
    {
        CountdownTimer.OnTimerFinished += HandleTimerFinished;
        EpisodeEvents.OnEpisodeChangeComplete += HandleEpisodeChangeComplete;
    }
    void OnDisable()
    {
        CountdownTimer.OnTimerFinished -= HandleTimerFinished;
        EpisodeEvents.OnEpisodeChangeComplete -= HandleEpisodeChangeComplete;
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
}
