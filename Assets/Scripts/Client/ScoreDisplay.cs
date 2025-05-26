using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public enum ScoreType { HIGHSCORE = 0, RECENT = 1}
namespace NetGame.Client
{
    public class ScoreDisplay : GenericSingleton<ScoreDisplay>
    {
        [SerializeField] private GameObject scoreDisplayObj;
        [SerializeField] private TextMeshProUGUI highscoreText;
        [SerializeField] private TextMeshProUGUI recentScoreText;

        public void ShowScore(List<ScoreInfo> scores, ScoreType scoreType)
        {
            TextMeshProUGUI editScore = (scoreType == ScoreType.HIGHSCORE) ? highscoreText : recentScoreText;

            string scoreText = "";
            for (int i = 0; i < scores.Count; i++)
            {
                scoreText += scores[i].ToString() + "\n";
            }

            editScore.text = scoreText;

            scoreDisplayObj.SetActive(true);
        }
    }
}

[System.Serializable]
public class ScoreInfo
{
    public string nickname;
    public int score;
    public string date;

   public ScoreInfo(string nickname, int score, string date)
    {
        this.nickname = nickname;
        this.score = score;
        this.date = date;
    }

    public override string ToString()
    {
        return $"{nickname} {score} - {date}";
    }
}