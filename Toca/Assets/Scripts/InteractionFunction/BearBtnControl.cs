using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BearBtnControl : TocaFunction
{
    private bool Active;
    public GameObject ActiveBtn;
    public Transform Highlights;
    private Vector3[] DefaultPositions;
    void Start()
    {
        Active = false;
        TouchControl tc = (TouchControl)TocaObject.GetTocaFunction<TouchControl>();
        tc.ClickCallBacks.Add(BtnPress);

        DefaultPositions = new Vector3[Highlights.childCount];
        for (int i = 0; i < Highlights.childCount; i++)
        {
            DefaultPositions[i] = Highlights.GetChild(i).position;
        }
    }

    public void BtnPress()
    {
        StopAllCoroutines();
        Active = !Active;

        ActiveBtn.SetActive(Active);
        for (int i = 0; i < Highlights.childCount; i++)
        {
            StartCoroutine("SetLight", i);
            if (Active)
                StartCoroutine("RandomMove", i);
        }
    }

    private IEnumerator SetLight(int index)
    {
        SpriteRenderer Highlight = Highlights.GetChild(index).GetComponent<SpriteRenderer>();
        
        if (Active)
        {
            while (Highlight.color.a < .99f)
            {
                Highlight.color += Color.black * Time.deltaTime;
                yield return null;
            }
            Highlight.color = Color.white;
        }
        else
        {
            while (Highlight.color.a > .01f)
            {
                Highlight.color -= Color.black * Time.deltaTime;
                yield return null;
            }
            Highlight.color = new Color(1, 1, 1, 0);
        }

        if (!Active)
            Highlight.transform.position = DefaultPositions[index];
    }

    private IEnumerator RandomMove(int index)
    {
        Transform tran = Highlights.GetChild(index);
        float angle = 0;
        Vector3 direction = new Vector3(Random.Range(-1f, 1f), Random.Range(-1f, 1f)).normalized;
        float angleSpeed = Random.Range(90, 180);
        float raidus = Random.Range(.1f, .3f);

        while (true)
        {
            tran.position = DefaultPositions[index] + direction * Mathf.Sin(angle * Mathf.Deg2Rad) * raidus;
            angle += Time.deltaTime * angleSpeed;
            yield return null;
        }
    }
}
