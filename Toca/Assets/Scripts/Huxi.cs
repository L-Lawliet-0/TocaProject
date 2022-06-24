using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Huxi : MonoBehaviour
{
    private void OnEnable()
    {
        StartCoroutine("huxi");
    }

    private IEnumerator huxi()
    {
        yield return new WaitForSeconds(4);
        float counter = 0;

        while (counter < 1)
        {
            transform.localScale += Time.deltaTime * Vector3.one;
            counter += Time.deltaTime;
            yield return null;
        }

        counter = 0;

        while (true)
        {
            transform.localScale = Vector3.one * 1 + Vector3.one * Mathf.Sin(counter) * .1f; 
            counter += Time.deltaTime;
            yield return null;
        }
    }
}
