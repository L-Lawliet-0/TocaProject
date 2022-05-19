using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Spine;
using Spine.Unity;

public class SpineUI : MonoBehaviour
{
    private static SpineUI m_Instance;
    public static SpineUI Instance { get { return m_Instance; } }
    public AttachmentControl mianju, maozi, glasses, kouzhao;

    public Dictionary<string, AttachmentControl> Pairs;
    private SkeletonGraphic SkeletonGraphic;

    private void Awake()
    {
        m_Instance = this;
    }
    private void Start()
    {
        SkeletonGraphic = GetComponent<SkeletonGraphic>();
        Skeleton skeleton = SkeletonGraphic.Skeleton;
        mianju = new AttachmentControl("mianju1", skeleton, "mianju");
        maozi = new AttachmentControl("maozi1", skeleton, "maozi");
        glasses = new AttachmentControl("glasses1", skeleton, "glasses");
        kouzhao = new AttachmentControl("kouzhao1", skeleton, "kouzhao");

        Pairs = new Dictionary<string, AttachmentControl>();
        Pairs.Add("mianju", mianju);
        Pairs.Add("maozi", maozi);
        Pairs.Add("glasses", glasses);
        Pairs.Add("kouzhao", kouzhao);
    }

    public void ChangeSkin(int index)
    {
        SkeletonGraphic.Skeleton.SetSkin("fushi" + index);
        SkeletonGraphic.Skeleton.SetSlotsToSetupPose();
    }

    public void SetSkinColor(Color color)
    {
        SkeletonGraphic.Skeleton.FindSlot("tou").SetColor(color);
        SkeletonGraphic.Skeleton.FindSlot("shenti").SetColor(color);
        SkeletonGraphic.Skeleton.FindSlot("youshou").SetColor(color);
        SkeletonGraphic.Skeleton.FindSlot("zuoshou").SetColor(color);
        SkeletonGraphic.Skeleton.FindSlot("youtui").SetColor(color);
        SkeletonGraphic.Skeleton.FindSlot("zuotui").SetColor(color);
    }
}
