using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
 * find control is used to find base to attach to
 * this may seem trivial now but in the feature there's a lot
 * logic needed for what object can snap with the other object
 */
public class FindControl : TocaFunction
{
    public float InteractionRadius;
    public bool IsHuman;
    public BaseControl CurrentAttachment; // the base this object is currently attaching
    public BaseControl BasePreview;
    public bool Arrived;
    private MoveControl MoveControl;
    private ArmControl NearByArm;

    public BaseControl AttachBase;
    public int AttachBaseID;

    private void Start()
    {
        MoveControl = (MoveControl)TocaObject.GetTocaFunction<MoveControl>();
    }

    private void Update()
    {
        
    }

    public void TryAttach()
    {
        Arrived = false;
        // find a good base to attach to
        CurrentAttachment = BasePreview;
        if (!CurrentAttachment)
            CurrentAttachment = FindBaseNearBy();
        if (!CurrentAttachment)
            CurrentAttachment = FindBaseByDirection(Vector2.down);
        if (!CurrentAttachment)
            CurrentAttachment = FindBaseByDirection(Vector2.up);

        Attach(CurrentAttachment);
    }

    public void TryDetach()
    {
        Detach();
    }

    public void Attach(BaseControl bc)
    {
        bc.Attach(this);
    }

    public void Detach()
    {
        if (CurrentAttachment)
            CurrentAttachment.Detach(this);
        CurrentAttachment = null;
    }

    public void SelectedUpdate()
    {
        BasePreview = FindBaseNearBy();
    }

    // return a base near by
    public BaseControl FindBaseNearBy()
    {
        BaseControl bc = null;
        Bounds bounds = GetComponent<Collider2D>().bounds;
        float interactionRange = (bounds.extents.x + bounds.extents.y) * 1.5f;

        Collider2D[] colliders = Physics2D.OverlapCircleAll(transform.position + Vector3.up * bounds.extents.y, interactionRange, 1 << LayerMask.NameToLayer("Base"));
        foreach (Collider2D collider in colliders)
        {
            bc = collider.GetComponentInParent<BaseControl>();
            if (BaseConditionCheck(bc))
                break;
            else
                bc = null;
        }

        return bc;
    }

    public BaseControl FindBaseByDirection(Vector2 direction)
    {
        BaseControl bc = null;
        RaycastHit2D[] hits = Physics2D.RaycastAll(transform.position, direction, 9999, 1 << LayerMask.NameToLayer("Base"));
        foreach (RaycastHit2D hit in hits)
        {
            bc = hit.collider.GetComponentInParent<BaseControl>();
            if (BaseConditionCheck(bc))
                break;
            else
                bc = null;
        }

        return bc;
    }

    private bool BaseConditionCheck(BaseControl baseControl)
    {
        bool selfCondition = baseControl && baseControl.TocaObject != TocaObject && ((baseControl.SnapWithHuman && IsHuman) || (baseControl.SnapWithProp && !IsHuman));

        return selfCondition && baseControl.CanbeSnapped(this);
    }
}


