using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class StandControl : StateControl
{
    private FindControl FindControl;
    private LayerControl LayerControl;
    private BaseControl BaseControl;
    public List<int> StandingParams;

    private void Start()
    {
        base.RegisterStateEvent();
        FindControl = (FindControl)TocaObject.GetTocaFunction<FindControl>();
        BaseControl = (BaseControl)TocaObject.GetTocaFunction<BaseControl>();
        LayerControl = (LayerControl)TocaObject.GetTocaFunction<LayerControl>();
    }

    private void Update()
    {

    }

    public override void OnSelection()
    {
        // object should be standing
        if (!BaseControl || BaseControl.Attachments.Count < 1)
        {
            StartCoroutine("StandUp");
        }
    }

    public override void OnDeselect()
    {
        StopAllCoroutines();
        if (ShouldObjectStand())
        {
            StartCoroutine("StandUp");
        }
        else
        {
            StartCoroutine("StandDown");
        }
    }

    private bool ShouldObjectStand()
    {
        if (BaseControl && BaseControl.Attachments.Count > 0)
            return false;

        if (FindControl.CurrentAttachment)
        {
            bool val = false;
            foreach (int index in StandingParams)
            {
                val |= FindControl.CurrentAttachment.MyBaseAttributes.Parameters[index];
            }
            return val;
        }

        return false;
    }

    private IEnumerator StandUp()
    {
        while (GlobalParameter.ClampAngle(transform.eulerAngles.z) < 270)
        {
            transform.eulerAngles -= Vector3.forward * Time.deltaTime * 360;
            yield return null;
        }
        transform.eulerAngles = Vector3.zero;

        if (FindControl && FindControl.CurrentAttachment)
        {
            FindControl.CurrentAttachment.RecalculateSnapPos(FindControl);
            FindControl.Arrived = false;
        }
    }

    private IEnumerator StandDown()
    {
        while (GlobalParameter.ClampAngle(transform.eulerAngles.z) < 90)
        {
            transform.eulerAngles += Vector3.forward * Time.deltaTime * 360;
            yield return null;
        }
        transform.eulerAngles = Vector3.forward * 90;

        if (FindControl && FindControl.CurrentAttachment)
        {
            FindControl.CurrentAttachment.RecalculateSnapPos(FindControl);
            FindControl.Arrived = false;
        }

        if (LayerControl)
            LayerControl.DetouchCallback();
    }
}
