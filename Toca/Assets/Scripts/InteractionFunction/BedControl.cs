using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BedControl : TocaFunction
{
    private BaseControl BaseControl;
    private SpineControl SpineControl;
    public Vector3 TargetAngle; // this is the angle that will perform
    private Quaternion m_TargetAngle;
    private void Awake()
    {
        m_TargetAngle = Quaternion.Euler(TargetAngle);
    }

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

            SpineControl.GetComponent<MeshRenderer>().sortingOrder = ((LayerControl)BaseControl.TocaObject.GetTocaFunction<LayerControl>()).OrderValue;


            StartCoroutine("Rotate");
        }
    }

    private IEnumerator Rotate()
    {
        FindControl fc = (FindControl)SpineControl.TocaObject.GetTocaFunction<FindControl>();
        while (!fc.Arrived)
            yield return null;
        while (SpineControl)
        {
            SpineControl.TocaObject.transform.rotation = Quaternion.RotateTowards(SpineControl.TocaObject.transform.rotation, m_TargetAngle, 360 * Time.deltaTime);
            if (Quaternion.Angle(SpineControl.TocaObject.transform.rotation, m_TargetAngle) < .1f)
            {
                SpineControl.Sleep(transform.position + Vector3.up * 1.5f + Vector3.right * 1.5f);
                break;
            }

            yield return null;
        }
    }

}
