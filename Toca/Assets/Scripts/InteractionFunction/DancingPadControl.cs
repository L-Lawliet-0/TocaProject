using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DancingPadControl : TocaFunction
{
    private bool ManStanding;
    private BaseControl BaseControl;
    public GameObject[] ActiveByOrder;
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
                foreach (GameObject obj in ActiveByOrder)
                    obj.SetActive(false);
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

        int index = 0;
        while (true)
        {
            BlinkFX.SetActive(!BlinkFX.activeInHierarchy);
            ActiveByOrder[index].SetActive(true);
            yield return new WaitForSeconds(.2f);
            ActiveByOrder[index].SetActive(false);
            index = index + 1 >= ActiveByOrder.Length ? 0 : index + 1;
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
