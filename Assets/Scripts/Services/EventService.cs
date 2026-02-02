using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EventService : MonoBehaviour
{

    public Action <Card> CardClick;
    public Action  HintRequested;
    public Action LevelComplete;
    private void Awake()
    {
        Services.Register<EventService>(this);
    }
    
}
