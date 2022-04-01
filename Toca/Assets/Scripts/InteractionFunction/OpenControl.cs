using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OpenControl : TocaFunction
{
    public bool Opening; // is this object currently opening
    public GameObject OpenObj, CloseObj;

    private void Start()
    {
        // register open and close event on touch control
        TouchControl openTouch = OpenObj.GetComponent<TouchControl>();
        TouchControl closeTouch = CloseObj.GetComponent<TouchControl>();
        openTouch.DeTouchCallBacks.Add(OnClose);
        closeTouch.DeTouchCallBacks.Add(OnOpen);

        OnClose(); // close the objects first
    }

    // when this game object is "opened"
    public void OnOpen()
    {
        Opening = true;

        if (OpenObj)
            OpenObj.SetActive(true);
        if (CloseObj)
            CloseObj.SetActive(false);
    }

    public void OnClose()
    {
        Opening = false;
        if (OpenObj)
            OpenObj.SetActive(false);
        if (CloseObj)
            CloseObj.SetActive(true);
    }
}
