using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioService : MonoBehaviour
{
    private void Awake()
    {
        Services.Register<AudioService>(this);
    }

}
