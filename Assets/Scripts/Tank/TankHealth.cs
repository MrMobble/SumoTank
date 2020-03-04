using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TankHealth : MonoBehaviour {

    private float Vulnerability;

    // Use this for initialization
    private void Start ()
    {
        Vulnerability = 0;
	}

    public void AddVulnerability()
    {
        Vulnerability++;
    }

    public float GetVulnerability()
    {
        return Vulnerability;
    }

    public void ResetVulnerability()
    {
        Vulnerability = 0;
    }
	

}
