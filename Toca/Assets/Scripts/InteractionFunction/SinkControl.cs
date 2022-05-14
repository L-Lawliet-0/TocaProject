using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SinkControl : MonoBehaviour
{
    public GameObject ActiveAlways;
    public GameObject ObjectTurn;

    private bool On;
    void Start()
    {
        On = false;
        TouchControl tc = GetComponent<TouchControl>();
        tc.ClickCallBacks.Add(Turn);
    }

    public void Turn()
    {
        On = !On;
        ActiveAlways.SetActive(true);
        ObjectTurn.SetActive(On);
    }
}
