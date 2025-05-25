using TMPro;
using UnityEngine;
using System;

public class CountdownTimer : MonoBehaviour
{
    [Header("Timer Settings")]
    public float totalTime = 90f;

    [Header("UI")]
    public TextMeshProUGUI timerLabel;
    TextSettings _timerLabelTextSettings;
    public Color normalColor = Color.white;
    public Color last10sColor = Color.red;

    public static event Action OnTimerFinished;

    float timeRemaining;
    bool running = false;

    // void OnEnable()
    // {
    //     EpisodeEvents.OnRabbitFed += StartClockIfNotStarted;
    // }
    // void OnDisable()
    // {
    //     EpisodeEvents.OnRabbitFed -= StartClockIfNotStarted;
    // }

    public void EnableTrigger()
    {
        EpisodeEvents.OnRabbitFed += StartClockIfNotStarted;
    }

    void Start()
    {
        _timerLabelTextSettings = timerLabel.GetComponent<TextSettings>();
    }

    void Update()
    {
        if (!running) return;

        timeRemaining -= Time.deltaTime;
        if (timeRemaining <= 0f)
        {
            timeRemaining = 0f;
            running = false;
            UpdateLabel(0f);
            ScoreTracker.isScoreDisabled = true;
            // pause?
            timerLabel.alpha = 0f;
            OnTimerFinished?.Invoke();
            return;
        }
        UpdateLabel(timeRemaining);
    }

    public bool IsTimerRunning()
    {
        return running;
    }

    public void StartClockIfNotStarted()
    {
        if (running) return; // Or let them reset clock every time they feed a rabbit and remove this line
        ScoreTracker.isScoreDisabled = false;
        timeRemaining = totalTime;
        running = true;
        UpdateLabel(timeRemaining);
        timerLabel.gameObject.SetActive(true);
        EpisodeEvents.OnRabbitFed -= StartClockIfNotStarted;
    }

    void UpdateLabel(float seconds)
    {
        int m = Mathf.FloorToInt(seconds / 60);
        int s = Mathf.FloorToInt(seconds % 60);
        timerLabel.text = $"{m:00}:{s:00}";
        _timerLabelTextSettings.ReinitializeDefaultText();
        timerLabel.color = (seconds <= 10f) ? last10sColor : normalColor;
    }
}
