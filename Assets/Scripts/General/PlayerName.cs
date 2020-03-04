using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerName : MonoBehaviour {

    NameManager _NameManager = NameManager.Instance;

    public string Name;
    public TextMesh TextMesh;
    public bool RandomNames;

    private string CurrentName;

    // Use this for initialization
    void Start ()
    {
        SetPlayerName(Name, RandomNames);
    }

    public void SetPlayerName(string _PlayerName, bool _Random)
    {
        if (_Random)
        {
            string _Name = _NameManager.GetUsableName();
            TextMesh.text = _Name;
            CurrentName = _Name;
        }
        else
        {
            TextMesh.text = _PlayerName;
            CurrentName = _PlayerName;
        }

        TextMesh.color = new Color(0.8f, 0.8f, 0.8f);
        TextMesh.fontStyle = FontStyle.Bold;
        TextMesh.alignment = TextAlignment.Center;
        TextMesh.anchor = TextAnchor.MiddleCenter;
        TextMesh.fontSize = 28;
    }

    public string GetCurrentName()
    {
        return CurrentName;
    }

    public void DisablePlayerName()
    {
        TextMesh.text = "";
    }
	
	// Update is called once per frame
	void Update ()
    {
        TextMesh.transform.rotation = Camera.main.transform.rotation;
        TankHealth TH = GetComponent<TankHealth>();
        TextMesh.color = new Color(0.95f, 0.95f - TH.GetVulnerability() * 0.05f, 0.95f - TH.GetVulnerability() * 0.05f);

    }
}
