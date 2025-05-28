using TMPro;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class ObjectiveAndTimerController : MonoBehaviour
{
    public TextMeshProUGUI objective;
    TextSettings _objectiveTextSettings;
    public float displayTime = 2f;
    public float fadeDuration = 1f;
    public ArrowPointer arrowPointer;
    public Transform player;
    public Transform unityChan;
    public Transform exitObject;
    public CarrotInventory carrotInventory;
    public EpisodeController episodeController;
    public string carrotTag = "Carrot";
    public string rabbitTag = "Rabbit";
    public float searchRadius = 200f;
    bool episodeTutorial = false;
    bool trackingRabbit = false;
    float refreshTimer = 0f;
    const float REFRESH_DT = 0.25f;
    CountdownTimer _countdownTimer;

    void Awake()
    {
        _countdownTimer = GetComponent<CountdownTimer>();
    }

    void Start()
    {
        _objectiveTextSettings = objective.GetComponent<TextSettings>();
    }

    void OnEnable()
    {
        EpisodeEvents.OnEpisodeChangeComplete += HandleEpisodeChangeComplete;
        EpisodeEvents.OnEpisodeChanged += HandleEpisodeChanged;
        EpisodeEvents.OnCarrotCollected += HandleCarrotCollected;
        EpisodeEvents.OnCarrotThrown += HandleCarrotThrown;
        EpisodeEvents.OnRabbitFed += HandleRabbitFed;
        EpisodeEvents.OnInitDamage += HandleInitDamage;
        EpisodeEvents.OnFedToFull += HandleFedToFull;
        CountdownTimer.OnTimerFinished += HandleTimerFinished;

    }
    void OnDisable()
    {
        EpisodeEvents.OnEpisodeChangeComplete -= HandleEpisodeChangeComplete;
        EpisodeEvents.OnEpisodeChanged -= HandleEpisodeChanged;
        EpisodeEvents.OnCarrotCollected -= HandleCarrotCollected;
        EpisodeEvents.OnCarrotThrown -= HandleCarrotThrown;
        EpisodeEvents.OnRabbitFed -= HandleRabbitFed;
        EpisodeEvents.OnInitDamage -= HandleInitDamage;
        EpisodeEvents.OnFedToFull -= HandleFedToFull;
        CountdownTimer.OnTimerFinished -= HandleTimerFinished;
    }
    void Update()
    {
        if (!episodeTutorial) return;

        refreshTimer -= Time.deltaTime;
        if (refreshTimer > 0f) return;
        refreshTimer = REFRESH_DT;

        if (trackingRabbit)
        {
            arrowPointer.objective = GetNearestObject(rabbitTag);
        }
        else
        {
            arrowPointer.objective = GetNearestObject(carrotTag);
        }
    }
    public bool IsTutorialCarrot()
    {
        // On the carrot-finding part of the tutorial ONLY
        return episodeTutorial && !trackingRabbit;
    }
    public bool IsTutorialRabbit()
    {
        // On the rabbit-finding part of the tutorial ONLY
        return episodeTutorial && trackingRabbit;
    }

    void HandleTimerFinished()
    {
        if (episodeController.GetEpisode() == 1)
        {
            ShowPopup("Talk to Unity-chan!");
            arrowPointer.objective = unityChan;
            arrowPointer.gameObject.SetActive(true);
        }
        else if (episodeController.GetEpisode() == 2)
        {
            episodeController.StartNextEpisode();
        }
    }
    bool shown = false;
    void HandleInitDamage()
    {
        ShowPopup("You were attacked!! Maybe throwing a carrot will distract them??");
    }
    void HandleRabbitFed()
    {
        if (!episodeTutorial) return;
        episodeTutorial = false;
        arrowPointer.gameObject.SetActive(false);
        if (!shown)
        {
            ShowPopup("Feed the rabbits!");
            shown = true;
        }
    }
    void HandleFedToFull()
    {
        string ft = "Oops! This rabbit's already full!";
        ShowPopup(ft);
    }
    void HandleCarrotThrown(Vector3 pos)
    {
        if (!episodeTutorial || !trackingRabbit) return;

        string ft = "Oops! You need to get close enough to a rabbit!";
        if (carrotInventory.CarrotCount < 1)
        {
            ft += "\nHarvest another carrot!";
            trackingRabbit = false;
            arrowPointer.objective = GetNearestObject(carrotTag);
        }
        ShowPopup(ft);
    }

    void HandleCarrotCollected()
    {
        if (!episodeTutorial || trackingRabbit) return;

        trackingRabbit = true;
        arrowPointer.objective = GetNearestObject(rabbitTag);
        ShowPopup("Find a rabbit!");
        _countdownTimer.EnableTrigger();
    }

    void HandleEpisodeChanged(EpisodeChangedArgs args)
    {
        // No overlay for 4
        if (args.episode == 4)
        {
            ShowPopup("Get to safety!!");
            arrowPointer.objective = exitObject;
        }

    }
    void HandleEpisodeChangeComplete(EpisodeChangedArgs args)
    {
        episodeTutorial = (args.episode == 1);
        trackingRabbit = false;

        switch (args.episode)
        {
            case 1:
                ShowPopup("Objective updated!");
                arrowPointer.objective = GetNearestObject(carrotTag);
                break;
            case 2:
                arrowPointer.objective = null;
                _countdownTimer.StartClockIfNotStarted();
                break;
            case 3:
                ShowPopup("Find Unity-chan.");
                arrowPointer.objective = unityChan;
                arrowPointer.gameObject.SetActive(true);
                break;
            default:
                arrowPointer.objective = null;
                break;
        }
    }
    private Coroutine currentFadeRoutine;

    public void ShowPopup(string text)
    {
        objective.text = text;
        _objectiveTextSettings.ReinitializeDefaultText();

        if (currentFadeRoutine != null)
            StopCoroutine(currentFadeRoutine);

        currentFadeRoutine = StartCoroutine(ShowAndFade());
    }
    private IEnumerator ShowAndFade()
    {
        _objectiveTextSettings.UpdateTextSettings();
        objective.alpha = 1f;

        yield return new WaitForSeconds(displayTime);

        float elapsed = 0f;
        while (elapsed < fadeDuration)
        {
            objective.alpha = Mathf.Lerp(1f, 0f, elapsed / fadeDuration);
            elapsed += Time.deltaTime;
            yield return null;
        }
        objective.alpha = 0f;
    }
    static readonly List<GameObject> cache = new List<GameObject>(64);
    Transform GetNearestObject(string tag)
    {
        cache.Clear();
        GameObject[] objs = GameObject.FindGameObjectsWithTag(tag);
        cache.AddRange(objs);

        Transform nearest = null;
        float minSqr = (searchRadius <= 0f) ? float.PositiveInfinity
                                            : searchRadius * searchRadius;

        Vector3 pos = player.position;
        foreach (var go in cache)
        {
            float sqr = (go.transform.position - pos).sqrMagnitude;
            if (sqr < minSqr)
            {
                minSqr = sqr;
                nearest = go.transform;
            }
        }
        return nearest;
    }
}
