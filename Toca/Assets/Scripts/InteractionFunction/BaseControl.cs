using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BaseControl : TocaFunction
{
    public BaseAttributes MyBaseAttributes;

    [System.Serializable]
    public class BaseAttributes
    {
        public bool[] Parameters;
        public bool InheirtLayer, // does attached object inheirt from this layer value
                    BookStand, // stand parameters
                    BrushStand, // dressing brush stand
                    HaveCover; // does this base has cover that needs to stand in front of the base layer

        public void InitalizePara()
        {
            Parameters = new bool[]
            {
                InheirtLayer,
                BookStand,
                BrushStand
            };
        }       
    }

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
    public SpriteRenderer Cover;

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
        MyBaseAttributes.InitalizePara();
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
            if (pair.Value.mc && !TocaObject.GetTocaFunction<SlideControl>())
            {
                // recalculate if this base has cover
                //if (MyBaseAttributes.HaveCover)
                pair.Value.mc.UpdateTargetPosition(CalculateTargetPos(pair.Key, pair.Value));
            }
        }
    }

    public void RecalculateSnapPos(FindControl find)
    {
        if (Attachments.ContainsKey(find))
        {
            Attachments[find].offset = FindSnapPosition(find) - transform.position;
        }
    }

    public Vector3 CalculateTargetPos(FindControl find, AttachData attach)
    {
        return transform.position + new Vector3(attach.offset.x, attach.offset.y) - Vector3.up * find.Yoffset;
    }

    public Vector3 GetTargetPos(FindControl find, AttachData attach)
    {
        return transform.position + new Vector3(attach.offset.x, attach.offset.y);
    }

    public void Attach(FindControl find)
    {
        AttachData data = new AttachData(find, FindSnapPosition(find), transform);
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
        bool width = finder.IsHuman || MaxObjectWidth > finder.ObjectWidth; // human automatically ignore width check
        bool height = finder.IsHuman || MaxObjectHeight > finder.ObjectHeight; // human automatically ignore height check
        return limit && width && height;
    }

    public Vector3 FindSnapPosition(FindControl find)
    {

        Vector3 bottomCenter = find.BottomCenter;
        if (!GetComponent<Collider2D>().OverlapPoint(bottomCenter))
        {
            // the object should fall straight down
            // do a ray cast downward first
            int saveLayer = gameObject.layer;
            gameObject.layer = LayerMask.NameToLayer("OnlyOne");

            RaycastHit2D hit = Physics2D.Raycast(bottomCenter, Vector2.down, 99, 1 << LayerMask.NameToLayer("OnlyOne"));
            if (hit)
                bottomCenter = hit.point;
            else
                bottomCenter = GetComponent<Collider2D>().ClosestPoint(bottomCenter);


            gameObject.layer = saveLayer;
            
        }
        // now bottom center is on the border or inside the polygon collider2d

        float x, y;
        if (find.IsHuman)
        {
            x = GetSnapValue(bottomCenter.x, bottomCenter, HumanPointSnap.position.x, HumanHorizontalSnapType == SnapType.PointSnap, find, Vector2.right);
            y = GetSnapValue(bottomCenter.y, bottomCenter, HumanPointSnap.position.y, HumanVerticalSnapType == SnapType.PointSnap, find, Vector2.up);
        }
        else
        {
            x = GetSnapValue(bottomCenter.x, bottomCenter, PropPointSnap.position.x, PropHorizontalSnapType == SnapType.PointSnap, find, Vector2.right);
            y = GetSnapValue(bottomCenter.y, bottomCenter, PropPointSnap.position.y, PropVerticalSnapType == SnapType.PointSnap, find, Vector2.up);
        }

        return new Vector3(x, y, GlobalParameter.Depth);
    }

    public float GetSnapValue(float objectValue, Vector3 objectPos, float snapValue, bool isPoint, FindControl find, Vector2 direction)
    {
        float value = objectValue;
        if (isPoint)
            value = snapValue;
        else if (direction.x > 0)
        {
            // check position to right side
            float extents = find.ObjectWidth / 2;
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
            float top = objectValue + find.ObjectHeight; // this is the top position of the incoming bounding box
            float maxTop = GetComponent<Collider2D>().bounds.center.y - GetComponent<Collider2D>().bounds.extents.y + MaxObjectHeight; // this is the max height allowed
            if (top > maxTop)
                value -= (top - maxTop);

            // if there's cover, snap so that part of the top is showing
            if (MyBaseAttributes.HaveCover)
            {
                float coverTop = Cover.bounds.center.y + Cover.bounds.extents.y;
                float objectTop = value + find.ObjectHeight;
                
                if (objectTop - .1f < coverTop)
                {
                    value += (coverTop + .1f - objectTop);
                }
            }
        }
        return value;
    }

    public float GetMaxY()
    {
        return GetComponent<Collider2D>().bounds.center.y + GetComponent<Collider2D>().bounds.extents.y;
    }

    public float GetYRange()
    {
        return GetComponent<Collider2D>().bounds.size.y;
    }
}
