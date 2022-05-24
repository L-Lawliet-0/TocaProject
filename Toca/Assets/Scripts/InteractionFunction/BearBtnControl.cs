using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BearBtnControl : TocaFunction
{
    private bool Active;
    public GameObject ActiveBtn;
    public SpriteRenderer Highlight;
    void Start()
    {
        Active = false;
        TouchControl tc = (TouchControl)TocaObject.GetTocaFunction<TouchControl>();
        tc.ClickCallBacks.Add(BtnPress);
    }

    public void BtnPress()
    {
        StopAllCoroutines();
        Active = !Active;
        StartCoroutine("Act");
    }

    private IEnumerator Act()
    {
        ActiveBtn.SetActive(Active);
        
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
    }
}
