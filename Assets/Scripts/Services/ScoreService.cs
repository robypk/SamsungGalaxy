using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScoreService : MonoBehaviour
{

    private void Awake()
    {
        Services.Register<ScoreService>(this);  
    }
}
