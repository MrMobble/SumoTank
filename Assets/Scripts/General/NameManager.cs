using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NameManager : MonoBehaviour
{

    private static NameManager _Instance;

    public static NameManager Instance {  get { return _Instance; } }

    private void Awake()
    {
        if (_Instance != null && _Instance != this)
        {
            Destroy(this.gameObject);
        }
        else
        {
            _Instance = this;
            InitNames();
        }
    }

    public List<string> PossibleNames;

    [SerializeField] List<string> UseableNames;

    public void InitNames()
    {
        foreach (string N in PossibleNames)
        {
            UseableNames.Add(N);
        }
    }

    public string GetUsableName()
    {
        string Name = UseableNames[0];
        UseableNames.RemoveAt(0);

        return Name;
    }
}
