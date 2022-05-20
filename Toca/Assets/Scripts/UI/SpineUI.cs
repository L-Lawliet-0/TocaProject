using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Spine;
using Spine.Unity;

public class SpineUI : MonoBehaviour
{
    private static SpineUI m_Instance;
    public static SpineUI Instance { get { return m_Instance; } }
    public AttachmentControl mianju, maozi, glasses, kouzhao, yanjing, bizi, zui, tou, toufa, toufahoumian;

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
        yanjing = new AttachmentControl("yanjing", skeleton, "yanjing");
        bizi = new AttachmentControl("bizi", skeleton, "bizi");
        zui = new AttachmentControl("zui2", skeleton, "zui");
        tou = new AttachmentControl("tou", skeleton, "");
        toufa = new AttachmentControl("toufa1", skeleton, "toufa");
        toufahoumian = new AttachmentControl("toufahoumian", skeleton, "toufahoumian");
        yanjing.SetAttachment(1);
        bizi.SetAttachment(1);
        zui.SetAttachment(1);
        tou.SetAttachment("tou");
        toufa.SetAttachment(1);

        Pairs = new Dictionary<string, AttachmentControl>();
        Pairs.Add("mianju", mianju);
        Pairs.Add("maozi", maozi);
        Pairs.Add("glasses", glasses);
        Pairs.Add("kouzhao", kouzhao);
        Pairs.Add("yanjing", yanjing);
        Pairs.Add("bizi", bizi);
        Pairs.Add("zui", zui);
        Pairs.Add("toufa", toufa);

        SetSkinColor(CharacterCreation.SkinColors[0]);
        SetHairColor(CharacterCreation.HairColors[0]);

        // reset some shit
        AttachmentControl ac = new AttachmentControl("1han", skeleton, "");
        ac = new AttachmentControl("7han", skeleton, "");
        ac = new AttachmentControl("6han", skeleton, "");
        ac = new AttachmentControl("5han", skeleton, "");
        ac = new AttachmentControl("4han", skeleton, "");
        ac = new AttachmentControl("3han", skeleton, "");
        ac = new AttachmentControl("2han", skeleton, "");
        ac = new AttachmentControl("4xing", skeleton, "");
        ac = new AttachmentControl("3xing", skeleton, "");
        ac = new AttachmentControl("2xing", skeleton, "");
        ac = new AttachmentControl("1xing", skeleton, "");
        ac = new AttachmentControl("xin3", skeleton, "");
        ac = new AttachmentControl("xin2", skeleton, "");
        ac = new AttachmentControl("xin1", skeleton, "");
        ac = new AttachmentControl("biaoqing1", skeleton, "");
        ac = new AttachmentControl("bianse1", skeleton, "");
        ac = new AttachmentControl("texiao", skeleton, "");
        ac = new AttachmentControl("zui1", skeleton, "");

        CharacterCreation.Instance.InitAllPanels();
    }

    public void ChangeSkin(int index)
    {
        SkeletonGraphic.Skeleton.ClearSkin();
        SkeletonGraphic.Skeleton.SetSkin("fushi" + index);
        //SkeletonGraphic.Skeleton.SetSlotsToSetupPose();
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

    public void SetHairColor(Color color)
    {
        SkeletonGraphic.Skeleton.FindSlot("toufahoumian").SetColor(color);
        SkeletonGraphic.Skeleton.FindSlot("toufa1").SetColor(color);
    }

    public Color GetHairColor()
    {
        return SkeletonGraphic.Skeleton.FindSlot("toufa1").GetColor();
    }

    public void SetHead(string head)
    {
        tou.SetAttachment(head);
        CharacterCreation.Instance.ResetHairElements();
    }

    public string GetHead()
    {
        return SkeletonGraphic.Skeleton.FindSlot("tou").Attachment.Name;
    }
}
