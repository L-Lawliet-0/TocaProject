using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UFOcontrol : TocaFunction
{
    public GameObject[] Lights;
    public bool OnTop;
    public bool Moving;
    private Vector3 TopPos;
    // Start is called before the first frame update
    void Start()
    {
        OnTop = true;
        Moving = false;
        TouchControl tc = (TouchControl)TocaObject.GetTocaFunction<TouchControl>();
        tc.ClickCallBacks.Add(Interact);

        TopPos = TocaObject.transform.position;
    }

    public void Interact()
    {
        if (!Moving)
        {
            Moving = true;
            StartCoroutine("Move");
        }
    }

    private IEnumerator Move()
    {
        foreach (GameObject obj in Lights)
            obj.SetActive(OnTop);
        int sign = OnTop ? -1 : 1;
        float counter = 2;
        float distance = 2.5f;
        float speed = distance / counter;

        while (counter > 0)
        {
            TocaObject.transform.position += Vector3.up * Time.deltaTime * speed * sign;
            counter -= Time.deltaTime;
            yield return null;
        }
        
        OnTop = !OnTop;
        TocaObject.transform.position = OnTop ? TopPos : TopPos - Vector3.up * distance;
        Moving = false;
    }
}
