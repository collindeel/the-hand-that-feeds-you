using System;
using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public bool isPaused = false;
    public bool storyMode = false;

    public GameObject questPrefab;
    public GameObject unityChan;
    public EpisodeController episodeController;
    public bool doesEscapeTriggerMenu = true;
    public GameObject globalVariablesPrefab;
    GlobalVariables _globalVariables;
    public CanvasGroup finalScoreOverlay;
    public LeaderboardUI leaderboardUI;
    public LeaderboardScroller leaderboardScroller;

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
        _globalVariables = globalVariablesObject.GetComponent<GlobalVariables>();
    }

    void Start()
    {
        if (PlayerPrefs.GetInt("IsHighRes", 1) == 1)
            PerfTuner.Apply(PerfTier.High);
        else
            PerfTuner.Apply(PerfTier.Low);

        Cursor.lockState = CursorLockMode.Locked;

        var dialogueManager = GetComponent<DialogueManager>();
        Tutorial.Initialize(questPrefab, unityChan, dialogueManager);
    }

    bool skipRequested = false;
    void Update()
    {
        if (Keyboard.current.spaceKey.wasPressedThisFrame)
        {
            skipRequested = true;
        }
    }

    public void SetPerfTier(bool isHigh)
    {
        int index = isHigh ? 1 : 0;
        PerfTuner.Apply((PerfTier)index);
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

    private IEnumerator UploadThenDownload(Action<bool, ScoreManager.ScoreList> callback)
    {
        ScoreManager.instance.Score = ScoreTracker.GetScore();

        bool uploadSuccess = false;
        bool downloadSuccess = false;
        ScoreManager.ScoreList scoreList = null;

        yield return StartCoroutine(ScoreManager.instance.UploadScore(ScoreTracker.GetScore(), result => uploadSuccess = result));
        if (uploadSuccess)
        {
            yield return StartCoroutine(ScoreManager.instance.DownloadScores((result, list) =>
            {
                downloadSuccess = result;
                scoreList = list;
            }));
        }

        callback?.Invoke(uploadSuccess && downloadSuccess, scoreList);
    }

    public float maxDisplayTimeForScrolling = 40f; // Even if it takes longer, don't wait longer than this
    public float minDisplayTime = 3f; // At least wait this long

    public IEnumerator WaitForSecondsRealtimeUnlessSkipped(float seconds)
    {
        float timer = 0f;
        while (timer < seconds)
        {
            if (Keyboard.current.spaceKey.wasPressedThisFrame)
            {
                skipRequested = true;
                break;
            }
            timer += Time.unscaledDeltaTime;
            yield return null;
        }
    }

    public IEnumerator ShowAndExitFlow(bool isWon)
    {
        if (isWon)
            AudioControllerScript.instance.PlayEndWon();
        else
            AudioControllerScript.instance.PlayEndDied();

        finalScoreOverlay.BroadcastMessage("UpdateTextSettings");
        finalScoreOverlay.alpha = 1f;

        float startTime = Time.realtimeSinceStartup;

        bool didSucceed = false;
        ScoreManager.ScoreList scoreList = null;

        yield return StartCoroutine(UploadThenDownload((success, list) =>
        {
            didSucceed = success;
            scoreList = list;
        }));

        if (didSucceed)
        {
            leaderboardUI.ShowScoreLabel("Top Scores");
            leaderboardUI.ShowLeaderboard(scoreList);
            float scrollStart = Time.realtimeSinceStartup;
            while (!leaderboardScroller.hasFinishedScrolling &&
                   Time.realtimeSinceStartup - scrollStart < maxDisplayTimeForScrolling &&
                   !skipRequested)
            {
                yield return null;
            }
            float scrollEnd = Time.realtimeSinceStartup;
            float scrollDuration = scrollEnd - scrollStart;
            yield return WaitForSecondsRealtimeUnlessSkipped(Mathf.Max(0f, minDisplayTime - scrollDuration));
        }
        else
        {
            leaderboardUI.ShowScore();
            leaderboardUI.ShowScoreLabel("Final Score");
            float elapsed = Time.realtimeSinceStartup - startTime;
            float remaining = Mathf.Max(0f, minDisplayTime - elapsed);
            yield return WaitForSecondsRealtimeUnlessSkipped(remaining);
        }
        yield return WaitForSecondsRealtimeUnlessSkipped(2f); // And hold for applause

        Cursor.lockState = CursorLockMode.None;
        _globalVariables.gameCompleted = true;
        SceneManager.LoadScene("Main Menu");
    }

}
