using TMPro;
using UnityEngine;
using System;

public class CountdownTimer : MonoBehaviour
{
    [Header("Timer Settings")]
    public float totalTime = 90f;

    [Header("UI")]
    public TMP_Text timerLabel;
    public Color normalColor = Color.white;
    public Color last10sColor = Color.red;

    public static event Action OnTimerFinished;

    float timeRemaining;
    bool running = false;

    void OnEnable()
    {
        EpisodeEvents.OnRabbitFed += HandleTutorialDone;
    }
    void OnDisable()
    {
        EpisodeEvents.OnRabbitFed -= HandleTutorialDone;
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
            // pause?
            timerLabel.alpha = 0f;
            OnTimerFinished?.Invoke();
            return;
        }
        UpdateLabel(timeRemaining);
    }

    void HandleTutorialDone()
    {
        timeRemaining = totalTime;
        running = true;
        UpdateLabel(timeRemaining);
        timerLabel.gameObject.SetActive(true);
    }

    void UpdateLabel(float seconds)
    {
        int m = Mathf.FloorToInt(seconds / 60);
        int s = Mathf.FloorToInt(seconds % 60);
        timerLabel.text = $"{m:00}:{s:00}";
        timerLabel.color = (seconds <= 10f) ? last10sColor : normalColor;
    }
}
