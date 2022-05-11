using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class StandControl : StateControl
{
    public float TargetDownAngle = 90;
    public float DownSign = 1;
    private float StandReferenceAngle;

    public BaseControl BaseControl;
    public List<int> StandingParams;

    private float HeightFix;

    private void Awake()
    {
        StandReferenceAngle = 360 - TargetDownAngle;
    }

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
        if (DownSign > 0)
        {
            while (GlobalParameter.ClampAngle(transform.eulerAngles.z) < StandReferenceAngle)
            {
                transform.eulerAngles -= DownSign * Vector3.forward * Time.deltaTime * 720;
                yield return null;
            }
        }
        else
        {
            while (GlobalParameter.ClampAngle(transform.eulerAngles.z) > StandReferenceAngle)
            {
                transform.eulerAngles -= DownSign * Vector3.forward * Time.deltaTime * 720;
                yield return null;
            }
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
        if (DownSign > 0)
        {
            while (GlobalParameter.ClampAngle(transform.eulerAngles.z) < TargetDownAngle)
            {
                transform.eulerAngles += DownSign * Vector3.forward * Time.deltaTime * 720;
                yield return null;
            }
        }
        else
        {
            if (transform.eulerAngles.z == 0)
                transform.eulerAngles = new Vector3(0, 0, -1);
            while (GlobalParameter.ClampAngle(transform.eulerAngles.z) > TargetDownAngle)
            {
                transform.eulerAngles += DownSign * Vector3.forward * Time.deltaTime * 720;
                yield return null;
            }
        }
        
        transform.eulerAngles = Vector3.forward * TargetDownAngle;

        //transform.position -= Vector3.up * HeightFix;
        yield return null;

        while (!FindControl.Arrived)
            yield return null;

        ForceEnd(true);

        if (BaseControl)
            BaseControl.gameObject.SetActive(true);
    }
}
