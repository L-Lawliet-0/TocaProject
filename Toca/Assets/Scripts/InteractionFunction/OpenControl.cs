using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OpenControl : TocaFunction
{
    public bool Opening; // is this object currently opening
    public GameObject OpenObj, CloseObj;
    public BaseControl Basecontrol;

    public delegate void VoidDelegate();
    public event VoidDelegate OnOpening, OnClosing;

    public int SFXopenindex = -1;
    public int SFXcloseindex = -1;

    private void Start()
    {
        // register open and close event on touch control
        TouchControl openTouch = OpenObj.GetComponent<TouchControl>();
        TouchControl closeTouch = CloseObj.GetComponent<TouchControl>();
        openTouch.ClickCallBacks.Add(OnClose);
        closeTouch.ClickCallBacks.Add(OnOpen);
        
        if (OpenObj && OpenObj.GetComponent<SpriteRenderer>() && !TocaObject.GetTocaFunction<OpenAnimationControl>())
            OpenObj.GetComponent<SpriteRenderer>().enabled = true;
        if (CloseObj && CloseObj.GetComponent<SpriteRenderer>() && !TocaObject.GetTocaFunction<OpenAnimationControl>())
            CloseObj.GetComponent<SpriteRenderer>().enabled = true;

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
        if (Basecontrol)
            Basecontrol.IgnoreLimit = true;
        OnOpening?.Invoke();

        if (SFXopenindex != -1)
            SoundManager.Instance.PlaySFX(SFXopenindex, true, TocaObject.transform.position);
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
        OnClosing?.Invoke();

        if (SFXcloseindex != -1)
            SoundManager.Instance.PlaySFX(SFXcloseindex, true, TocaObject.transform.position);
    }
}
