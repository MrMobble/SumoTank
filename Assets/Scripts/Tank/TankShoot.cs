using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

//This class is mostly copied from the Tank example code.
public class TankShoot : MonoBehaviour
{

    public Rigidbody ShellActor;
    public Transform SpawnLocation;
    public LineRenderer LineRend;

    public float MinLaunchForce;
    public float MaxLaunchForce;
    public float MaxChargeTime;

    private float CurrentLaunchForce;
    private float ChargeSpeed;
    private bool bFired;

    public int PlayerNum;

    private void OnEnable()
    {
        // When the tank is turned on, reset the launch force and the UI
        CurrentLaunchForce = MinLaunchForce;
    }

    // Use this for initialization
    private void Start ()
    {
        ChargeSpeed = (MaxLaunchForce - MinLaunchForce) / MaxChargeTime;
    }
	
	// Update is called once per frame
	void Update ()
    {
        Vector3[] Points = new Vector3[10];
        LineRend.SetPositions(Points);

        // If the max force has been exceeded and the shell hasn't yet been launched...
        if (CurrentLaunchForce >= MaxLaunchForce && !bFired)
        {
            // ... use the max force and launch the shell.
            CurrentLaunchForce = MaxLaunchForce;
            Fire();
        }

        // Otherwise, if the fire button has just started being pressed...
        else if (Input.GetButtonDown("Fire" + PlayerNum))
        {
            // ... reset the fired flag and reset the launch force.
            bFired = false;
            CurrentLaunchForce = MinLaunchForce;

        }
        // Otherwise, if the fire button is being held and the shell hasn't been launched yet...
        else if (Input.GetButton("Fire" + PlayerNum) && !bFired)
        {
            // Increment the launch force and update the slider.
            CurrentLaunchForce += ChargeSpeed * Time.deltaTime;

            Vector3 LanuchForce = CurrentLaunchForce * SpawnLocation.forward;

            Points = TrajectoryFunctions.GetTrajectoryPath(LanuchForce, SpawnLocation.position, 10,
                     TrajectoryFunctions.TimeToReachTarget(LanuchForce, 15.0f, SpawnLocation.position.y, 0.0f));

            LineRend.SetPositions(Points);
        }
        // Otherwise, if the fire button is released and the shell hasn't been launched yet...
        else if (Input.GetButtonUp("Fire" + PlayerNum) && !bFired)
        {
            // ... launch the shell.
            Fire();
        }
    }

    private void Fire()
    {
        // Set the fired flag so only Fire is only called once.
        bFired = true;

        // Create an instance of the shell and store a reference to it's rigidbody.
        Rigidbody ShellInstance = Instantiate(ShellActor, SpawnLocation.position, SpawnLocation.rotation) as Rigidbody;

        // Set the shell's velocity to the launch force in the fire position's forward direction.
        ShellInstance.velocity = CurrentLaunchForce * SpawnLocation.forward;

        // Reset the launch force.  This is a precaution in case of missing button events.
        CurrentLaunchForce = MinLaunchForce;
    }
}
