using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SaveService : MonoBehaviour
{
    private void Awake()
    {
        Services.Register<SaveService>(this);
    }
}
