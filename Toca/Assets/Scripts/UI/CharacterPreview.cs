using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Spine;
using Spine.Unity;

/// <summary>
/// this script is used for character preview in character selection panel
/// </summary>
public class CharacterPreview : MonoBehaviour
{
    private SkeletonGraphic SkeletonGraphic;
    private Skeleton Skeleton;
    public AttachmentControl mianju, maozi, glasses, kouzhao, yanjing, bizi, zui, tou, toufa, toufahoumian;

    private void Start()
    {
        SkeletonGraphic = GetComponent<SkeletonGraphic>();
        Skeleton = SkeletonGraphic.Skeleton;

        mianju = new AttachmentControl("mianju1", Skeleton, "mianju");
        maozi = new AttachmentControl("maozi1", Skeleton, "maozi");
        glasses = new AttachmentControl("glasses1", Skeleton, "glasses");
        kouzhao = new AttachmentControl("kouzhao1", Skeleton, "kouzhao");
        yanjing = new AttachmentControl("yanjing", Skeleton, "yanjing");
        bizi = new AttachmentControl("bizi", Skeleton, "bizi");
        zui = new AttachmentControl("zui2", Skeleton, "zui");
        tou = new AttachmentControl("tou", Skeleton, "");
        toufa = new AttachmentControl("toufa1", Skeleton, "toufa");
        toufahoumian = new AttachmentControl("toufahoumian", Skeleton, "toufahoumian");

        AttachmentControl ac = new AttachmentControl("1han", Skeleton, "");
        ac = new AttachmentControl("7han", Skeleton, "");
        ac = new AttachmentControl("6han", Skeleton, "");
        ac = new AttachmentControl("5han", Skeleton, "");
        ac = new AttachmentControl("4han", Skeleton, "");
        ac = new AttachmentControl("3han", Skeleton, "");
        ac = new AttachmentControl("2han", Skeleton, "");
        ac = new AttachmentControl("4xing", Skeleton, "");
        ac = new AttachmentControl("3xing", Skeleton, "");
        ac = new AttachmentControl("2xing", Skeleton, "");
        ac = new AttachmentControl("1xing", Skeleton, "");
        ac = new AttachmentControl("xin3", Skeleton, "");
        ac = new AttachmentControl("xin2", Skeleton, "");
        ac = new AttachmentControl("xin1", Skeleton, "");
        ac = new AttachmentControl("biaoqing1", Skeleton, "");
        ac = new AttachmentControl("bianse1", Skeleton, "");
        ac = new AttachmentControl("texiao", Skeleton, "");
        ac = new AttachmentControl("zui1", Skeleton, "");
    }

    public void UpdateCharacter(CharacterData data)
    {
        mianju.SetAttachment(data.ID_mianju);
        maozi.SetAttachment(data.ID_maozi);
        glasses.SetAttachment(data.ID_glasses);
        kouzhao.SetAttachment(data.ID_kouzhao);
        yanjing.SetAttachment(data.ID_yanjing);
        bizi.SetAttachment(data.ID_bizi);
        zui.SetAttachment(data.ID_zui);
        toufa.SetAttachment(data.ID_toufa);
        toufahoumian.SetAttachment(data.ID_toufahoumian);

        SetSkinColor(CharacterCreation.SkinColors[data.ID_skinColor]);
        SetHairColor(CharacterCreation.HairColors[data.ID_hairColor]);
        ChangeSkin(data.ID_skin);

        tou.SetAttachment(data.ID_tou);

        Skeleton.SetAttachment("maozi40", data.ID_toufa == 40 ? "maozi40" : null);
        Skeleton.SetAttachment("hudeijie", data.ID_toufa == 24 ? "hudeijie" : null);
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
}
