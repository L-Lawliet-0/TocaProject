using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Spine.Unity;
using Spine;

public class OpenAnimationControl : TocaFunction
{
    public AnimationReferenceAsset OpenAnimation;
    public SkeletonAnimation m_SkeletonAnimation;
    public int TrackIndex;
    private OpenControl Oc;
    public float AnimationSpeed = 1;

    private void Awake()
    {
        BaseSaves = new List<Transform>();
    }

    private void Start()
    {
        Oc = (OpenControl)TocaObject.GetTocaFunction<OpenControl>();
        Oc.OnOpening += Open;
        Oc.OnClosing += Close;

        Oc.OnClose();
    }

    public void Open()
    {
        TrackEntry track = m_SkeletonAnimation.AnimationState.SetAnimation(TrackIndex, OpenAnimation, false);
        track.Reverse = false;
        track.TimeScale = AnimationSpeed;
    }

    private List<Transform> BaseSaves;
    public void Close()
    {
        TrackEntry track = m_SkeletonAnimation.AnimationState.SetAnimation(TrackIndex, OpenAnimation, false);
        track.Reverse = true;
        track.TimeScale = AnimationSpeed;

        FinishClose();
        
        if (Oc.OpenObj)
        {
            for (int i = Oc.OpenObj.transform.childCount - 1; i >= 0; i--)
            {
                Transform tran = Oc.OpenObj.transform.GetChild(i);
                if (tran.GetComponent<BaseControl>())
                {
                    tran.parent = null;
                    BaseSaves.Add(tran);
                }
            }
        }
        Invoke("FinishClose", OpenAnimation.Animation.Duration / track.TimeScale);
    }

    private void FinishClose()
    {
        foreach (Transform tran in BaseSaves)
        {
            tran.SetParent(Oc.OpenObj.transform);
        }
        BaseSaves.Clear();
    }
}
