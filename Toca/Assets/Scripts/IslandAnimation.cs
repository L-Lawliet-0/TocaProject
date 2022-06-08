using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Spine;
using Spine.Unity;

public class IslandAnimation : MonoBehaviour
{
    public AnimationReferenceAsset[] LoopAnimations;

    private void Start()
    {
        GetComponent<SkeletonAnimation>().AnimationState.SetAnimation(1, LoopAnimations[0], true);
    }
}