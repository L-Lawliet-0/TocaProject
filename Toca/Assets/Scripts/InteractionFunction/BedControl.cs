using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BedControl : TocaFunction
{
    private BaseControl BaseControl;
    private SpineControl SpineControl;
    private void Start()
    {
        BaseControl = (BaseControl)TocaObject.GetTocaFunction<BaseControl>();
    }

    private void Update()
    {
        if (SpineControl && BaseControl.Attachments.Count < 1)
        {
            SpineControl.WakeUp();
            SpineControl = null;
        }
        else if (!SpineControl && BaseControl.Attachments.Count > 0)
        {
            List<FindControl> finds = new List<FindControl>(BaseControl.Attachments.Keys);
            FindControl find = finds[0];
            SpineControl = (SpineControl)find.TocaObject.GetTocaFunction<SpineControl>();
            SpineControl.Sleep(transform.position + Vector3.up * 5);
        }
    }

}
