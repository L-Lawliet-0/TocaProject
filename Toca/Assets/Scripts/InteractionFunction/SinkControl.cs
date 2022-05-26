using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SinkControl : MonoBehaviour
{
    public GameObject ActiveAlways;
    public GameObject ObjectTurn;

    private float TargetWidth, TargetHeight;
    private SpriteRenderer m_SpriteRenderer;

    private bool On;
    void Start()
    {
        On = false;
        TouchControl tc = GetComponent<TouchControl>();
        tc.ClickCallBacks.Add(Turn);

        if (ActiveAlways)
        {
            m_SpriteRenderer = ActiveAlways.GetComponent<SpriteRenderer>();
            TargetWidth = m_SpriteRenderer.size.x;
            TargetHeight = m_SpriteRenderer.size.y;
        }
    }

    public void Turn()
    {
        On = !On;

        if (ActiveAlways && !ActiveAlways.activeInHierarchy)
        {
            ActiveAlways.SetActive(true);
            StartCoroutine("FillCup");
        }
        ObjectTurn.SetActive(On);
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
}
