using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//Created A New Static Class To Hold Functions For The GameManager Because It Was Getting Messy.
public static class ManagerFunctions
{

    //---------------------------------------------------------------------------------------------

    public static int GetActiveTanks()
    {
        int _TankCount = 0;
        TankMovement[] SceneTanks = GameObject.FindObjectsOfType<TankMovement>();

        foreach (TankMovement T in SceneTanks)
        {
            if (T.gameObject.activeInHierarchy) _TankCount++;
        }

        return _TankCount;
    }

    //---------------------------------------------------------------------------------------------

    public static void EnableTankControls(List<TankManager> _Tanks)
    {
        foreach (TankManager T in _Tanks)
        {
            if (T.ObjectReference != null) T.EnableAgent();
        }
    }

    public static void EnableAgentControls(List<AgentManager> _Agents)
    {
        foreach (AgentManager A in _Agents)
        {
            if (A.ObjectReference != null) A.EnableAgent();
        }
    }

    //---------------------------------------------------------------------------------------------

    public static void DisableTankControls(List<TankManager> _Tanks)
    {
        foreach (TankManager T in _Tanks)
        {
            if (T.ObjectReference != null) T.DisableAgent();
        }
    }

    public static void DisableAgentControls(List<AgentManager> _Agents)
    {
        foreach (AgentManager A in _Agents)
        {
            if (A.ObjectReference != null) A.DisableAgent();
        }
    }

    //---------------------------------------------------------------------------------------------

    public static void ResetSpawnPoints(SpawnPoint[] _SpawnPoints)
    {
        foreach (SpawnPoint S in _SpawnPoints)
        {
            S.bIsUsed = false;
        }
    }

    //---------------------------------------------------------------------------------------------

    public static void SetTargetFrameRate(int _TargetFrameRate)
    {
        QualitySettings.vSyncCount = 0;
        Application.targetFrameRate = _TargetFrameRate;
    }

    //---------------------------------------------------------------------------------------------

    public static void TanksRoundReset(List<TankManager> _Tanks)
    {
        foreach (TankManager T in _Tanks)
        {
            if (T.ObjectReference != null) T.RoundReset();
        }
    }

    public static void AgentsRoundReset(List<AgentManager> _Agents)
    {
        foreach (AgentManager A in _Agents)
        {
            if (A.ObjectReference != null) A.RoundReset();
        }
    }

    //---------------------------------------------------------------------------------------------

    public static void AddScoreToFinalTank()
    {
        TankHealth[] Tank = GameObject.FindObjectsOfType<TankHealth>();
        if (Tank[0].GetComponent<Agent>()) Tank[0].GetComponent<Agent>().AddScore();
        else if (Tank[0].GetComponent<TankMovement>()) Tank[0].GetComponent<TankMovement>().AddScore();
    }

    //---------------------------------------------------------------------------------------------

    public static void EnableScoreBoard(bool _Enable, GameObject _ScorePanel)
    {
        _ScorePanel.SetActive(_Enable);

        if (_Enable)
        {
            ScorePanelManager _scorePanel = _ScorePanel.GetComponent<ScorePanelManager>();
            if (_scorePanel != null)
            {
                _scorePanel.CreateScores();
            }
        }
    }

    //---------------------------------------------------------------------------------------------

}
