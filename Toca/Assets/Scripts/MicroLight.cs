using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Experimental.Rendering.Universal;

public class MicroLight : TocaFunction
{
    private BaseControl BaseControl;
    private Light2D Light2D;
    private bool Blinking;

    private GameObject SFX;

    private void Awake()
    {
        Light2D = GetComponent<Light2D>();
        Light2D.enabled = false;
    }

    private void Start()
    {
        BaseControl = (BaseControl)TocaObject.GetTocaFunction<BaseControl>();
        Blinking = false;
    }

    private void Update()
    {
        if (!Blinking && BaseControl.Attachments.Count > 0)
        {
            Blinking = true;
            StartCoroutine("Blink");
            SFX = SoundManager.Instance.PlaySFX(11, false, TocaObject.transform.position);
        }
        else if (Blinking && BaseControl.Attachments.Count == 0)
        {
            StopBlink();
        }
    }

    private void StopBlink()
    {
        Destroy(SFX);
        StopAllCoroutines();
        Blinking = false;
        Light2D.enabled = false;
    }

    private void OnDisable()
    {
        StopBlink();
    }

    private IEnumerator Blink()
    {
        Light2D.enabled = true;
        float r = Light2D.color.r;
        //float g = Light2D.color.g;
        float sign = 1;

        while (true)
        {
            r += sign * Time.deltaTime;
            //g -= sign * Time.deltaTime;

            if (sign > 0 && r > .99f)
                sign = -sign;
            else if (sign < 0 && r < .1f)
                sign = -sign;

            Light2D.color = new Color(r, 0, 0, 1);

            yield return null;
        }
    }
}
