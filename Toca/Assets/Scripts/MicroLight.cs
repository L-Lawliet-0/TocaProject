using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Experimental.Rendering.Universal;

public class MicroLight : MonoBehaviour
{
    private Light2D Light2D;
    private void Awake()
    {
        Light2D = GetComponent<Light2D>();
    }

    private void OnEnable()
    {
        StartCoroutine("Blink");
    }

    private IEnumerator Blink()
    {
        float r = Light2D.color.r;
        float g = Light2D.color.g;
        float sign = 1;

        while (true)
        {
            r += sign * Time.deltaTime;
            g -= sign * Time.deltaTime;

            if (sign > 0 && r > .99f)
                sign = -sign;
            else if (sign < 0 && r < .1f)
                sign = -sign;

            Light2D.color = new Color(r, g, 0, 1);

            yield return null;
        }
    }
}
