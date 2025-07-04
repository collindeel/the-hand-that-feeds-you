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
    GlobalVariables _globalVariables;

    [SerializeField]
    private string serverIP;

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
    void Start()
    {
        _globalVariables = GameObject.FindWithTag("GlobalVariables").GetComponent<GlobalVariables>();
        playerName = _globalVariables.playerName;
    }

    public int Score
    {
        get { return score; }
        set { score = value; }
    }
    public void ResetScore()
    {
        score = 0;
    }

    public IEnumerator UploadScore(int score, Action<bool> callback)
    {
        string json = JsonUtility.ToJson(new ScoreData(playerName, score));
        UnityWebRequest request = new UnityWebRequest($"http://{serverIP}:3001/submit-score", "POST");
        byte[] bodyRaw = System.Text.Encoding.UTF8.GetBytes(json);
        request.uploadHandler = new UploadHandlerRaw(bodyRaw);
        request.downloadHandler = new DownloadHandlerBuffer();
        request.SetRequestHeader("Content-Type", "application/json");

        yield return request.SendWebRequest();

        bool success = request.result == UnityWebRequest.Result.Success;
        callback?.Invoke(success);
    }

    public IEnumerator DownloadScores(Action<bool, ScoreList> callback)
    {
        UnityWebRequest request = UnityWebRequest.Get($"http://{serverIP}:3001/get-scores");

        yield return request.SendWebRequest();

        ScoreList scoreList;
        if (request.result == UnityWebRequest.Result.Success)
        {
            string json = request.downloadHandler.text;
            //Debug.Log("Scores retrieved: " + json);

            scoreList = JsonUtility.FromJson<ScoreList>("{\"scores\":" + json + "}");

            scoreList = FinalizeScoreList(scoreList);
            callback?.Invoke(true, scoreList);
            
        }
        else
        {
            scoreList = new ScoreList();
            callback?.Invoke(false, scoreList);
        }
    }

    public ScoreList FinalizeScoreList(ScoreList scoreList)
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

        if (score != -1 && lse.FindIndex(t => t.name == playerName && t.score == score) == -1)
        {
            ScoreEntry se = new ScoreEntry();
            se.name = playerName;
            se.rank = playerRank;
            se.score = score;
            lse.Add(se);
        }

        scoreList.activePlayerName = playerName;
        scoreList.activePlayerScore = score;
        scoreList.scores = lse;
        return scoreList;
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
