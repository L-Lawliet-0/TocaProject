using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class StandControl : StateControl
{
    private BaseControl BaseControl;
    public List<int> StandingParams;

    private float HeightFix;

    private void Start()
    {
        base.RegisterStateEvent();
        FindControl = (FindControl)TocaObject.GetTocaFunction<FindControl>();
        BaseControl = (BaseControl)TocaObject.GetTocaFunction<BaseControl>();
        LayerControl = (LayerControl)TocaObject.GetTocaFunction<LayerControl>();
        HeightFix = (FindControl.ObjectHeight - FindControl.ObjectWidth) / 2;

        Invoke("OnDeselect", 2);
    }

    private void Update()
    {

    }

    public override void OnSelection()
    {
        StopAllCoroutines();
        // object should be standing
        if (!BaseControl || BaseControl.Attachments.Count < 1)
        {
            StartCoroutine("StandUp", false);
        }
    }

    public override void OnDeselect()
    {
        StopAllCoroutines();
        if (ShouldObjectStand())
        {
            StartCoroutine("StandUp", true);
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
            val |= FindControl.CurrentAttachment.MyBaseAttributes.IsLeftHand || FindControl.CurrentAttachment.MyBaseAttributes.IsRightHand;
            return val;
        }

        return false;
    }

    private IEnumerator StandUp(bool resetLayer = false)
    {
        
        while (GlobalParameter.ClampAngle(transform.eulerAngles.z) < 270)
        {
            transform.eulerAngles -= Vector3.forward * Time.deltaTime * 720;
            yield return null;
        }
        
        transform.eulerAngles = Vector3.zero;

        //transform.position += Vector3.up * HeightFix;
        yield return null;

        while (!FindControl.Arrived)
            yield return null;

        ForceEnd(resetLayer);

        if (BaseControl)
            BaseControl.gameObject.SetActive(false);
    }

    private IEnumerator StandDown()
    {
        
        while (GlobalParameter.ClampAngle(transform.eulerAngles.z) < 90)
        {
            transform.eulerAngles += Vector3.forward * Time.deltaTime * 720;
            yield return null;
        }
        
        transform.eulerAngles = Vector3.forward * 90;

        //transform.position -= Vector3.up * HeightFix;
        yield return null;

        while (!FindControl.Arrived)
            yield return null;

        ForceEnd(true);

        if (BaseControl)
            BaseControl.gameObject.SetActive(true);
    }
}
