using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ClothControl : TocaFunction
{
    public int ClothIndex;
    public bool ClothFolded = true;
    private SpriteRenderer m_SpriteRenderer;
    private FindControl m_FindControl;
    private BaseControl m_BaseControl;

    public bool Stacking { get { return m_BaseControl.Attachments.Count > 0 || (m_FindControl.CurrentAttachment && m_FindControl.CurrentAttachment.MyBaseAttributes.IsCloth); } }

    private void Start()
    {
        TouchControl tc = (TouchControl)TocaObject.GetTocaFunction<TouchControl>();
        tc.TouchCallBacks.Add(OnSelection);
        tc.DeTouchCallBacks.Add(OnDeselection);
        m_SpriteRenderer = GetComponent<SpriteRenderer>();
        m_FindControl = (FindControl)TocaObject.GetTocaFunction<FindControl>();
        m_BaseControl = (BaseControl)TocaObject.GetTocaFunction<BaseControl>();
    }

    private void Update()
    {
        if (m_FindControl.CurrentAttachment && m_FindControl.CurrentAttachment.MyBaseAttributes.IsBody)
        {
            SoundManager.Instance.PlaySFX(13, true, TocaObject.transform.position);

            Destroy(gameObject);
            SpineControl sc = (SpineControl)m_FindControl.CurrentAttachment.TocaObject.GetTocaFunction<SpineControl>();
            int value = sc.SwapSkin(ClothIndex);
            if (value != 0)
            {
                GameObject SpawnedObject = GlobalParameter.Instance.CreateObject(Resources.Load<GameObject>("Prefabs/" + (305 + value).ToString()), m_FindControl.CurrentAttachment.transform.position);
                TocaObject toca = SpawnedObject.GetComponent<TocaObject>();
                toca.TocaSave.ObjectID = toca.GetHashCode();
            }
        }

        if (m_FindControl.CurrentAttachment && m_FindControl.CurrentAttachment.MyBaseAttributes.IsClothHang && ClothFolded)
            ExpandCloth();
    }

    public void OnSelection()
    {
        if (!Stacking)
            ExpandCloth();
    }

    public void OnDeselection()
    {
        FoldCloth();
        if (m_FindControl.CurrentAttachment && m_FindControl.CurrentAttachment.MyBaseAttributes.IsClothHang)
            ExpandCloth();
    }

    public void FoldCloth()
    {
        ClothFolded = true;
        m_SpriteRenderer.sprite = Resources.Load<Sprite>("Cloth/" + ClothIndex.ToString());
        Destroy(GetComponent<PolygonCollider2D>());
        gameObject.AddComponent<PolygonCollider2D>();
    }

    public void ExpandCloth()
    {
        ClothFolded = false;
        m_SpriteRenderer.sprite = Resources.Load<Sprite>("clothhang/f" + ClothIndex.ToString());
        Destroy(GetComponent<PolygonCollider2D>());
        gameObject.AddComponent<PolygonCollider2D>();
    }
}
