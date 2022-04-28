using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HorseShakeControl : TocaFunction
{
    private FindControl Target;
    private BaseControl BaseControl;
    private bool Shaking;
    private void Start()
    {
        BaseControl = (BaseControl)TocaObject.GetTocaFunction<BaseControl>();
        Shaking = false;
    }

    private void Update()
    {
        if (!Shaking && BaseControl.Attachments.Count > 0)
        {
            StopAllCoroutines();
            StartCoroutine("Shake");
            Shaking = true;
        }
        else if (Shaking && BaseControl.Attachments.Count < 1)
        {
            StopAllCoroutines();
            StartCoroutine("StopShake");
            Shaking = false;
        }
    }

    private IEnumerator Shake()
    {
        Target = new List<FindControl>(BaseControl.Attachments.Keys)[0];
        float startAngle = 0;
        float limit = 5; // test value 10 degree
        float sign = 1;
        float speed = 10;
        while (true)
        {
            startAngle += sign * speed * Time.deltaTime;
            if (sign > 0 && startAngle > limit)
            {
                sign = -sign;
                startAngle = limit;
            }
            else if (sign < 0 && startAngle < -limit)
            {
                sign = -sign;
                startAngle = -limit;
            }
            transform.eulerAngles = Vector3.forward * startAngle;

            yield return null;
        }
    }

    private IEnumerator StopShake()
    {
        yield return null;
        transform.eulerAngles = Vector3.zero;
        if (Target)
        {
            Target.TocaObject.transform.eulerAngles = Vector3.zero;
        }
        Target = null;
    }
}
