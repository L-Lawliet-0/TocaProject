using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class StandControl : StateControl
{
    private FindControl FindControl;
    public List<BaseControl.BaseType> StandingTypes;

    private void Start()
    {
        base.RegisterStateEvent();
        FindControl = (FindControl)TocaObject.GetTocaFunction<FindControl>();
    }

    public override void OnSelection()
    {
        // object should be standing
        StartCoroutine("StandUp");
    }

    public override void OnDeselect()
    {
        StopAllCoroutines();
        if (ShouldObjectStand())
            StartCoroutine("StandUp");
        else
            StartCoroutine("StandDown");
    }

    private bool ShouldObjectStand()
    {
        if (FindControl.CurrentAttachment)
        {
            return StandingTypes.Contains(FindControl.CurrentAttachment.MyBaseType);
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
    }

    private IEnumerator StandDown()
    {
        while (GlobalParameter.ClampAngle(transform.eulerAngles.z) < 90)
        {
            transform.eulerAngles += Vector3.forward * Time.deltaTime * 360;
            yield return null;
        }
        transform.eulerAngles = Vector3.forward * 90;
    }
}
