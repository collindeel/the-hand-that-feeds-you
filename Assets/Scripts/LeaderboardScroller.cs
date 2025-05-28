using UnityEngine;

public class LeaderboardScroller : MonoBehaviour
{
    [SerializeField] private LeaderboardUI leaderboardUI;
    [SerializeField] private RectTransform contentContainer;
    [SerializeField] private RectTransform pinnedPlayerEntry;
    [SerializeField] private float scrollSpeed = 70f;
    [SerializeField] private float scrollDelay = 1f; // seconds
    private float delayTimer = 0f;
    private bool hasStartedScrolling = false;
    private bool hasFinishedScrolling = false;

    private RectTransform playerEntryInScroll;

    void Update()
    {
        if (hasFinishedScrolling) return;
        if (playerEntryInScroll == null)
        {
            playerEntryInScroll = leaderboardUI.PlayerScrollRow;
            if (playerEntryInScroll == null) return;
        }
        if (!hasStartedScrolling)
        {
            delayTimer += Time.unscaledDeltaTime;
            if (delayTimer < scrollDelay) return;
            hasStartedScrolling = true;
        }

        float contentBottomY = contentContainer.anchoredPosition.y;
        float contentHeight = contentContainer.rect.height;
        float viewportHeight = ((RectTransform)contentContainer.parent).rect.height;

        if (contentBottomY + viewportHeight >= contentHeight - 1f)
        {
            hasFinishedScrolling = true;
            return;
        }
        contentContainer.anchoredPosition += Vector2.up * scrollSpeed * Time.unscaledDeltaTime;
        Vector3 playerWorldPos = playerEntryInScroll.position;
        Vector3 pinnedWorldPos = pinnedPlayerEntry.position;

        if (playerWorldPos.y >= pinnedWorldPos.y)
        {
            pinnedPlayerEntry.gameObject.SetActive(false);
        }
    }
}