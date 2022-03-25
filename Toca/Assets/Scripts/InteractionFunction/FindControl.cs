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
    private MoveControl MoveControl;
    private ArmControl NearByArm;

    private void Start()
    {
        MoveControl = (MoveControl)TocaObject.GetTocaFunction<MoveControl>();
    }

    private void Update()
    {
        // used to notific nearby hand to reach this toca object
        // must be selected to reach object
        if (MoveControl.Selected)
        {
            TocaObject p = TocaObject.AttachedObject();
            if (!p)
            {
                // only detect hand when it's selected and no attaching parent
                Transform hand = FindBase(true);
                if (hand)
                {
                    Destroy(hand.gameObject);
                    hand = hand.parent;

                    NearByArm = hand.parent.parent.GetComponent<ArmControl>();
                }

            }
        }
    }

    public Transform FindBase(bool findHand = false)
    {
        // when selection is over, when finger leaves the touch screen
        // first try to find the basecontrol to snap to
        // if can snap
        // do all of it attach object transform to become a child
        // static object can be attached, because it doesn't move
        // 

        BaseControl baseControl = null;

        // try to find base control through interaction radius
        Collider2D[] colliders = Physics2D.OverlapCircleAll(transform.position, InteractionRadius, 1 << LayerMask.NameToLayer("Base"));
        foreach (Collider2D collider in colliders)
        {
            baseControl = collider.GetComponentInParent<BaseControl>();
            if (BaseConditionCheck(baseControl) && (!findHand || baseControl.IsHand))
                break;
            else
                baseControl = null;
        }

        if (!baseControl)
        {
            // do a ray cast toward the bottom to detect ground
            RaycastHit2D[] hits = Physics2D.RaycastAll(transform.position, -Vector2.up, 9999, 1 << LayerMask.NameToLayer("Base"));
            foreach (RaycastHit2D hit in hits)
            {
                baseControl = hit.collider.GetComponentInParent<BaseControl>();
                if (BaseConditionCheck(baseControl) && (!findHand || baseControl.IsHand))
                    break;
                else
                    baseControl = null;
            }
        }

        if (!baseControl)
        {
            // do a ray cast toward the bottom to detect ground
            RaycastHit2D[] hits = Physics2D.RaycastAll(transform.position, Vector2.up, 9999, 1 << LayerMask.NameToLayer("Base"));
            foreach (RaycastHit2D hit in hits)
            {
                baseControl = hit.collider.GetComponentInParent<BaseControl>();
                if (BaseConditionCheck(baseControl) && (!findHand || baseControl.IsHand))
                    break;
                else
                    baseControl = null;
            }
        }

        if (!baseControl)
        {
            Debug.LogError("Critical Error, should not happen");
            return null;
        }

        return baseControl.FindSnapPosition(transform.position, IsHuman, GetComponent<Collider2D>().bounds);
    }

    private void OnDrawGizmos()
    {
        //Gizmos.DrawSphere(transform.position, InteractionRadius);
    }

    private bool BaseConditionCheck(BaseControl baseControl)
    {
        bool selfCondition = baseControl && baseControl.TocaObject != TocaObject && ((baseControl.SnapWithHuman && IsHuman) || (baseControl.SnapWithProp && !IsHuman));

        return selfCondition && baseControl.CanbeSnapped(this);
    }
}


