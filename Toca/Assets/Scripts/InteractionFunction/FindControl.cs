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
    public bool IsFood;
    public bool IsGlasses; // is this glasses
    public BaseControl CurrentAttachment; // the base this object is currently attaching
    public BaseControl BasePreview;
    public bool Arrived;
    private MoveControl MoveControl;
    private ArmControl NearByArm;

    public BaseControl AttachBase;
    public int AttachBaseID;

    private Collider2D ThisCollider;
    public float ObjectWidth { get { return ThisCollider.bounds.size.x / transform.lossyScale.x; } }
    public float ObjectHeight { get { return ThisCollider.bounds.size.y / transform.lossyScale.y; } }
    public float Yoffset { get { return CalculateOffset(); } set { yoffset = value; } }
    private float yoffset;

    public Vector3 BottomCenter { get { return GetBottomCenter(); } }


    private Vector3 GetBottomCenter()
    {
        float angle = GlobalParameter.ClampAngle(transform.eulerAngles.z);
        if (angle < 10 || angle > 350)
            return transform.position;
        else
            return ThisCollider.bounds.center + Yoffset * Vector3.up;
    }

    private float CalculateOffset()
    {
        float angle = GlobalParameter.ClampAngle(transform.eulerAngles.z);
        if (angle < 10 || angle > 350)
            return yoffset;

        // do raycast
        int temp = gameObject.layer;
        gameObject.layer = LayerMask.NameToLayer("OnlyOne");
        RaycastHit2D hit = Physics2D.Raycast(ThisCollider.bounds.center + Vector3.down * 20, Vector2.up, 99, 1 << LayerMask.NameToLayer("OnlyOne"));

        gameObject.layer = temp;
        
        return -(20 - hit.distance) / transform.lossyScale.y;
    }

    private void Awake()
    {
        Bounds bounds = GetComponent<Collider2D>().bounds;
        //ObjectWidth = bounds.size.x;
        //ObjectHeight = bounds.size.y;
        ThisCollider = GetComponent<Collider2D>();
        Yoffset = transform.localPosition.y * transform.lossyScale.y;
    }

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

        //Debug.LogError("Reach!" + CurrentAttachment.name);
        Attach(CurrentAttachment);

        BasePreview = null;
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

        if (BasePreview && BasePreview.transform.parent && BasePreview.transform.parent.GetComponent<SpineControl>())
        {
            BasePreview.transform.parent.GetComponent<SpineControl>().AssignAttach(BasePreview.MyBaseAttributes.IsLeftHand, this);
        }

        if (BasePreview && BasePreview.MyBaseAttributes.IsMouth)
        {
            BasePreview.GetComponent<EatControl>().PendingFood = this;
        }
    }

    // return a base near by
    public BaseControl FindBaseNearBy()
    {
        List<BaseControl> cache = new List<BaseControl>();
        BaseControl bc = null;
        Bounds bounds = GetComponent<Collider2D>().bounds;
        float interactionRange = Mathf.Min(bounds.extents.x, bounds.extents.y);

        Collider2D[] colliders = Physics2D.OverlapCircleAll(transform.position /*+ Vector3.up * bounds.extents.y*/, interactionRange, 1 << LayerMask.NameToLayer("Base"));
        foreach (Collider2D collider in colliders)
        {
            bc = collider.GetComponentInParent<BaseControl>();
            if (BaseConditionCheck(bc))
                cache.Add(bc);
            else
                bc = null;
        }

        BaseControl value = null;
        float minDis = float.MaxValue;
        foreach (BaseControl b in cache)
        {
            float dis = Vector3.Distance(transform.position, b.GetComponent<Collider2D>().bounds.center);//ClosestPoint(transform.position)); //b.transform.position);
            if (dis < minDis)
            {
                minDis = dis;
                value = b;
            }
        }

        return value;
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

    public bool BaseConditionCheck(BaseControl baseControl)
    {
        bool selfCondition = baseControl && baseControl.TocaObject != TocaObject && ((baseControl.SnapWithHuman && IsHuman) || (baseControl.SnapWithProp && !IsHuman));
        if (!selfCondition)
            return false;

        // don't attach to any object attached to this object itself
        bool isChild = false;

        TocaObject toca = baseControl.TocaObject;
        FindControl otherFind = null;

        if (toca)
            otherFind = (FindControl)baseControl.TocaObject.GetTocaFunction<FindControl>();
        List<TocaFunction> myBases = TocaObject.GetTocaFunctions<BaseControl>(); 
        if (otherFind)
        {
            isChild = FindInBase(otherFind, myBases);
        }
        return !isChild && baseControl.CanbeSnapped(this);
    }

    private bool FindInBase(FindControl target, List<TocaFunction> bases)
    {
        foreach (BaseControl bc in bases)
        {
            foreach (KeyValuePair<FindControl, BaseControl.AttachData> pair in bc.Attachments)
            {
                if (pair.Key == target)
                    return true;
                // get new bases
                List<TocaFunction> myBases = pair.Key.TocaObject.GetTocaFunctions<BaseControl>();
                if (FindInBase(target, myBases))
                    return true;
                // continue to search
            }
        }
        return false;
    }
}


