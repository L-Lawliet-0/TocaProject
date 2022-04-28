using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HandPivotControl : TocaFunction
{
    private FindControl Find;
    private bool InHand;
    // this function is used to fix position and rotation of hand object
    private void Start()
    {
        Find = (FindControl)TocaObject.GetTocaFunction<FindControl>();
        InHand = false;
    }

    private void Update()
    {
        if (!InHand && Find.CurrentAttachment && (Find.CurrentAttachment.MyBaseAttributes.IsLeftHand || Find.CurrentAttachment.MyBaseAttributes.IsRightHand))
        {
            SetInHand();
        }
        else if (InHand && !Find.CurrentAttachment)
        {
            SetOut();
        }

        if (InHand && Find.CurrentAttachment && Find.Arrived)
        {
            if (Find.CurrentAttachment.MyBaseAttributes.IsLeftHand)
                TocaObject.transform.position = Find.CurrentAttachment.transform.position - transform.localPosition;
            else
                TocaObject.transform.position = Find.CurrentAttachment.transform.position + new Vector3(transform.localPosition.x, -transform.localPosition.y);
        }
    }

    private void SetInHand()
    {
        InHand = true;
        TocaObject.GetComponent<SpriteRenderer>().flipX = Find.CurrentAttachment.MyBaseAttributes.IsRightHand;

    }

    private void SetOut()
    {
        InHand = false;
        TocaObject.GetComponent<SpriteRenderer>().flipX = false;
    }
}
