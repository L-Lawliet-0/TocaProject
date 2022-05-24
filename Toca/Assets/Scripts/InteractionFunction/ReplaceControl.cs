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
                GameObject prefab;
                if (ObjectsPool != null && ObjectsPool.Length > 0)
                {
                    prefab = ObjectsPool[i % ObjectsPool.Length];
                }
                else
                    prefab = ReplaceObject;
                GlobalParameter.Instance.CreateObject(prefab, transform.position + new Vector3(Random.Range(-.1f, .1f), Random.Range(-.1f, .1f)));
            }

            Destroy(gameObject, .2f);
        }
    }
}
