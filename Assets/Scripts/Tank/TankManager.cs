using System;
using UnityEngine;

[Serializable]
public class TankManager
{
    public GameObject ObjectReference;


    public Material PlayerMaterial;

    private TankMovement MovementScript;
    private TankShoot ShootScript;
    private PlayerName NameScript;
    private LineRenderer LineRender;

    [HideInInspector] public Vector3 SpawnLocation;
    [HideInInspector] public Quaternion SpawnRotation;
    [HideInInspector] public bool bisSpawned = false;

    public int PlayerNum;

    // Use this for initialization
    public void SetUp()
    {
        MovementScript = ObjectReference.GetComponent<TankMovement>();
        ShootScript = ObjectReference.GetComponent<TankShoot>();
        NameScript = ObjectReference.GetComponent<PlayerName>();
        LineRender = ObjectReference.GetComponent<LineRenderer>();

        MovementScript.PlayerNum = PlayerNum;
        ShootScript.PlayerNum = PlayerNum;
        NameScript.Name = NameScript.Name + " " + PlayerNum;

        LineRender.material = PlayerMaterial;

        // Get all of the renderers of the tank.
        MeshRenderer[] Renderers = ObjectReference.GetComponentsInChildren<MeshRenderer>();

        // Go through all the renderers...
        for (int i = 0; i < Renderers.Length; i++)
        {
            // ... set their material color to the color specific to this tank.
            if (Renderers[i].name != "MeshText")
            {
                Renderers[i].material = PlayerMaterial;
            }
            
        }
    }

    public void EnableAgent()
    {
        MovementScript.enabled = true;
        ShootScript.enabled = true;
    }

    public void DisableAgent()
    {
        MovementScript.enabled = false;
        ShootScript.enabled = false;
    }

    public void RoundReset()
    {
        ObjectReference.transform.position = SpawnLocation;
        ObjectReference.transform.rotation = SpawnRotation;
        ObjectReference.GetComponent<Rigidbody>().velocity = new Vector3(0.0f, 0.0f, 0.0f);
        ObjectReference.GetComponent<TankHealth>().ResetVulnerability();
        ObjectReference.SetActive(true);
    }

    public void GameReset()
    {
        bisSpawned = false;
    }

}
