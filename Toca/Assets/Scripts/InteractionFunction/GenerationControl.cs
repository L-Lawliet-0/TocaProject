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

        GameObject candy = Instantiate(CandyPrefabs[Random.Range(0, CandyPrefabs.Length)]); // clone a random new candy
        candy.SetActive(true);
        candy.transform.position = transform.position; //+ new Vector3(Random.Range(-1f, 1f), Random.Range(0f, 2f));
        StartCoroutine("DelayInit", candy);
    }

    private IEnumerator DelayInit(GameObject candy)
    {
        yield return new WaitForSeconds(.1f);
        candy.GetComponent<TouchControl>().OnTouch(candy.transform.position);
        yield return new WaitForSeconds(.1f);
        candy.GetComponent<TouchControl>().OnDeTouch();
    }

}
