using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WaterFillControl : TocaFunction
{
    private SpriteRenderer m_SpriteRenderer;
    private bool Filled;
    public bool WaterFilled { get { return m_SpriteRenderer.size.y > 0; } }
    private float TargetWidth, TargetHeight;
    private void Start()
    {
        m_SpriteRenderer = GetComponent<SpriteRenderer>();
        TargetWidth = m_SpriteRenderer.size.x;
        TargetHeight = m_SpriteRenderer.size.y;
        m_SpriteRenderer.size = Vector2.zero;
        Filled = false;
    }

    public void Fill(Color color)
    {
        if (!Filled)
        {
            m_SpriteRenderer.color = color;
            Filled = true;
            StartCoroutine("FillCup");
        }
    }

    public void UnFill()
    {
        if (Filled)
        {
            Filled = false;
            StartCoroutine("ClearCup");       
        }
    }

    private void Update()
    {
        if (Filled)
        {
            Collider2D[] colliders = Physics2D.OverlapCircleAll(transform.position + TargetHeight * Vector3.up, .1f, 1 << LayerMask.NameToLayer("Base"));
            foreach (Collider2D collider in colliders)
            {
                if (collider && collider.GetComponent<BaseControl>() && collider.GetComponent<BaseControl>().MyBaseAttributes.IsMouth)
                {
                    UnFill();
                    if (collider.GetComponent<EatControl>())
                        collider.GetComponent<EatControl>().PendingWater = this;
                    break;
                }
            }
        }
    }

    private IEnumerator FillCup()
    {
        float initX = .3f;
        float time = 1f;
        float x_speed = (TargetWidth - initX) / time;
        float y_speed = TargetHeight / time;

        m_SpriteRenderer.size = new Vector2(initX, 0);

        while (time > 0)
        {
            m_SpriteRenderer.size += Vector2.right * Time.deltaTime * x_speed;
            m_SpriteRenderer.size += Vector2.up * Time.deltaTime * y_speed;
            time -= Time.deltaTime;
            yield return null;
        }

        m_SpriteRenderer.size = new Vector2(TargetWidth, TargetHeight);
    }

    private IEnumerator ClearCup()
    {
        float time = 1f;
        float x_speed = -TargetWidth / time;
        float y_speed = -TargetHeight / time;

        while (time > 0)
        {
            m_SpriteRenderer.size += Vector2.right * Time.deltaTime * x_speed;
            m_SpriteRenderer.size += Vector2.up * Time.deltaTime * y_speed;
            time -= Time.deltaTime;
            TocaObject.transform.eulerAngles += Vector3.forward * Time.deltaTime * 30;
            yield return null;
        }

        m_SpriteRenderer.size = Vector2.zero;
        TocaObject.transform.eulerAngles = Vector3.zero;
    }
}
