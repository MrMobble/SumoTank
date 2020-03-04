using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TankMovement : MonoBehaviour {

    Rigidbody Body;
    public Component Turret;
    [SerializeField, Range(0, 500)] float MovementSpeed;
    [SerializeField, Range(0, 500)] float TurningSpeed;
    [SerializeField, Range(0, 500)] float TurretSpeed;
    public Transform CenterOfMass;

    private PlayerScore ScoreScript;

    public int PlayerNum;

    // Use this for initialization
    void Start ()
    {
        Body = GetComponent<Rigidbody>();
        Body.centerOfMass = CenterOfMass.localPosition;

        ScoreScript = GetComponent<PlayerScore>();
    }
	
	// Update is called once per frame
	void Update ()
    {
        if (transform.position.y < -1)
        {
            AddScore();

            gameObject.SetActive(false);
        }

        var Horizontal = Input.GetAxis("Horizontal" + PlayerNum);
        var Vertial = Input.GetAxis("Vertical" + PlayerNum);

        var Turret_Value = Input.GetAxis("Turret_Value" + PlayerNum);

        Body.AddForce((transform.forward * Horizontal * MovementSpeed) * Time.deltaTime);
        Body.AddTorque((transform.up * Vertial * TurningSpeed) * Time.deltaTime);

        Turret.transform.Rotate(Vector3.up * Turret_Value * TurretSpeed);
        
    }

    public void AddScore()
    {
        PlayerName[] Players = GameObject.FindObjectsOfType<PlayerName>();

        ScoreScript.Score += Mathf.Clamp(4 - Players.Length, 0, 4);

        ScoreManager.Instance.AddScore(ScoreScript.Score, GetComponent<PlayerName>().GetCurrentName());
    }
}
