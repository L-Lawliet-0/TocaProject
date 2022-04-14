using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Spine.Unity;
using Spine;

public class SpineTest : MonoBehaviour
{
    private SkeletonAnimation SkeletonAnimation;
    private Bone LeftArm, RightArm, LeftLeg, RightLeg;
    private void Start()
    {
        SkeletonAnimation = GetComponent<SkeletonAnimation>();
        Skeleton skeleton = SkeletonAnimation.Skeleton;
        Slot slot = skeleton.FindSlot("zuoshou");
        LeftArm = slot.Bone;

        LeftArm = SkeletonAnimation.skeleton.FindBone("bone7");
        skeleton.FindIkConstraint("zuoshoukongzhi").Mix = 0;

        foreach (Slot s in skeleton.Slots)
        {
            Debug.LogError(s.ToString() + " : " + s.Bone.ToString());
        }
    }

    private void Update()
    {
        //LeftArm.SetLocalPosition(LeftArm.GetLocalPosition() + Vector2.up * Time.deltaTime);
        LeftArm.Rotation += 60 * Time.deltaTime;
        Debug.LogError(LeftArm.Rotation);
    }
}
