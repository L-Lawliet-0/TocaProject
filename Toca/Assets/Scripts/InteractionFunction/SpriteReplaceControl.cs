using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpriteReplaceControl : TocaFunction
{
    private SpriteRenderer m_Renderer;
    public Sprite[] SpriteBanks;
    private int Index;
    void Start()
    {
        m_Renderer = GetComponent<SpriteRenderer>();
        Index = 0;
        TouchControl tc = (TouchControl)TocaObject.GetTocaFunction<TouchControl>();
        tc.ClickCallBacks.Add(ReplaceSprite);
    }

    public void ReplaceSprite()
    {
        Index = Index + 1 >= SpriteBanks.Length ? 0 : Index + 1;
        m_Renderer.sprite = SpriteBanks[Index];
    }
}
