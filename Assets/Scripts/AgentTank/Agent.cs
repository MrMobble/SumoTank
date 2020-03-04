using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Agent : MonoBehaviour
{
    private Rigidbody Body;

    [SerializeField] Transform Target = null;
    private Vector3 TargetLocation = new Vector3(0.0f, 0.0f, 0.0f);

    public Component Turret;
    public Rigidbody ShellActor;
    public Transform MuzzleLocation;
    public LayerMask StageMask;

    public Transform CenterOfMass;

    //Behaviour Variables
    private Behaviour AgentBehaviour;
    private PlayerScore ScoreScript;

    public Behaviour TankBehaviour
    {
        set { AgentBehaviour = value; }
    }

    private bool bFire;
    private float FrameCount;

    [SerializeField, Range(0, 500)] float TankSpeed;
    [SerializeField, Range(0, 500)] float TurnSpeed;

    private IEnumerator CurrentState;
    private IEnumerator NextState;

    // Use this for initialization
    void Awake()
    {
        Body = GetComponent<Rigidbody>();
        Body.centerOfMass = CenterOfMass.localPosition;

        ScoreScript = GetComponent<PlayerScore>();

        CurrentState = Move();
        StartCoroutine(StateMachine());
    }

    private void OnEnable()
    {
        Target = null;

        NextState = Move();
        StartCoroutine(StateMachine());
    }

    private void OnDisable()
    {
        Target = null;
        Transition(Disabled(), "Agent Disabled");
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

    IEnumerator Move()
    {
        while (NextState == null)
        {
            AgentChecks();
               
            if (Target == null) continue; //If AI has no target then skip.

            float Dir = AgentFunctions.AngleDir(transform.up, (transform.position - TargetLocation), transform.forward);


            //Because I am using torque and not calculating a direction force, everything is simplified.
            Body.AddForce((transform.forward * TankSpeed) * Time.deltaTime);
            Body.AddTorque((transform.up * TurnSpeed * Dir) * Time.deltaTime);

            //Finds the turret rotation ignores the y rotation.
            Quaternion TargetRotation = AgentFunctions.GetTargetRotation(Target, transform);
            Turret.transform.rotation = Quaternion.Lerp(Turret.transform.rotation, TargetRotation, AgentBehaviour.RotationSpeed * Time.deltaTime);

            //FramesCount uses Time.deltatime so that it works with TimeScale
            FrameCount -= Time.deltaTime;
            if (FrameCount <= 0) bFire = false;

            //If turret is in specified range, change to shoot state.
            if (AgentFunctions.GetTargetAngle(Turret, Target, transform, AgentBehaviour.RotationAccuracy) && !bFire)
            {
                Transition(Shoot(), "Target In Range");
            }

            yield return null;
        }
    }

    //Tank Shoot
    IEnumerator Shoot()
    {
        bFire = true;
        FrameCount = AgentBehaviour.ReloadTime;

        if (Target == null) Transition(Move(), "No Target Return To Move");

        // Create an instance of the shell and store a reference to it's rigidbody.
        Rigidbody ShellInstance = Instantiate(ShellActor, MuzzleLocation.position, MuzzleLocation.rotation) as Rigidbody;

        // Set the shell's velocity to the launch force in the fire position's forward direction.
        // I divide by a random value to simulate it's accuracy
        if (ShellInstance != null && Target != null) ShellInstance.velocity = AgentFunctions.BallisticVel(transform, Target, 25.0f) / Random.Range(AgentBehaviour.MinDistAccuracy, AgentBehaviour.MaxDistAccuracy);

        Transition(Move(), "Fired At Target");
        
        yield return null;
    }

    //Finds new target for tank to shoot at.
    IEnumerator FindTarget()
    {
        Target = null;

        while (NextState == null)
        {
            //Get all tanks in the scene.
            TankHealth[] Tanks = GameObject.FindObjectsOfType<TankHealth>();

            //Set random tank from array to Target.
            int RandNum = Random.Range(0, Tanks.Length);
            if (Tanks[RandNum].gameObject != gameObject && Tanks[RandNum].gameObject.activeInHierarchy) Target = Tanks[RandNum].transform;

            //If the target is not self return to main state.
            if (Target != null && Target.gameObject != gameObject)
            {
                Transition(Move(), "New Target Found");
            }

            yield return null;
        }
    }

    //Finds new location for tank to move towards.
    IEnumerator FindLocation()
    {
        while (NextState == null)
        {
            float RandomX = Random.Range(-12, 12);
            float RandomZ = Random.Range(-12, 12);

            TargetLocation = new Vector3(RandomX, 1.0f, RandomZ);

            if (Physics.Raycast(TargetLocation + Vector3.up * 5.0f, -Vector3.up, 10.0f, StageMask))
            {
                Transition(Move(), "New Location Found");
            }

            yield return null;
        }
    }

    //Death State.
    IEnumerator Death()
    {
        while (NextState == null)
        {

            AddScore();

            gameObject.SetActive(false);

            yield return null;
        }
    }

    //Disabled Tank State.
    IEnumerator Disabled()
    {
        while (NextState == null)
        {
            yield return null;
        }
    }

    private void Transition(IEnumerator NewState, string Reason)
    {
        NextState = NewState;
        //Debug.Log(name + "   NextState: " + NewState.ToString() + "   Reason: " + Reason); //With too many AI spams to log making it unreadable.
    }

    //All the checks the agent goes though.
    private void AgentChecks()
    {
        if (AgentFunctions.CheckTarget(Target)) Transition(FindTarget(), "No Target");

        if (AgentFunctions.CheckTargetLocation(TargetLocation)) Transition(FindLocation(), "No Target Location");

        if (AgentFunctions.CheckForDeath(transform, -1)) Transition(Death(), "Off Map");

        if (AgentFunctions.CheckDistanceToLocation(transform, TargetLocation, 2.0f)) Transition(FindLocation(), "Arrived At Last Location");
    }

    public void AddScore()
    {
        PlayerName[] Players = GameObject.FindObjectsOfType<PlayerName>();

        ScoreScript.Score += Mathf.Clamp(4 - Players.Length, 0, 4);

        ScoreManager.Instance.AddScore(ScoreScript.Score, GetComponent<PlayerName>().GetCurrentName());
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawCube(GetComponent<Rigidbody>().worldCenterOfMass, new Vector3(1.0f, 1.0f, 1.0f));
    }

    private void OnTriggerStay(Collider other)
    {
        //Very simple Tank avoidance.
        if (other.gameObject.tag == "Tank")
        {
            float Dir = AgentFunctions.AngleDir(transform.up, (transform.position - other.transform.position), transform.forward);

            if (Dir == 1)
            {
                Body.AddTorque(transform.up * TurnSpeed * -0.50f);
                Debug.DrawLine(transform.position, transform.position + transform.right * TurnSpeed * -2);
            }
            else
            {
                Body.AddTorque(transform.up * TurnSpeed * 0.50f);
                Debug.DrawLine(transform.position, transform.position + transform.right * TurnSpeed * 2);
            }
        }

        //Very simple Mine avoidance.
        if (other.gameObject.tag == "Mine")
        {
            float Dir = AgentFunctions.AngleDir(transform.up, (transform.position - other.transform.position), transform.forward);

            if (Dir == 1)
            {
                Body.AddTorque(transform.up * TurnSpeed * -1.0f);
                Debug.DrawLine(transform.position, transform.position + transform.right * TurnSpeed * -2);
            }
            else
            {
                Body.AddTorque(transform.up * TurnSpeed * 1.0f);
                Debug.DrawLine(transform.position, transform.position + transform.right * TurnSpeed * 2);
            }
        }
    }
}
