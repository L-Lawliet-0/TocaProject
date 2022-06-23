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
        for (int i = 0; i < 1; i++)
        {
            TrackEntry track = GetComponent<SkeletonAnimation>().AnimationState.SetAnimation(1 + i * 9999, LoopAnimations[i], true);
        }

        Skeleton skeleton = GetComponent<SkeletonAnimation>().skeleton;
        skeleton.SetAttachment("biyan", null);
        skeleton.SetAttachment("daxiao", null);
        skeleton.SetAttachment("xingxing", null);
    }

    private void OnEnable()
    {
        StartCoroutine("Feiji");
    }

    private IEnumerator Feiji()
    {
        while (true)
        {
            TrackEntry track = GetComponent<SkeletonAnimation>().AnimationState.SetAnimation(10000 , LoopAnimations[1], false);
            track.TimeScale = .25f;
            yield return new WaitForSeconds(Random.Range(15f, 30f));
            GetComponent<SkeletonAnimation>().AnimationState.ClearTrack(10000);
        }
    }
}
