public static class ScoreTracker
{
    public static int GetScore()
    {
        return score;
    }
    public static void SetScore(int score)
    {
        if (!isScoreDisabled)
            ScoreTracker.score = score;
    }
    public static void AddScore(int scoreToAdd)
    {
        if (!isScoreDisabled)
            score += scoreToAdd;
    }
    public static void Reset(int startValue = 0, bool disable = true)
    {
        score = startValue;
        isScoreDisabled = disable;
    }

    public static bool isScoreDisabled { get; set; } = true;
    static int score = 0;
}
