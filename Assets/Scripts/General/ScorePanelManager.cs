using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class ScorePanelManager : MonoBehaviour
{
    private ScoreManager _ScoreManager;
    [SerializeField] GameObject _ScoreEntry;
    [SerializeField] GameObject _ScoreList;

    public void CreateScores()
    {
        ClearCurrentScores();

        _ScoreManager = ScoreManager.Instance;

        string[] Names = _ScoreManager.GetScores();

        for (int x = 0; x < Names.Length; x++)
        {
            GameObject Entry = Instantiate(_ScoreEntry) as GameObject;
            Entry.transform.SetParent(_ScoreList.transform);
            Entry.GetComponent<ScoreEntry>().SetScoreText(Names[x], _ScoreManager.GetSpecificScore(Names[x]));
        }
    }

    public void ClearCurrentScores()
    {
        ScoreEntry[] _Scores =  gameObject.GetComponentsInChildren<ScoreEntry>();
        foreach (ScoreEntry S in _Scores)
        {
            Destroy(S.gameObject);
        }
    }
}
