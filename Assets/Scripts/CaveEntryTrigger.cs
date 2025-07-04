using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class CaveEntryTrigger : MonoBehaviour
{
    [Header("Win sequence settings")]
    [SerializeField] string playerTag = "Player";
    [SerializeField] float slowMoFactor = 0.1f;
    [SerializeField] float slowMoTime = 0.6f;         // realtime seconds
    [SerializeField] float fadeDuration = 3f;        // realtime
    [SerializeField] float holdWhiteTime = 3f;       // realtime
    public AudioSource audioSource;
    public AudioClip intoLightSound;
    public CanvasGroup finalScoreOverlay;
    public FSPopupController fspc;
    [SerializeField] Image fadeImg;
    GlobalVariables _globalVariables;
    public ScorePopupController scorePC;
    public FloatingScoreDelta fsd;
    public TMP_Text scoreLabel;
    public GameManager gameManager;

    CanvasGroup fadeGroup;
    bool sequenceRunning;

    void Awake()
    {
        if (fadeImg == null)
        {
            Debug.LogError("Fade Image reference missing!");
            enabled = false;
            return;
        }

        fadeGroup = fadeImg.GetComponent<CanvasGroup>()
                   ?? fadeImg.gameObject.AddComponent<CanvasGroup>();
        fadeGroup.alpha = 0;
    }

    void Start()
    {
        _globalVariables = GameObject.FindWithTag("GlobalVariables").GetComponent<GlobalVariables>();
    }

    void OnTriggerEnter(Collider other)
    {
        if (sequenceRunning || !other.CompareTag(playerTag)) return;
        sequenceRunning = true;
        StartCoroutine(WinSequence(other.gameObject));
    }

    System.Collections.IEnumerator WinSequence(GameObject player)
    {
        //-------------------------------------------------
        // 1. Freeze player controls & physics
        //-------------------------------------------------
        var rb = player.GetComponent<Rigidbody>();
        if (rb) rb.isKinematic = true;
        var input = player.GetComponent<MonoBehaviour>(); // Cease player input here
        if (input) input.enabled = false;

        AudioControllerScript.instance.Halt();
        audioSource.clip = intoLightSound;
        audioSource.Play();

        //-------------------------------------------------
        // 2. Slow-mo ramp
        //-------------------------------------------------
        float t = 0;
        float startScale = 1f;
        while (t < slowMoTime)
        {
            Time.timeScale = Mathf.Lerp(startScale, slowMoFactor, t / slowMoTime);
            t += Time.unscaledDeltaTime;
            yield return null;
        }
        Time.timeScale = slowMoFactor;


        //-------------------------------------------------
        // 3. Fade to white (using realtime so it ignores timeScale)
        //-------------------------------------------------
        t = 0;
        while (t < fadeDuration)
        {
            fadeGroup.alpha = Mathf.Lerp(0, 1, t / fadeDuration);
            t += Time.unscaledDeltaTime;
            yield return null;
        }
        fadeGroup.alpha = 1;

        ScoreTracker.isScoreDisabled = false;
        ScoreTracker.AddScore(500);
        if (!ScoreTracker.isScoreDisabled)
        {
            scorePC.ShowPopup(ScoreTracker.GetScore());
            fsd.Play(500);

        }
        ScoreTracker.isScoreDisabled = true;

        _globalVariables.gameCompleted = true;

        //-------------------------------------------------
        // 4. Hold, then load Credits
        //-------------------------------------------------
        yield return new WaitForSecondsRealtime(holdWhiteTime);
        Time.timeScale = 0f;

        StartCoroutine(gameManager.ShowAndExitFlow(true));
    }
}
