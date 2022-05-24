using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GenerationControl : TocaFunction
{
    public GameObject[] CandyPrefabs;
    public int MaxCount;
    private int CurrentCount;
    // Start is called before the first frame update
    void Start()
    {
        TouchControl touch = (TouchControl)TocaObject.GetTocaFunction<TouchControl>();
        touch.ClickCallBacks.Add(MakeCandy);
        CurrentCount = 0;
    }

    private void MakeCandy()
    {
        if (CurrentCount >= MaxCount)
            return;
        CurrentCount++;

        GlobalParameter.Instance.CreateObject(CandyPrefabs[Random.Range(0, CandyPrefabs.Length)], transform.position);
    }
}
