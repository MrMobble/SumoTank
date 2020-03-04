using System;
using UnityEngine;

[Serializable]
public class AgentManager
{
    private Agent AgentScript;
    public GameObject ObjectReference;

    [HideInInspector] public Vector3 SpawnLocation;
    [HideInInspector] public Quaternion SpawnRotation;
    [HideInInspector] public bool bisSpawned = false;

    // Use this for initialization
    public void SetUp(Behaviour _Behaviour)
    {
        AgentScript = ObjectReference.GetComponent<Agent>();
        AgentScript.TankBehaviour = _Behaviour;

        // Get all of the renderers of the tank.
        MeshRenderer[] Renderers = ObjectReference.GetComponentsInChildren<MeshRenderer>();

        // Go through all the renderers...
        for (int i = 0; i < Renderers.Length; i++)
        {
            // ... set their material color to the color specific to this tank.
            if (Renderers[i].name != "MeshText")
            {
                Renderers[i].material.color = _Behaviour.TankColour;
            }
        }
    }

    public void EnableAgent()
    {
        AgentScript.enabled = true;
    }

    public void DisableAgent()
    {
        AgentScript.enabled = false;
    }

    public void RoundReset()
    {
        ObjectReference.GetComponent<Rigidbody>().velocity = new Vector3(0.0f, 0.0f, 0.0f);
        AgentScript.Turret.transform.rotation = new Quaternion(0.0f, 0.0f, 0.0f, 0.0f);
        ObjectReference.transform.position = SpawnLocation;
        ObjectReference.transform.rotation = SpawnRotation;
        ObjectReference.GetComponent<TankHealth>().ResetVulnerability();
        ObjectReference.SetActive(true);
    }

    public void GameReset()
    {
        bisSpawned = false;
    }

}

