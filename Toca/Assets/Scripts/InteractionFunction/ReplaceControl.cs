using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ReplaceControl : TocaFunction
{
    private bool Replaced;
    public GameObject ReplaceObject;
    public GameObject[] ObjectsPool;
    public int GenerateCount = 1;
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

            for (int i = 0; i < GenerateCount; i++)
            {
                GameObject obj;
                if (ObjectsPool != null && ObjectsPool.Length > 0)
                {
                    obj = Instantiate(ObjectsPool[Random.Range(0, ObjectsPool.Length)]);
                }
                else
                    obj = Instantiate(ReplaceObject);
                obj.transform.position = transform.position + new Vector3(Random.Range(-.1f, .1f), Random.Range(-.1f, .1f));
                MoveControl mc = obj.GetComponent<MoveControl>();
                if (mc)
                    mc.TargetPosition = obj.transform.position;
                obj.SetActive(false);


                StartCoroutine("DelayInit", obj);
            }

            Destroy(gameObject, .2f);
        }
    }

    private IEnumerator DelayInit(GameObject obj)
    {
        yield return new WaitForSeconds(.1f);
        
        obj.SetActive(true);
        TouchControl tc = obj.GetComponent<TouchControl>();
        tc.OnTouch(obj.transform.position);
        yield return null;
        tc.OnDeTouch();
        
    }
}
