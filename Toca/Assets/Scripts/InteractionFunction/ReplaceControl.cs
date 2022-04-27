using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ReplaceControl : TocaFunction
{
    private bool Replaced;
    public GameObject ReplaceObject;
    private void Start()
    {
        TouchControl tc = (TouchControl)TocaObject.GetTocaFunction<TouchControl>();
        tc.ClickCallBacks.Add(Replace);
        Replaced = false;
    }

    private void Replace()
    {
        if (!Replaced)
        {
            Replaced = true;

            // play effect
            GameObject fx = Instantiate(GlobalParameter.Instance.RunTimeEffects[4], transform.position, Quaternion.identity);
            Destroy(fx, 1f);

            GameObject obj = Instantiate(ReplaceObject);
            obj.transform.position = transform.position;
            obj.SetActive(false);

            StartCoroutine("DelayInit", obj);
        }
    }

    private IEnumerator DelayInit(GameObject obj)
    {
        yield return new WaitForSeconds(.1f);
        obj.SetActive(true);
        TouchControl tc = obj.GetComponent<TouchControl>();
        tc.OnTouch(transform.position);
        yield return null;
        tc.OnDeTouch();
        Destroy(gameObject);
    }
}
