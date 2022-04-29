using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WaterFillControl : TocaFunction
{
    private SpriteRenderer m_SpriteRenderer;
    private bool Filled;
    private void Start()
    {
        m_SpriteRenderer = GetComponent<SpriteRenderer>();
        m_SpriteRenderer.size = Vector2.zero;
        Filled = false;
    }

    public void Fill()
    {
        if (!Filled)
        {
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
            Collider2D collider = Physics2D.OverlapCircle(transform.position + .68f * Vector3.up, .5f, 1 << LayerMask.NameToLayer("Base"));
            if (collider && collider.GetComponent<BaseControl>() && collider.GetComponent<BaseControl>().MyBaseAttributes.IsMouth)
            {
                UnFill();
            }
        }
    }

    private IEnumerator FillCup()
    {
        float initX = .3f;
        float targetWidth = .6f;
        float targetHeigth = .68f;
        float time = .5f;
        float x_speed = (targetWidth - initX) / time;
        float y_speed = targetHeigth / time;

        m_SpriteRenderer.size = new Vector2(initX, 0);

        while (time > 0)
        {
            m_SpriteRenderer.size += Vector2.right * Time.deltaTime * x_speed;
            m_SpriteRenderer.size += Vector2.up * Time.deltaTime * y_speed;
            time -= Time.deltaTime;
            yield return null;
        }

        m_SpriteRenderer.size = new Vector2(targetWidth, targetHeigth);
    }

    private IEnumerator ClearCup()
    {
        float time = .5f;
        float x_speed = -.6f / time;
        float y_speed = -.68f / time;

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
