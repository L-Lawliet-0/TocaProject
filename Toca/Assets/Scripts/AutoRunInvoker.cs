using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AutoRunInvoker : MonoBehaviour
{
    public ClawControl ClawControl;
    public Sprite PressedSprite;
    private Sprite DefaultSprite;
    private SpriteRenderer m_SpriteRenderer;
    void Start()
    {
        m_SpriteRenderer = GetComponent<SpriteRenderer>();
        DefaultSprite = m_SpriteRenderer.sprite;
        GetComponent<TouchControl>().ClickCallBacks.Add(ClawControl.AutoRun);
        GetComponent<TouchControl>().ClickCallBacks.Add(Press);
    }

    public void Press()
    {
        m_SpriteRenderer.sprite = PressedSprite;
        Invoke("ChangeBack", .1f);
    }

    private void ChangeBack()
    {
        m_SpriteRenderer.sprite = DefaultSprite;
    }

}
