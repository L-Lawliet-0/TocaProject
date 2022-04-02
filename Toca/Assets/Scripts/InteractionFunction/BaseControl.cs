using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BaseControl : TocaFunction
{
    public bool SnapWithHuman, SnapWithProp;
    public SnapType HumanHorizontalSnapType, HumanVerticalSnapType;
    public SnapType PropHorizontalSnapType, PropVerticalSnapType;

    public Dictionary<FindControl, AttachData> Attachments; // objects currently being attached on this object

    public List<FindControl> VisibleDatas;

    public int SnapLimit;

    public enum SnapType
    {
        BoundSnap, // bound snap is to keep object bounding box within the snap object, this uses snap bound collider value
        PointSnap // Point snap is to keep object x or y position at one specific position
    }

    public Transform HumanPointSnap, PropPointSnap;
    public float MaxObjectWidth = float.MaxValue, MaxObjectHeight = float.MaxValue;
    public bool IgnoreLimit;
    public bool IsHand;
    public int BaseID;

    public class AttachData
    {
        public MoveControl mc;
        public Vector2 offset;

        public AttachData(FindControl find, Vector3 snapPos, Transform me)
        {
            mc = (MoveControl)find.TocaObject.GetTocaFunction<MoveControl>();
            if (mc)
            {
                offset = snapPos - me.position;
            }
        }
    }

    private void Awake()
    {
        if (!HumanPointSnap)
            HumanPointSnap = transform;
        if (!PropPointSnap)
            PropPointSnap = transform;
        Attachments = new Dictionary<FindControl, AttachData>();
        VisibleDatas = new List<FindControl>();
    }

    private void Update()
    {
        foreach (KeyValuePair<FindControl, AttachData> pair in Attachments)
        {
            if (pair.Value.mc)
            {
                pair.Value.mc.UpdateTargetPosition(CalculateTargetPos(pair.Key, pair.Value));
            }
        }
    }

    public Vector3 CalculateTargetPos(FindControl find, AttachData attach)
    {
        return transform.position + new Vector3(attach.offset.x, attach.offset.y) - Vector3.up * find.transform.localPosition.y * find.transform.lossyScale.y;
    }

    public Vector3 GetTargetPos(FindControl find, AttachData attach)
    {
        return transform.position + new Vector3(attach.offset.x, attach.offset.y);
    }

    public void Attach(FindControl find)
    {
        AttachData data = new AttachData(find, FindSnapPosition(find.transform.position, find.IsHuman, find.GetComponent<Collider2D>().bounds), transform);
        Attachments.Add(find, data);
        VisibleDatas.Add(find);
        if (data.mc)
        {
            data.mc.UpdateTargetPosition(CalculateTargetPos(find, data));
        }
    }

    public void Detach(FindControl find)
    {
        Attachments.Remove(find);
        VisibleDatas.Remove(find);
    }

    public bool CanbeSnapped(FindControl finder)
    {
        // can not have more than limit amount of attachment
        bool limit = IgnoreLimit || Attachments.Count < SnapLimit;
        bool width = MaxObjectWidth > finder.GetComponent<Collider2D>().bounds.size.x;
        bool height = MaxObjectHeight > finder.GetComponent<Collider2D>().bounds.size.y;
        return limit && width && height;
    }

    public Vector3 FindSnapPosition(Vector3 bottomCenter, bool isHuman, Bounds boundingBox)
    {
        if (!GetComponent<Collider2D>().OverlapPoint(bottomCenter))
        {
            bottomCenter = GetComponent<Collider2D>().ClosestPoint(bottomCenter);
        }
        // now bottom center is on the border or inside the polygon collider2d

        float x, y;
        if (isHuman)
        {
            x = GetSnapValue(bottomCenter.x, bottomCenter, HumanPointSnap.position.x, HumanHorizontalSnapType == SnapType.PointSnap, boundingBox, Vector2.right);
            y = GetSnapValue(bottomCenter.y, bottomCenter, HumanPointSnap.position.y, HumanVerticalSnapType == SnapType.PointSnap, boundingBox, Vector2.up);
        }
        else
        {
            x = GetSnapValue(bottomCenter.x, bottomCenter, PropPointSnap.position.x, PropHorizontalSnapType == SnapType.PointSnap, boundingBox, Vector2.right);
            y = GetSnapValue(bottomCenter.y, bottomCenter, PropPointSnap.position.y, PropVerticalSnapType == SnapType.PointSnap, boundingBox, Vector2.up);
        }

        return new Vector3(x, y, GlobalParameter.Depth);
    }

    public float GetSnapValue(float objectValue, Vector3 objectPos, float snapValue, bool isPoint, Bounds boundingBox, Vector2 direction)
    {
        float value = objectValue;
        if (isPoint)
            value = snapValue;
        else if (direction.x > 0)
        {
            // check position to right side
            float extents = boundingBox.extents.x;
            int saveLayer = gameObject.layer;
            gameObject.layer = LayerMask.NameToLayer("OnlyOne");

            // check enough space to the right
            Vector3 checkPos = objectPos + new Vector3(direction.x, direction.y).normalized * extents;
            if (!Physics2D.OverlapBox(checkPos, Vector2.one * .1f, 0, 1 << LayerMask.NameToLayer("OnlyOne")))
            {
                // use box cast instead
                // Physics2D.BoxCast(checkPos, Vector2.one * .1f, 0, -direction, 9999, 1 << LayerMask.NameToLayer("OnlyOne"));

                // space not enough
                RaycastHit2D hit = Physics2D.BoxCast(checkPos, Vector2.one * .1f, 0, -direction, 9999, 1 << LayerMask.NameToLayer("OnlyOne")); //Physics2D.Raycast(checkPos, -direction, 9999, 1 << LayerMask.NameToLayer("OnlyOne"));
                if (hit)
                    value -= hit.distance;
                Debug.LogError("Adjusted position because of right " + hit.distance);
            }
            // reverse direction
            checkPos = objectPos - new Vector3(direction.x, direction.y).normalized * extents;
            if (!Physics2D.OverlapBox(checkPos, Vector2.one * .1f, 0, 1 << LayerMask.NameToLayer("OnlyOne")))
            {
                RaycastHit2D hit = Physics2D.BoxCast(checkPos, Vector2.one * .1f, 0, direction, 9999, 1 << LayerMask.NameToLayer("OnlyOne")); //Physics2D.Raycast(checkPos, direction, 9999, 1 << LayerMask.NameToLayer("OnlyOne"));
                if (hit)
                    value += hit.distance;
                Debug.LogError("Adjusted position because of left " + hit.distance);
            }

            gameObject.layer = saveLayer;
        }
        else if (direction.y > 0)
        {
            // we know that the incoming bounding box must be smaller than the maxobject height
            // we know that the incoming position will not drop below the bottom
            // so we only check the top
            float top = objectValue + boundingBox.size.y; // this is the top position of the incoming bounding box
            float maxTop = GetComponent<Collider2D>().bounds.center.y - GetComponent<Collider2D>().bounds.extents.y + MaxObjectHeight; // this is the max height allowed
            if (top > maxTop)
                value -= (top - maxTop);
        }
        return value;
    }
}
