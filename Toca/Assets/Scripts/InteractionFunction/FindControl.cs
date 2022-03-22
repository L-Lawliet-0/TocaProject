using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
 * find control is used to find base to attach to
 * this may seem trivial now but in the feature there's a lot
 * logic needed for what object can snap with the other object
 */
public class FindControl : MonoBehaviour
{
    public float InteractionRadius;
    public bool IsHuman;
    public Vector3 FindBase(Transform bottom)
    {
        BaseControl baseControl = null;

        // try to find base control through interaction radius
        Collider2D[] colliders = Physics2D.OverlapCircleAll(bottom.position, InteractionRadius, 1 << LayerMask.NameToLayer("Base"));
        foreach (Collider2D collider in colliders)
        {
            baseControl = collider.GetComponentInParent<BaseControl>();
            if (BaseConditionCheck(baseControl, bottom))
                break;
            else
                baseControl = null;
        }

        if (!baseControl)
        {
            // do a ray cast toward the bottom to detect ground
            RaycastHit2D[] hits = Physics2D.RaycastAll(bottom.position, -Vector2.up, 9999, 1 << LayerMask.NameToLayer("Base"));
            foreach (RaycastHit2D hit in hits)
            {
                baseControl = hit.collider.GetComponentInParent<BaseControl>();
                if (BaseConditionCheck(baseControl, bottom))
                    break;
                else
                    baseControl = null;
            }
        }

        if (!baseControl)
        {
            // do a ray cast toward the bottom to detect ground
            RaycastHit2D[] hits = Physics2D.RaycastAll(bottom.position, Vector2.up, 9999, 1 << LayerMask.NameToLayer("Base"));
            foreach (RaycastHit2D hit in hits)
            {
                baseControl = hit.collider.GetComponentInParent<BaseControl>();
                if (BaseConditionCheck(baseControl, bottom))
                    break;
                else
                    baseControl = null;
            }
        }

        if (!baseControl)
        {
            Debug.LogError("Critical Error, should not happen");
            return -Vector3.one;
        }

        return baseControl.FindSnapPosition(bottom.position, IsHuman, bottom.GetComponent<Collider2D>().bounds);
    }

    private void OnDrawGizmos()
    {
        //Gizmos.DrawSphere(transform.position, InteractionRadius);
    }

    private bool BaseConditionCheck(BaseControl baseControl, Transform bottom)
    {
        return baseControl && baseControl.transform != bottom && ((baseControl.SnapWithHuman && IsHuman) || (baseControl.SnapWithProp && !IsHuman));
    }
}


