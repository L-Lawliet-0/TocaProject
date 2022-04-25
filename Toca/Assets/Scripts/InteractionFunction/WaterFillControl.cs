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

    private void OnParticleCollision(GameObject other)
    {
        if (!Filled)
        {
            Filled = true;
            StartCoroutine("FillCup");
            GetComponent<Collider2D>().isTrigger = true;
        }
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.F) && !Filled)
        {
            Filled = true;
            StartCoroutine("FillCup");
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
}
