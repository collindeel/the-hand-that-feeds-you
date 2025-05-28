using TMPro;
using UnityEngine;
using UnityEngine.UI;
public class LeaderboardUI : MonoBehaviour
{
    [SerializeField] private ScoreManager scoreManager;
    [SerializeField] private Transform contentContainer;
    [SerializeField] private GameObject scoreRowPrefab;
    [SerializeField] private RectTransform pinnedPlayerEntry;
    private RectTransform playerScrollRow;
    public RectTransform PlayerScrollRow => playerScrollRow;
    public FSPopupController fspc;
    public TMP_Text finalScoreLabel;

    void OnEnable()
    {
        scoreManager.OnScoresRetrieved += Populate;
        scoreManager.OnNoConnect += ShowScore;
    }

    void OnDisable()
    {
        scoreManager.OnScoresRetrieved -= Populate;
        scoreManager.OnNoConnect -= ShowScore;
    }

    void ShowScore()
    {
        fspc.ShowPopup(ScoreTracker.GetScore());
        finalScoreLabel.text = "Final Score";
    }

    void Populate(ScoreManager.ScoreList scoreList)
    {
        // Clear existing entries
        foreach (Transform child in contentContainer)
        {
            Destroy(child.gameObject);
        }

        bool isAMatchFound = false;
        // Add new ones
        foreach (var entry in scoreList.scores)
        {
            GameObject rowObj = Instantiate(scoreRowPrefab, contentContainer);
            ScoreRow row = rowObj.GetComponent<ScoreRow>();
            row.Init(entry.rank, entry.name, entry.score);
            //print($"Init with {entry.rank},{entry.name}, and {entry.score}");
            if (!isAMatchFound && entry.name == scoreList.activePlayerName && entry.score == scoreList.activePlayerScore)
            {
                isAMatchFound = true;
                playerScrollRow = rowObj.GetComponent<RectTransform>();
                pinnedPlayerEntry.GetComponent<ScoreRow>().Init(entry.rank, entry.name, entry.score);
                if (entry.rank > 9)
                {
                    pinnedPlayerEntry.gameObject.SetActive(true);
                }

                ScoreRow scrollRow = playerScrollRow.GetComponent<ScoreRow>();
                ScoreRow pinnedRow = pinnedPlayerEntry.GetComponent<ScoreRow>();

                scrollRow.nameText.color = pinnedRow.nameText.color;
                scrollRow.rankText.color = pinnedRow.rankText.color;
                scrollRow.scoreText.color = pinnedRow.rankText.color;
            }
        }

    }
}
