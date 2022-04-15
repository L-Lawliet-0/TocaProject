using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Spine.Unity;
using Spine;

public class SpineControl : MonoBehaviour
{
    private SkeletonAnimation SkeletonAnimation;
    private Skeleton MySkeleton;
    private Bone LeftArm, RightArm, LeftLeg, RightLeg, LeftHand, RightHand;
    private LimbControl LeftArmControl, RightArmControl, LeftLegControl, RightLegControl;
    private const string leftHand = "zuoshou", leftHandControl = "zuoshoukongzhi";
    private const string rightHand = "youshou", rightHandControl = "youshoukongzhi";
    private const string leftLeg = "zuotui", leftLegControl = "zuotuikongzhi";
    private const string rightLeg = "youtui", rightLegControl = "youtuikongzhi";
    private string[] AllControls;
    private LimbControl[] AllLimbControls;

    public AnimationReferenceAsset LeftHandRaise,
        LeftHandDrop,
        RightHandRaise,
        RightHandDrop,
        Sit,
        Stand,
        Idle;

    public enum Animations
    {
        LeftHandRaise,
        LeftHandDrop,
        RightHandRaise,
        RightHandDrop,
        Sit,
        Stand,
        Idle
    }

    public Transform LeftHandBase, RightHandBase;

    private void Start()
    {
        AllControls = new string[]
        {
            leftHandControl, rightHandControl, leftLegControl, rightLegControl
        };

        SkeletonAnimation = GetComponent<SkeletonAnimation>();
        MySkeleton = SkeletonAnimation.Skeleton;

        LeftArmControl = InitLimbControl(leftHand, leftHandControl, -30, 10, 10, -30, out LeftArm, true);
        RightArmControl = InitLimbControl(rightHand, rightHandControl, -10, 30, -10, 30, out RightArm, true);
        LeftLegControl = InitLimbControl(leftLeg, leftLegControl, -20, 10, 10, -20, out LeftLeg);
        RightLegControl = InitLimbControl(rightLeg, rightLegControl, -10, 20, -10, 20, out RightLeg);

        ((ArmControl)LeftArmControl).HandBase = LeftHandBase.GetComponent<BaseControl>();
        ((ArmControl)RightArmControl).HandBase = RightHandBase.GetComponent<BaseControl>();

        AllLimbControls = new LimbControl[]
        {
            LeftArmControl,
            RightArmControl,
            LeftLegControl,
            RightLegControl
        };

        PlayAnimation(Animations.Idle);

        LeftHand = MySkeleton.FindBone("bone9");
        RightHand = MySkeleton.FindBone("bone12");
    }

    private LimbControl InitLimbControl(string slotName, string controlName, float hu, float hl, float vu, float vl, out Bone thisRef, bool isArm = false)
    {
        Slot slot = MySkeleton.FindSlot(slotName);
        thisRef = slot.Bone;
        SetControlValue(controlName, 0);

        if (isArm)
            return new ArmControl(GetComponent<MoveControl>(), hu, hl, vu, vl, thisRef);
        return new LimbControl(GetComponent<MoveControl>(), hu, hl, vu, vl, thisRef);
    }

    public void PlayAnimation(Animations animation)
    {
        switch (animation) 
        {
            case Animations.Idle:
                SkeletonAnimation.AnimationName = "";
                foreach (string s in AllControls)
                    SetControlValue(s, 0);
                foreach (LimbControl l in AllLimbControls)
                    l.Active = true;
                break;
            case Animations.LeftHandRaise:
                TrackEntry track = SkeletonAnimation.AnimationState.SetAnimation(3, LeftHandRaise, false);
                track.TimeScale = 2f;
                SetControlValue(leftHandControl, 1);
                LeftArmControl.Active = false;
                SkeletonAnimation.AnimationState.End -= HandleLeftHand;
                break;
            case Animations.LeftHandDrop:
                track = SkeletonAnimation.AnimationState.SetAnimation(3, LeftHandDrop, false);
                track.TimeScale = 2;
                SetControlValue(leftHandControl, 1);
                LeftArmControl.Active = false;
                SkeletonAnimation.AnimationState.End += HandleLeftHand;
                break;
            case Animations.RightHandRaise:
                track = SkeletonAnimation.AnimationState.SetAnimation(2, RightHandRaise, false);
                track.TimeScale = 2;
                SetControlValue(rightHandControl, 1);
                RightArmControl.Active = false;
                SkeletonAnimation.AnimationState.End -= HandleRightHand;
                break;
            case Animations.RightHandDrop:
                track = SkeletonAnimation.AnimationState.SetAnimation(2, RightHandDrop, false);
                track.TimeScale = 2;
                SetControlValue(rightHandControl, 1);
                RightArmControl.Active = false;
                SkeletonAnimation.AnimationState.End += HandleRightHand;
                break;
            case Animations.Stand:
                SkeletonAnimation.AnimationState.SetAnimation(1, Stand, false);
                SetControlValue(leftLegControl, 1);
                SetControlValue(rightLegControl, 1);
                LeftLegControl.Active = false;
                RightLegControl.Active = false;
                SkeletonAnimation.AnimationState.End += HandleLeg;
                break;
            case Animations.Sit:
                SkeletonAnimation.AnimationState.SetAnimation(1, Sit, false);
                SetControlValue(leftLegControl, 1);
                SetControlValue(rightLegControl, 1);
                LeftLegControl.Active = false;
                RightLegControl.Active = false;
                SkeletonAnimation.AnimationState.End -= HandleLeg;
                break;
        }
        
    }

    private void HandleLeftHand(TrackEntry trackEntry)
    { 
        if (trackEntry.TrackIndex == 3)
        {
            SetControlValue(leftHandControl, 0);
            LeftArmControl.Active = true;
        }
    }

    private void HandleRightHand(TrackEntry trackEntry)
    {
        if (trackEntry.TrackIndex == 2)
        {
            SetControlValue(rightHandControl, 0);
            RightArmControl.Active = true;
        }
    }

    private void HandleLeg(TrackEntry trackEntry)
    {
        if (trackEntry.TrackIndex == 1)
        {
            SetControlValue(leftLegControl, 0);
            SetControlValue(rightLegControl, 0);
            LeftLegControl.Active = true;
            RightLegControl.Active = true;
        }
        Debug.LogError("called");
    }

    public void SetControlValue(string controlName, float value)
    {
        MySkeleton.FindIkConstraint(controlName).Mix = value;
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.K))
        {
            LeftArm.ScaleY = -LeftArm.ScaleY;
        }

        if (Input.GetKeyDown(KeyCode.L))
        {
            PlayAnimation(Animations.LeftHandDrop);
        }

        LeftArmControl.UpdateLimbRotation();
        RightArmControl.UpdateLimbRotation();
        LeftLegControl.UpdateLimbRotation();
        RightLegControl.UpdateLimbRotation();

        // update base position so that it follows bone
        LeftHandBase.position = LeftHand.GetWorldPosition(transform);
        RightHandBase.position = RightHand.GetWorldPosition(transform);

    }

    public void AssignAttach(bool leftArm, FindControl fc)
    {
        if (leftArm)
            ((ArmControl)LeftArmControl).PendingAttach = fc;
        else
            ((ArmControl)RightArmControl).PendingAttach = fc;
    }
}
