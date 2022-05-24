using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DancingPadControl : TocaFunction
{
    private bool ManStanding;
    private BaseControl BaseControl;
    void Start()
    {
        ManStanding = false;
        BaseControl = (BaseControl)TocaObject.GetTocaFunction<BaseControl>();
    }

    private void Update()
    {
        if (!ManStanding)
        {
            foreach (FindControl fc in BaseControl.Attachments.Keys)
            {
                if (fc.IsHuman)
                {
                    BlinkFX.SetActive(false);
                    StopAllCoroutines();
                    ManStanding = true;
                    StartCoroutine("Dance");
                    break;
                }
            }
        }
        else
        {
            bool breakOut = true;
            foreach (FindControl fc in BaseControl.Attachments.Keys)
            {
                if (fc.IsHuman)
                    breakOut = false;
            }

            if (breakOut)
            {
                BlinkFX.SetActive(false);
                StopAllCoroutines();
                ManStanding = false;
                StartCoroutine("StopDance");
            }
        }
    }

    public SpriteRenderer YellowLight;
    public GameObject BlinkFX;
    private IEnumerator Dance()
    {
        while (YellowLight.color.a < .99f)
        {
            YellowLight.color += Color.black * Time.deltaTime;
            yield return null;
        }
        YellowLight.color = Color.white;

        while (true)
        {
            BlinkFX.SetActive(!BlinkFX.activeInHierarchy);
            yield return new WaitForSeconds(.2f);
        }
    }

    private IEnumerator StopDance()
    {
        while (YellowLight.color.a > .01f)
        {
            YellowLight.color -= Color.black * Time.deltaTime;
            yield return null;
        }
        YellowLight.color = new Color(1, 1, 1, 0);
    }
}
