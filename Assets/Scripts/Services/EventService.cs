using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EventService : MonoBehaviour
{

    public Action <Card> CardClick;
    public Action  hitClick;
    private void Awake()
    {
        Services.Register<EventService>(this);
    }
    
}
