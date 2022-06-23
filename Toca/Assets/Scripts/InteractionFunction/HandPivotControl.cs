using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HandPivotControl : TocaFunction
{
    private FindControl Find;
    private MoveControl Move;
    private bool InHand;
    private float myTimer;
    // this function is used to fix position and rotation of hand object
    private void Start()
    {
        Find = (FindControl)TocaObject.GetTocaFunction<FindControl>();
        Move = (MoveControl)TocaObject.GetTocaFunction<MoveControl>();
        InHand = false;
    }

    private void OnEnable()
    {
        if (InHand)
            SetInHand();
        else
            SetOut();
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

        if (InHand && myTimer > 0)
        {
            Find.CurrentAttachment.Attachments[Find].Timer = 2;
            Debug.LogError(transform.localScale.x);
            if (transform.localScale.x > 0)
                Move.UpdateTargetPosition(Find.CurrentAttachment.transform.position - Find.TocaObject.Bottom.localPosition);
            else
                Move.UpdateTargetPosition(Find.CurrentAttachment.transform.position - new Vector3(-Find.TocaObject.Bottom.localPosition.x, Find.TocaObject.Bottom.localPosition.y));
            myTimer -= Time.deltaTime;
        }
    }

    private void SetInHand()
    {
        InHand = true;
        if (Find.CurrentAttachment.MyBaseAttributes.IsRightHand)
            transform.localScale = new Vector3(Mathf.Abs(transform.localScale.x), transform.localScale.y, transform.localScale.z);
        else if (Find.CurrentAttachment.MyBaseAttributes.IsLeftHand)
            transform.localScale = new Vector3(-Mathf.Abs(transform.localScale.x), transform.localScale.y, transform.localScale.z);

        myTimer = 1; // update position for 1 seconds
    }

    private void SetOut()
    {
        InHand = false;
        TocaObject.GetComponent<SpriteRenderer>().flipX = false;
        transform.localScale = new Vector3(Mathf.Abs(transform.localScale.x), transform.localScale.y, transform.localScale.z);
    }
}
