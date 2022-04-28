using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SleepFX : MonoBehaviour
{
    private void OnEnable()
    {
        StartCoroutine("Zloop");
    }
    private IEnumerator Zloop()
    {
        StartCoroutine("loop", transform.GetChild(0));
        yield return new WaitForSeconds(.2f);

        StartCoroutine("loop", transform.GetChild(1));
        yield return new WaitForSeconds(.2f);

        StartCoroutine("loop", transform.GetChild(2));
    }

    private IEnumerator loop(Transform tran)
    {
        float sign = 1;
        float scale = 0;
        while (true)
        {
            scale += Time.deltaTime * sign;
            if (sign > 0 && scale > .99f)
                sign = -sign;
            else if (sign < 0 && scale < .01f)
                sign = -sign;

            GlobalParameter.SetGlobalScale(tran, Vector3.one * scale);
            yield return null;
        }
    }
}
