using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class ScoreManager : MonoBehaviour
{

    private static ScoreManager _Instance;

    public static ScoreManager Instance { get { return _Instance; } }

    [SerializeField] public Dictionary<string, int> PlayerScores = new Dictionary<string, int>();

    private void Awake()
    {
        if (_Instance != null && _Instance != this)
        {
            Destroy(this.gameObject);
        }
        else
        {
            _Instance = this;
        }
    }

    public int GetSpecificScore(string _Name)
    {

        if (!PlayerScores.ContainsKey(_Name)) return 0;

        return PlayerScores[_Name];
    }


    public string[] GetScores()
    {

        string[] Names = PlayerScores.Keys.ToArray();
        Names.OrderByDescending(N => GetSpecificScore(N)).ToArray();

        return Names;
    }

    public void AddScore(int _score, string _playername)
    {
        if (!PlayerScores.ContainsKey(_playername))
        {
            PlayerScores.Add(_playername, _score);
            return;
        }

        if (PlayerScores.ContainsKey(_playername))
        {
            PlayerScores[_playername] = _score;
        }
    }

    public void ClearScores()
    {
        PlayerScores.Clear();
    }


}
