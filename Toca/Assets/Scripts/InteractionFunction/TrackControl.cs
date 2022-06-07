using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TrackControl : TocaFunction
{
    public Transform CharacterShadowLeft, CharacterShadowRight;
    public BaseControl m_BaseControl;
    private int CountCache;

    private static TrackControl m_Instance;
    public static TrackControl Instance { get { return m_Instance; } }

    public float PositionOffset; // the position offset of the track
    public float OffsetRange;

    private void Awake()
    {
        m_Instance = this;
    }

    private void Start()
    {
        m_BaseControl = (BaseControl)TocaObject.GetTocaFunction<BaseControl>();
        //CharacterTrack.Instance.SetTrack(false);

        TouchControl tc = GetComponent<TouchControl>();
        tc.PositionChangeCallBacksV3.Add(PositionChange);
        tc.TouchCallBacksV3.Add(OnTouch);
        tc.DeTouchCallBacks.Add(DeTouch);
    }

    private Vector3 PositionSave;
    public void OnTouch(Vector3 pos)
    {
        PositionSave = pos;
        Debug.LogError("OnTouch");
    }

    public void PositionChange(Vector3 pos)
    {
        float xDiff = pos.x - PositionSave.x;
        PositionOffset += xDiff;
        PositionOffset = Mathf.Clamp(PositionOffset, -OffsetRange, OffsetRange);

        PositionSave = pos;
    }

    public void DeTouch()
    {
        if (PositionOffset < -OffsetRange / 2)
            CharacterTrack.Instance.SwapPage(false);
        else if (PositionOffset > OffsetRange / 2)
            CharacterTrack.Instance.SwapPage(true);
    }

    private void Update()
    {
        transform.position = new Vector3(CameraController.Instance.transform.position.x, transform.position.y, transform.position.z) + Vector3.right * PositionOffset;
        if (m_BaseControl.Attachments.Count != CountCache)
        {
            CharacterShadowLeft.transform.localPosition = new Vector3(-CameraController.Instance.CamWidth, -1.5f);
            CharacterShadowRight.transform.localPosition = new Vector3(CameraController.Instance.CamWidth, -1.5f);

            List<FindControl> finds = new List<FindControl>(m_BaseControl.Attachments.Keys);

            // sort finds based on their horizontal positions
            Xcompare xc = new Xcompare();
            finds.Sort(xc);

            // now findcntrols are sorted
            float startPos = /*CameraController.Instance.*/transform.position.x - (finds.Count - 1) * 3.5f / 2;
            int start = -finds.Count / 2;
            for (int i = 0; i < finds.Count; i++)
            {
                MoveControl mc = (MoveControl)finds[i].TocaObject.GetTocaFunction<MoveControl>();
                Vector3 newPos = new Vector3(startPos + i * 3.5f, m_BaseControl.transform.position.y, GlobalParameter.Depth);
                ((SpineControl)mc.TocaObject.GetTocaFunction<SpineControl>()).ArrivedTarget = false;
                mc.UpdateTargetPosition(newPos);
            }
        }
        CountCache = m_BaseControl.Attachments.Count;
    }

    public void CharacterIn()
    {
        CountCache = -1;
        Update();
    }

    public void CharacterOut()
    {
        List<FindControl> finds = new List<FindControl>(m_BaseControl.Attachments.Keys);

        // sort finds based on their horizontal positions
        Xcompare xc = new Xcompare();
        finds.Sort(xc);

        // now findcntrols are sorted
        float bound = CameraController.Instance.Width_Half + 2;

        Vector3 leftPos = new Vector3(CameraController.Instance.transform.position.x - bound, m_BaseControl.transform.position.y, GlobalParameter.Depth);
        Vector3 rightPos = new Vector3(CameraController.Instance.transform.position.x + bound, m_BaseControl.transform.position.y, GlobalParameter.Depth);

        int left = finds.Count / 2;
        for (int i = 0; i < left; i++)
        {
            MoveControl mc = (MoveControl)finds[i].TocaObject.GetTocaFunction<MoveControl>();
            ((SpineControl)mc.TocaObject.GetTocaFunction<SpineControl>()).ArrivedTarget = false;
            mc.UpdateTargetPosition(leftPos);
        }

        for (int i = left; i < finds.Count; i++)
        {
            MoveControl mc = (MoveControl)finds[i].TocaObject.GetTocaFunction<MoveControl>();
            ((SpineControl)mc.TocaObject.GetTocaFunction<SpineControl>()).ArrivedTarget = false;
            mc.UpdateTargetPosition(rightPos);
        }
    }

    public class Xcompare : Comparer<FindControl>
    {
        public override int Compare(FindControl x, FindControl y)
        {
            return (int)(x.TocaObject.transform.position.x - y.TocaObject.transform.position.x);
        }
    }

    public List<TocaObject> DetachAllAttachments()
    {
        List<TocaObject> returnValues = new List<TocaObject>();
        List<FindControl> finds = new List<FindControl>(m_BaseControl.Attachments.Keys);

        foreach (FindControl find in finds)
        {
            find.Detach();
            returnValues.Add(find.TocaObject);
            ((MoveControl)find.TocaObject.GetTocaFunction<MoveControl>()).CurrentMoveMode = MoveControl.MoveMode.TimeReach;
        }

        return returnValues;
    }
}
