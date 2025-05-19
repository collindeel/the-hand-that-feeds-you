using UnityEngine;
using UnityEngine.UI;
using UnityEngine.InputSystem;   // if you use the new Input System
using TMPro;                     // if you use TextMeshPro

public enum RabbitBehaviorLevel { Timid, Medium, Aggressive }
public class EpisodeController : MonoBehaviour
{
    [Header("UI")]
    [SerializeField] GameObject overlay;          // EpisodeOverlay
    [SerializeField] TMP_Text label;            // EpisodeLabel

    [Header("Rabbit Switching")]

    int episode = 1;

    /* --- keys --- */
    const Key showKey = Key.Period;      // trigger next episode
    const Key resumeKey = Key.Space;  // dismiss overlay
    void Start()
    {
        ShowOverlay();
    }
    void Update()
    {
        var kb = Keyboard.current;
        if (kb == null) return;

        // Trigger next episode
        if (kb[showKey].wasPressedThisFrame && !overlay.activeSelf)
            StartNextEpisode();

        // Resume play
        if (kb[resumeKey].wasPressedThisFrame && overlay.activeSelf)
            HideOverlay();
    }

    /* ====================== */

    void StartNextEpisode()
    {
        episode++;

        // Decide target rabbit level
        RabbitBehaviorLevel level = RabbitBehaviorLevel.Timid;
        if (episode == 2) level = RabbitBehaviorLevel.Medium;
        else if (episode >= 3) level = RabbitBehaviorLevel.Aggressive;

        var rabbits =
            Object.FindObjectsByType<RabbitModelSwitcher>(FindObjectsSortMode.None);

        foreach (var r in rabbits)
            r.SetLevel(level);

        ShowOverlay();
    }
    void ShowOverlay()
    {
        string ft = "";
        switch (episode)
        {
            case 1:
                ft = "Give carrots to the sweet rabbits";
                break;
            case 2:
                ft = "Don't let the rabbits take your carrots";
                break;
            case 3:
            default:
                ft = "F*cking run";
                break;
        }
        label.text = $"Episode {episode}\n\n{ft}";
        overlay.SetActive(true);
        Time.timeScale = 0f;
    }
    void HideOverlay()
    {
        overlay.SetActive(false);
        Time.timeScale = 1f;
    }
}
