using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StateChangeControl : TocaFunction
{
    public Sprite SelectionSprite, StackSprite, HangSprite;
    private SpriteRenderer SpriteRenderer;
    private FindControl FindControl;

    private void Start()
    {
        TouchControl tc = (TouchControl)TocaObject.GetTocaFunction<TouchControl>();
        tc.TouchCallBacks.Add(OnSelection);
        tc.DeTouchCallBacks.Add(Deselection);
        SpriteRenderer = GetComponent<SpriteRenderer>();
        FindControl = (FindControl)TocaObject.GetTocaFunction<FindControl>();
    }
    public void OnSelection()
    {
        SpriteRenderer.sprite = SelectionSprite;
        RecreateCollider();
    }

    public void Deselection()
    {
        if (FindControl.CurrentAttachment && FindControl.CurrentAttachment.MyBaseAttributes.IsHanger)
            SpriteRenderer.sprite = HangSprite;
        else
            SpriteRenderer.sprite = StackSprite;
        RecreateCollider();
    }

    private void RecreateCollider()
    {
        Destroy(GetComponent<PolygonCollider2D>());
        gameObject.AddComponent<PolygonCollider2D>();
    }
}
