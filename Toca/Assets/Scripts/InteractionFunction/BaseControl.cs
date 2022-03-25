using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BaseControl : TocaFunction
{
    public bool SnapWithHuman, SnapWithProp;
    public SnapType HumanHorizontalSnapType, HumanVerticalSnapType;
    public SnapType PropHorizontalSnapType, PropVerticalSnapType;
    public List<FindControl> Attachments; // objects currently being attached on this object

    public int SnapLimit;

    public enum SnapType
    {
        BoundSnap, // bound snap is to keep object bounding box within the snap object, this uses snap bound collider value
        PointSnap // Point snap is to keep object x or y position at one specific position
    }

    public Transform HumanPointSnap, PropPointSnap;
    public bool IgnoreLimit, IgnoreWidth, IgnoreHeight;
    public bool IsHand;

    private void Awake()
    {
        if (!HumanPointSnap)
            HumanPointSnap = transform;
        if (!PropPointSnap)
            PropPointSnap = transform;
        Attachments = new List<FindControl>();
    }

    public bool CanbeSnapped(FindControl finder)
    {
        // can not have more than limit amount of attachment
        bool limit = IgnoreLimit || Attachments.Count < SnapLimit;
        bool width = IgnoreWidth || GetComponent<Collider2D>().bounds.extents.x > finder.GetComponent<Collider2D>().bounds.extents.x;
        bool height = IgnoreHeight || GetComponent<Collider2D>().bounds.extents.y > finder.GetComponent<Collider2D>().bounds.extents.y;
        return limit && width && height;
    }

    public Transform FindSnapPosition(Vector3 bottomCenter, bool isHuman, Bounds boundingBox)
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

        // create an empty gameobject for attachment
        GameObject snap = new GameObject();
        snap.transform.position = new Vector3(x, y, GlobalParameter.Depth);
        snap.transform.SetParent(transform);
        return snap.transform;
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
                // space not enough
                RaycastHit2D hit = Physics2D.Raycast(checkPos, -direction, 9999, 1 << LayerMask.NameToLayer("OnlyOne"));
                if (hit)
                    value -= hit.distance;
                Debug.LogError("Adjusted position because of right " + hit.distance);
            }
            // reverse direction
            checkPos = objectPos - new Vector3(direction.x, direction.y).normalized * extents;
            if (!Physics2D.OverlapBox(checkPos, Vector2.one * .1f, 0, 1 << LayerMask.NameToLayer("OnlyOne")))
            {
                RaycastHit2D hit = Physics2D.Raycast(checkPos, direction, 9999, 1 << LayerMask.NameToLayer("OnlyOne"));
                if (hit)
                    value += hit.distance;
                Debug.LogError("Adjusted position because of left " + hit.distance);
            }

            gameObject.layer = saveLayer;
        }
        return value;
    }

    public void Attach(FindControl find)
    {
        Attachments.Add(find);
    }

    public void Detach(FindControl find)
    {
        Attachments.Remove(find);
    }
}
