using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StateControl : TocaFunction
{
    public LayerControl LayerControl;
    public FindControl FindControl;

    public virtual void ForceEnd(bool resetLayer = false)
    {
        //transform.eulerAngles = Vector3.zero;

        if (FindControl && FindControl.CurrentAttachment) //&& FindControl.CurrentAttachment.MyBaseAttributes.HaveCover)
        {
            FindControl.CurrentAttachment.RecalculateSnapPos(FindControl);
            FindControl.Arrived = false;
        }

        if (LayerControl && resetLayer)
            LayerControl.DetouchCallback();
    }

    public void RegisterStateEvent()
    {
        TouchControl tc = (TouchControl)TocaObject.GetTocaFunction<TouchControl>();
        if (tc)
        {
            tc.TouchCallBacks.Add(OnSelection);
            tc.DeTouchCallBacks.Add(OnDeselect);
        }

        LayerControl = (LayerControl)TocaObject.GetTocaFunction<LayerControl>();
        FindControl = (FindControl)TocaObject.GetTocaFunction<FindControl>();
    }

    // handle the state change of objects such as book
    // when the object is on selection
    public virtual void OnSelection()
    {
    }

    // when the object is put down
    public virtual void OnDeselect()
    {
    }
}
