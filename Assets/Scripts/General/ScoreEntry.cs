using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ScoreEntry : MonoBehaviour
{
    public Text PlayerName;
    public Text PlayerScore;

    public void SetScoreText(string _PlayerName, int _PlayerScore)
    {
        PlayerName.text = _PlayerName;
        PlayerScore.text = "Score: " + _PlayerScore.ToString();
    }
}
