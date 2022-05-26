using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SwapSpriteControl : TocaFunction
{
    public Sprite SwapSprite;
    private SpriteRenderer m_SpriteRenderer;
    private Sprite DefaultSprite;

    private void Start()
    {
        TouchControl tc = (TouchControl)TocaObject.GetTocaFunction<TouchControl>();
        tc.ClickCallBacks.Add(Swap);
        m_SpriteRenderer = GetComponent<SpriteRenderer>();
        DefaultSprite = m_SpriteRenderer.sprite;
    }

    private void Swap()
    {
        m_SpriteRenderer.sprite = m_SpriteRenderer.sprite == DefaultSprite ? SwapSprite : DefaultSprite;
    }
}
