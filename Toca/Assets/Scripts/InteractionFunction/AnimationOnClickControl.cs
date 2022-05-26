using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Spine;
using Spine.Unity;

public class AnimationOnClickControl : TocaFunction
{
    public string animationName;
    private SkeletonAnimation SkeletonAnimation;
    private void Start()
    {
        SkeletonAnimation = GetComponent<SkeletonAnimation>();
        TouchControl tc = (TouchControl)TocaObject.GetTocaFunction<TouchControl>();
        tc.ClickCallBacks.Add(PlayAnimation);
    }

    private void PlayAnimation()
    {
        SkeletonAnimation.AnimationName = null;
        SkeletonAnimation.AnimationName = animationName;
    }
}
