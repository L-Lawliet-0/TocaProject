using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IslandSelectionControl : TocaFunction
{
    public int SceneIndex;
    // Start is called before the first frame update
    void Start()
    {
        GetComponent<TouchControl>().ClickCallBacks.Add(Click);
    }

    public void Click()
    {
        LoadingCtrl.Instance.LoadScene(SceneIndex);
    }
}
