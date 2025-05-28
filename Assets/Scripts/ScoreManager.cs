using UnityEngine;
using UnityEngine.Networking;

using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

public class ScoreManager : MonoBehaviour
{
    public static ScoreManager instance;
    private int score = -1;
    private string playerName = "Player";

    void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }
    public event Action<ScoreList> OnScoresRetrieved;

    public int Score => score;
    public void ResetScore()
    {
        score = 0;
    }
    public void SetPlayerName(string name)
    {
        if (!string.IsNullOrEmpty(name))
        {
            this.playerName = name;
            Debug.Log("Player name set to: " + this.playerName);
        }
    }

    public IEnumerator UploadScore(int score)
    {
        string json = JsonUtility.ToJson(new ScoreData(playerName, score));
        UnityWebRequest request = new UnityWebRequest("http://XXXXXXXX      # literal IP → placeholder:3001/submit-score", "POST");
        byte[] bodyRaw = System.Text.Encoding.UTF8.GetBytes(json);
        request.uploadHandler = new UploadHandlerRaw(bodyRaw);
        request.downloadHandler = new DownloadHandlerBuffer();
        request.SetRequestHeader("Content-Type", "application/json");

        yield return request.SendWebRequest();

        if (request.result == UnityWebRequest.Result.Success)
            Debug.Log("Score uploaded successfully for: " + playerName);
        else
            Debug.LogWarning("Error uploading score: " + request.error);
    }

    public IEnumerator DownloadScores()
    {
        UnityWebRequest request = UnityWebRequest.Get($"http://XXXXXXXX      # literal IP → placeholder:3001/get-scores");

        yield return request.SendWebRequest();

        ScoreList scoreList;
        if (request.result == UnityWebRequest.Result.Success)
        {
            string json = request.downloadHandler.text;
            Debug.Log("Scores retrieved: " + json);

            scoreList = JsonUtility.FromJson<ScoreList>("{\"scores\":" + json + "}");

            FinalizeScoreListAndRaiseEvent(scoreList);
        }
        else
        {
            scoreList = new ScoreList();
            Debug.LogWarning("Error retrieving scores: " + request.error);
            OnScoresRetrieved?.Invoke(scoreList);
        }
    }

    public void FinalizeScoreListAndRaiseEvent(ScoreList scoreList)
    {
        //score = ScoreTracker.GetScore();

        // It's a cold load, so make up a score
        /*if (score == 0)
        {
            score = UnityEngine.Random.Range(100, 5001);
            print($"Making up a score, {score}");

            ScoreEntry se = new ScoreEntry();
            se.name = playerName;
            se.score = score;
            scoreList.scores.Add(se);
        }*/

        List<ScoreEntry> lse = GetOrderedScores(scoreList);
        int playerRank = 0;

        playerRank = GetRank(scoreList, score);

        foreach (ScoreEntry s in lse)
        {
            s.rank = GetRank(scoreList, s.score);
        }

        /*if (score != -1 && lse.FindIndex(t => t.name == playerName && t.score == score) == -1)
        {
            ScoreEntry se = new ScoreEntry();
            se.name = playerName;
            se.rank = playerRank;
            se.score = score;
            lse.Add(se);
        }*/

        scoreList.activePlayerName = "tommy"; //playerName;
        scoreList.activePlayerScore = 3;  //score;
        scoreList.scores = lse;
        OnScoresRetrieved?.Invoke(scoreList);
    }

    public int GetRank(ScoreList scoreList, int curScore)
    {
        return scoreList.scores.OrderByDescending(s => s.score)
            .ToList()
            .Count(s => s.score > curScore) + 1;
    }

    public List<ScoreEntry> GetOrderedScores(ScoreList scoreList)
    {
        return scoreList.scores.OrderByDescending(s => s.score).ToList();
    }

    [Serializable]
    public class ScoreEntry
    {
        public int rank;
        public string name;
        public int score;
    }

    [Serializable]
    public class ScoreList
    {
        public string activePlayerName;
        public int activePlayerScore;
        public List<ScoreEntry> scores;
    }

    [Serializable]
    public class ScoreData
    {
        public string name;
        public int score;
        public ScoreData(string n, int s) { name = n; score = s; }
    }

}
