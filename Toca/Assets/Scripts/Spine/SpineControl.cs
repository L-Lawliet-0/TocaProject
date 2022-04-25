using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Spine.Unity;
using Spine;

public class SpineControl : TocaFunction
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
        Idle,
        Eat;

    public enum Animations
    {
        LeftHandRaise,
        LeftHandDrop,
        RightHandRaise,
        RightHandDrop,
        Sit,
        Stand,
        Idle,
        Eat
    }

    public Transform LeftHandBase, RightHandBase;

    public float HipOffset;

    private AttachmentControl Maozi, Kouzhao, Glasses, Mianju, Ershi, Meimao, Bizi, Zui, Yanjing, Toufa, Toufahoumian, Jiaodongxi;

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

        HipOffset = (MySkeleton.FindBone("bone2").GetWorldPosition(transform).y - transform.GetChild(0).transform.position.y) / transform.lossyScale.y * GetComponent<SelectionControl>().DefaultScale.y;

        Maozi = new AttachmentControl("maozi1", MySkeleton, "maozi");
        Kouzhao = new AttachmentControl("kouzhao1", MySkeleton, "kouzhao");
        Glasses = new AttachmentControl("glasses1", MySkeleton, "glasses");
        Mianju = new AttachmentControl("mianju1", MySkeleton, "mianju");
        Ershi = new AttachmentControl("ershi2", MySkeleton, "ershi");
        Meimao = new AttachmentControl("meimao", MySkeleton, "meimao");
        Bizi = new AttachmentControl("bizi", MySkeleton, "bizi");
        Zui = new AttachmentControl("zui2", MySkeleton, "zui");
        Yanjing = new AttachmentControl("yanjing", MySkeleton, "yanjing");
        Toufa = new AttachmentControl("toufa1", MySkeleton, "toufa");
        Toufahoumian = new AttachmentControl("toufahoumian", MySkeleton, "toufahoumian");
        Jiaodongxi = new AttachmentControl("zui1", MySkeleton, "mouth");

        Yanjing.SetAttachment(1);
        Bizi.SetAttachment(1);
        Toufa.SetAttachment(1);
        Zui.SetAttachment(1);

        B7 = MySkeleton.FindBone("bone7");
        B8 = MySkeleton.FindBone("bone8");
        B9 = MySkeleton.FindBone("bone9");
        B10 = MySkeleton.FindBone("bone10");
        B11 = MySkeleton.FindBone("bone11");
        B12 = MySkeleton.FindBone("bone12");
        R7 = B7.Rotation;
        R8 = B8.Rotation;
        R9 = B9.Rotation;
        R10 = B10.Rotation;
        R11 = B11.Rotation;
        R12 = B12.Rotation;
    }

    private Bone B7, B8, B9, B10, B11, B12;
    private float R7, R8, R9, R10, R11, R12;

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
                SkeletonAnimation.AnimationState.Complete += HandleLeftHandRaise;
                break;
            case Animations.LeftHandDrop:
                SkeletonAnimation.AnimationState.Complete -= HandleLeftHandRaise;
                
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
                SkeletonAnimation.AnimationState.Complete += HandleRightHandRaise;
                break;
            case Animations.RightHandDrop:
                SkeletonAnimation.AnimationState.Complete -= HandleRightHandRaise;
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
            case Animations.Eat:
                // set corresponding attachment to true
                if (Eating)
                    break;
                Eating = true;
                SetOpenMouse();
                SkeletonAnimation.AnimationState.Complete += HandleEat;
                SkeletonAnimation.AnimationState.SetAnimation(4, Eat, false);
                
                break;
        }
        
    }
    private bool Eating = false;

    private void HandleEat(TrackEntry trackEntry)
    {
        Debug.LogError("Handleeat");
        if (trackEntry.TrackIndex == 4)
        {
            Eating = false;
            SetDefaultMouse();
            SkeletonAnimation.AnimationState.Complete -= HandleEat;
        }
    }

    public void SetOpenMouse()
    {
        Jiaodongxi.SetAttachment("mouth1");
        Zui.SetAttachment();
        Debug.LogError("Set open mouse");
    }

    public void SetDefaultMouse()
    {
        if (!Eating)
        {
            Jiaodongxi.SetAttachment();
            Zui.SetAttachment(1);
        }
    }

    private void HandleLeftHandRaise(TrackEntry trackEntry)
    {
        if (trackEntry.TrackIndex == 3)
        {
            float r7 = B7.AppliedRotation;
            float r8 = B8.AppliedRotation;
            float r9 = B9.AppliedRotation;

            SetControlValue(leftHandControl, 0);
            LeftArmControl.Active = true;

            B7.Rotation = r7;
            B8.Rotation = r8;
            B9.Rotation = r9;
        }
    }

    private void HandleRightHandRaise(TrackEntry trackEntry)
    {
        if (trackEntry.TrackIndex == 2)
        {
            float r10 = B10.AppliedRotation;
            float r11 = B11.AppliedRotation;
            float r12 = B12.AppliedRotation;

            SetControlValue(rightHandControl, 0);
            RightArmControl.Active = true;

            B10.Rotation = r10;
            B11.Rotation = r11;
            B12.Rotation = r12;
        }
    }

    private void HandleLeftHand(TrackEntry trackEntry)
    { 
        if (trackEntry.TrackIndex == 3)
        {
            B7.Rotation = R7;
            B8.Rotation = R8;
            B9.Rotation = R9;

            SetControlValue(leftHandControl, 0);
            LeftArmControl.Active = true;
        }
    }

    private void HandleRightHand(TrackEntry trackEntry)
    {
        if (trackEntry.TrackIndex == 2)
        {
            B10.Rotation = R10;
            B11.Rotation = R11;
            B12.Rotation = R12;

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
            PlayAnimation(Animations.Eat);
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