//using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[System.Serializable]
public struct Behaviour
{
    public string BehaviourName;
    public Color TankColour;

    public float ReloadTime;
    public float RotationAccuracy;
    public float RotationSpeed;
    [SerializeField, Range(1.0f, 1.5f)] public float MinDistAccuracy;
    [SerializeField, Range(1.0f, 3.0f)] public float MaxDistAccuracy;


    public Behaviour(float _reloadspeed = 500.0f, float _rotationaccuracy = 10.0f, float _mindistaccuracy = 1.0f, float _maxdistaccuracy = 1.2f, float _rotationspeed = 1.0f)
    {
        BehaviourName = "Default";
        TankColour = Color.red;
        ReloadTime = _reloadspeed;
        RotationAccuracy = _rotationaccuracy;
        MinDistAccuracy = _mindistaccuracy;
        MaxDistAccuracy = _maxdistaccuracy;
        RotationSpeed = _rotationspeed;
    }
}

public class GameManager : MonoBehaviour
{
    public Text MessageText;
    public Text CountDownText;

    public GameObject MenuButton;
    public GameObject SpeedButton;
    public GameObject ScorePanel;
    public GameObject PauseMenu;

    public GameObject MenuCanvas;
    public GameObject MainMenu;
    public GameObject ConfigMenu;

    //
    private int NumOfPlayers;
    private int NumOfBots;
    private int NumOfRounds;

    private float TimeLeft = 3;

    //Prefab variables.
    public GameObject Tank_Prefab;
    public GameObject Agent_Prefab;
    public GameObject Mine_Prefab;

    //Spawn points
    private SpawnPoint[] Spawns;

    public List<AgentManager> Agents;
    public List<TankManager> Tanks;

    public List<Behaviour> AgentBehaviours;

    private List<string> UsedBehaviours = new List<string>();

    public int MineSpawnDelay = 0;
    private float FrameCount = 0;

    public int MaximumFPS = 60;

    private int CurrentRound = 0;

    private IEnumerator CurrentState;
    private IEnumerator NextState;

    // Use this for initialization
    void Start ()
    {
        //Time.timeScale = 5;

        //Sets Target Frame Rate
        ManagerFunctions.SetTargetFrameRate(MaximumFPS);

        //Gets All SpawnPoint Game objects From The Scene.
        Spawns = GameObject.FindObjectsOfType<SpawnPoint>();
    }

    IEnumerator StateMachine()
    {
        while (CurrentState != null)
        {
            yield return StartCoroutine(CurrentState);
            CurrentState = NextState;
            NextState = null;
        }
    }

    //----------------------------------------------------------------------------------------------------------------------------------------------

    private IEnumerator RoundStart()
    {
        TimeLeft = 5;

        CurrentRound++;

        while (NextState == null)
        {

            while (TimeLeft > 1)
            {

                TimeLeft -= Time.deltaTime;

                if (TimeLeft < 3)
                {
                    CountDownText.text = (TimeLeft).ToString("0");
                    MessageText.text = string.Empty;
                }
                else
                {
                    CountDownText.text = string.Empty;
                    MessageText.text = "ROUND " + CurrentRound.ToString();
                }




                yield return null;
            }

            NextState = RoundPlaying();

            yield return null;
        }
    }

    private IEnumerator RoundPlaying()
    {

        ManagerFunctions.EnableScoreBoard(false, ScorePanel);

        // As soon as the round begins playing let the players control the tanks.
        ManagerFunctions.EnableTankControls(Tanks);
        ManagerFunctions.EnableAgentControls(Agents);

        CountDownText.text = string.Empty;
        MessageText.text = string.Empty;

        while (NextState == null)
        {

            // While there is not one tank left...
            while (GameObject.FindObjectsOfType<TankHealth>().Length > 1)
            {
                if (Input.GetButtonDown("Pause"))
                {
                    PauseMenu.SetActive(true);
                    SetGameTimeScale(0.0f);
                }

                FrameCount += Time.deltaTime;
                if (FrameCount > MineSpawnDelay)
                {
                    SpawnRandomMine();
                    FrameCount = 0;
                }

                if (ManagerFunctions.GetActiveTanks() < 1 && !SpeedButton.activeInHierarchy)
                {
                    SpeedButton.SetActive(true);
                }

                yield return null;
            }

            if (CurrentRound < NumOfRounds)
            {
                ManagerFunctions.AddScoreToFinalTank();
                NextState = RoundBreak();
            }
            else
            {
                ManagerFunctions.AddScoreToFinalTank();
                NextState = GameEnding();
            }

            yield return null;

        }
    }

    private IEnumerator RoundBreak()
    {
        TimeLeft = 10;

        CurrentRound++;

        StartNewRound();

        ManagerFunctions.EnableScoreBoard(true, ScorePanel);

        while (TimeLeft > 1)
        {
            TimeLeft -= Time.deltaTime;

            if (TimeLeft > 3)
            {
                CountDownText.text = string.Empty;
                MessageText.text = "ROUND " + CurrentRound.ToString();
            }
            else
            {
                if (ScorePanel.activeInHierarchy) ManagerFunctions.EnableScoreBoard(false, ScorePanel);
                CountDownText.text = (TimeLeft).ToString("0");
                MessageText.text = string.Empty;
            }

            yield return null;
        }

        NextState = RoundPlaying();

        yield return null;

    }

    private IEnumerator GameEnding()
    {
        //// As soon as the round begins playing let the players control the tanks.
        ManagerFunctions.ResetSpawnPoints(Spawns);

        NameManager.Instance.InitNames();

        ManagerFunctions.EnableScoreBoard(true, ScorePanel);

        ScoreManager.Instance.ClearScores();

        StartNewRound();

        PlayerScore[] TankScores = GameObject.FindObjectsOfType<PlayerScore>();

        PlayerScore WinningTank = TankScores[0];
        foreach (PlayerScore S in TankScores)
        {
            if (S.Score > WinningTank.Score)
            {
                WinningTank = S;
            }
        }

        MessageText.text = WinningTank.GetComponent<PlayerName>().GetCurrentName() + " WINS";

        WinningTank.gameObject.transform.position = new Vector3(0.0f, 1.0f, 0.0f);

        Camera MainCamera = GameObject.FindObjectOfType<Camera>();

        MainCamera.GetComponent<Animator>().SetTrigger("WinIn");

        MenuButton.SetActive(true);

        StopAllCoroutines();

        yield return null;
    }

    //Apply the game settings
    public void ApplyGameSettings(int _NPlayers, int _NBots, int _NRounds)
    {
        NumOfPlayers = _NPlayers;
        NumOfBots = _NBots;
        NumOfRounds = _NRounds;
    }

    //This function is responsible for starting a new game.
    public void StartNewGame()
    {
        DestroyTanks();
        ResetTanks();

        SpawnTanks();
        SpawnAgents();

        CurrentRound = 0;

        CurrentState = RoundStart();
        StartCoroutine(StateMachine());
    }

    //This function is responsible for starting a new round.
    public void StartNewRound()
    {
        SetGameTimeScale(1.0f);

        if (SpeedButton.activeInHierarchy) SpeedButton.SetActive(false);

        //Active all tanks.
        ManagerFunctions.TanksRoundReset(Tanks);
        ManagerFunctions.AgentsRoundReset(Agents);

        //Disable all tank controls.
        ManagerFunctions.DisableTankControls(Tanks);
        ManagerFunctions.DisableAgentControls(Agents);

        ClearMines();
        ClearShells();
    }

    public void ReturnToMenu()
    {
        StopAllCoroutines();

        StartNewRound();

        ManagerFunctions.ResetSpawnPoints(Spawns);

        NameManager.Instance.InitNames();

        ScoreManager.Instance.ClearScores();

        Camera MainCamera = GameObject.FindObjectOfType<Camera>();

        DestroyTanks();

        PauseMenu.SetActive(false);

        MenuCanvas.SetActive(true);
        MainMenu.SetActive(true);
        ConfigMenu.SetActive(false);
    }

    public void SetGameTimeScale(float _NewTimeScale)
    {
        Time.timeScale = _NewTimeScale;
    }

    private void SpawnTanks()
    {
        for (int b = 0; b < NumOfPlayers; b++)
        {
            while (!Tanks[b].bisSpawned)
            {

                int RandNum = Random.Range(0, Spawns.Length);
                SpawnPoint SpawnLocation = Spawns[RandNum];

                if (!SpawnLocation.bIsUsed)
                {
                    Tanks[b].ObjectReference = Instantiate(Tank_Prefab, SpawnLocation.transform.position, SpawnLocation.transform.rotation) as GameObject;
                    Tanks[b].PlayerNum = b + 1;
                    Tanks[b].SetUp();
                    Tanks[b].DisableAgent();

                    Tanks[b].bisSpawned = true;
                    Tanks[b].SpawnLocation = SpawnLocation.transform.position;
                    Tanks[b].SpawnRotation = SpawnLocation.transform.rotation;

                    SpawnLocation.bIsUsed = true;
                }
            }
        }
    }

    private void SpawnAgents()
    {
        for (int b = 0; b < NumOfBots; b++)
        {
            while (!Agents[b].bisSpawned)
            {

                int RandNum = Random.Range(0, Spawns.Length);
                SpawnPoint SpawnLocation = Spawns[RandNum];

                if (!SpawnLocation.bIsUsed)
                {
                    Agents[b].ObjectReference = Instantiate(Agent_Prefab, SpawnLocation.transform.position, SpawnLocation.transform.rotation) as GameObject;
                    Agents[b].SetUp(GetRandomBehaviour());
                    Agents[b].DisableAgent();
                    Agents[b].bisSpawned = true;

                    Agents[b].SpawnLocation = SpawnLocation.transform.position;
                    Agents[b].SpawnRotation = SpawnLocation.transform.rotation;

                    SpawnLocation.bIsUsed = true;
                }
            }
        }
    }

    public void DestroyTanks()
    {
        TankHealth[] WorldTanks = GameObject.FindObjectsOfType<TankHealth>();

        foreach (TankHealth T in WorldTanks)
        {
            Destroy(T.gameObject);
        }
    }

    public void ClearMines()
    {
        MineManager[] _Mines = GameObject.FindObjectsOfType<MineManager>();
        foreach (MineManager M in _Mines)
        {
            Destroy(M.gameObject);
        }
    }

    public void ClearShells()
    {
        ShellManager[] _Shells = GameObject.FindObjectsOfType<ShellManager>();
        foreach (ShellManager S in _Shells)
        {
            Destroy(S.gameObject);
        }
    }

    public void ResetTanks()
    {
        foreach (AgentManager A in Agents)
        {
            A.ObjectReference = null;
            A.GameReset();
        }

        foreach (TankManager T in Tanks)
        {
            T.ObjectReference = null;
            T.GameReset();
        }

        UsedBehaviours.Clear();

    }

    private Behaviour GetRandomBehaviour()
    {
        int RNum = Random.Range(0, AgentBehaviours.Count);

        if (!UsedBehaviours.Contains(AgentBehaviours[RNum].BehaviourName))
        {
            UsedBehaviours.Add(AgentBehaviours[RNum].BehaviourName);
            return AgentBehaviours[RNum];
        }
        else
        {
            while (UsedBehaviours.Contains(AgentBehaviours[RNum].BehaviourName))
            {
                RNum = Random.Range(0, AgentBehaviours.Count);
                if (!UsedBehaviours.Contains(AgentBehaviours[RNum].BehaviourName))
                {
                    UsedBehaviours.Add(AgentBehaviours[RNum].BehaviourName);
                    return AgentBehaviours[RNum];
                }

                if (UsedBehaviours.Count == AgentBehaviours.Count) break;
            }
        }

        return new Behaviour(2.0f, 15.0f, 0.9f, 1.4f, 1.0f); //Return Default Agent
    }

    private void SpawnRandomMine()
    {
        float RandomX = Random.Range(-15, 15);
        float RandomZ = Random.Range(-15, 15);

        Vector3 SpawnLocation = new Vector3(RandomX, 25.0f, RandomZ);

        Instantiate(Mine_Prefab, SpawnLocation, new Quaternion(0.0f, 0.0f, 0.0f, 0.0f));
    }
}
