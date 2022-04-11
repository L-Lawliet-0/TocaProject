using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StateControl : TocaFunction
{
    public void RegisterStateEvent()
    {
        TouchControl tc = (TouchControl)TocaObject.GetTocaFunction<TouchControl>();
        if (tc)
        {
            tc.TouchCallBacks.Add(OnSelection);
            tc.DeTouchCallBacks.Add(OnDeselect);
        }
    }

    // handle the state change of objects such as book
    // when the object is on selection
    public virtual void OnSelection()
    {
        Debug.LogError("Fire in parent");
    }

    // when the object is put down
    public virtual void OnDeselect()
    {
        Debug.LogError("Fire in parent");
    }
}
