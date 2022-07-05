using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Spine;
using Spine.Unity;

public class CatControl : TocaFunction
{
    public SkeletonAnimation SkeletonAnimation;

    // an array of animations that will be played randomly
    public AnimationReferenceAsset[] RandomAnimations;
    public int[] AnimationGaps;
    public bool[] Loopables;

    public AnimationReferenceAsset Eat;
    private AttachmentControl Mouth;
    public GameObject EatFX;

    private void Awake()
    {
        Mouth = new AttachmentControl("maozui1", SkeletonAnimation.Skeleton, "");
        SetMouth(false);
    }

    private void Start()
    {
        TouchControl tc = (TouchControl)TocaObject.GetTocaFunction<TouchControl>();
        tc.TouchCallBacks.Add(Scream);
        tc.ClickCallBacks.Add(Scream);
    }

    public void Scream()
    {
        SoundManager.Instance.PlaySFX(27, true, TocaObject.transform.position);
    }

    private void OnEnable()
    {
        for (int i = 0; i < RandomAnimations.Length; i++)
        {
            StartCoroutine("RandomlyPlayAnimation", i);
        }
    }

    private IEnumerator RandomlyPlayAnimation(int index)
    {
        AnimationReferenceAsset animation = RandomAnimations[index];
        int trackIndex = index;
        float lowGap = AnimationGaps[index];
        float highGap = lowGap * 2;
        bool loop = Loopables[index];

        while (true)
        {
            bool isLoop = loop && Random.Range(0f, 1f) < .5f;
            SkeletonAnimation.AnimationState.SetAnimation(trackIndex, animation, isLoop);
            yield return new WaitForSeconds(Random.Range(lowGap, highGap));
        }
    }

    public void PlayEat()
    {
        Debug.LogError("Play eat!");
        TrackEntry track = SkeletonAnimation.AnimationState.SetAnimation(RandomAnimations.Length, Eat , false);
        track.TimeScale = 2;
        SkeletonAnimation.AnimationState.Complete += HandleEat;

        GameObject obj = Instantiate(EatFX);
        Destroy(obj, 1);
        obj.transform.position = transform.GetChild(2).position;
    }

    public void HandleEat(TrackEntry track)
    {
        if (track.TrackIndex == RandomAnimations.Length)
        {
            SkeletonAnimation.AnimationState.SetEmptyAnimation(RandomAnimations.Length, -1f);
            //SkeletonAnimation.AnimationState.AddEmptyAnimation(RandomAnimations.Length, 0, 0);
            SkeletonAnimation.AnimationState.Complete -= HandleEat;
            SetMouth(false);
        }
    }

    public void SetMouth(bool open)
    {
        if (open)
            Mouth.SetAttachment("maozuichidx");
        else
            Mouth.SetAttachment("maozui1");
    }
}
