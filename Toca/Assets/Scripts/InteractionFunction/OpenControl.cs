using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OpenControl : TocaFunction
{
    public bool Opening; // is this object currently opening
    public GameObject OpenObj, CloseObj;
    public BaseControl Basecontrol;

    private void Start()
    {
        // register open and close event on touch control
        TouchControl openTouch = OpenObj.GetComponent<TouchControl>();
        TouchControl closeTouch = CloseObj.GetComponent<TouchControl>();
        openTouch.ClickCallBacks.Add(OnClose);
        closeTouch.ClickCallBacks.Add(OnOpen);

        OnClose(); // close the objects first

        if (OpenObj && OpenObj.GetComponent<SpriteRenderer>())
            OpenObj.GetComponent<SpriteRenderer>().enabled = true;
        if (CloseObj && CloseObj.GetComponent<SpriteRenderer>())
            CloseObj.GetComponent<SpriteRenderer>().enabled = true;
    }

    // when this game object is "opened"
    public void OnOpen()
    {
        Opening = true;

        if (OpenObj)
            OpenObj.SetActive(true);
        if (CloseObj)
            CloseObj.SetActive(false);
        if (Basecontrol)
            Basecontrol.IgnoreLimit = true;
    }

    public void OnClose()
    {
        Opening = false;
        if (OpenObj)
            OpenObj.SetActive(false);
        if (CloseObj)
            CloseObj.SetActive(true);
        if (Basecontrol)
            Basecontrol.IgnoreLimit = false;
    }
}
