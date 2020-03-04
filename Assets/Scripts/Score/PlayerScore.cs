using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerScore : MonoBehaviour
{
    private int _PlayerScore;

    public int Score
    {
        get { return _PlayerScore; }

        set { _PlayerScore = value; }
    }
}
