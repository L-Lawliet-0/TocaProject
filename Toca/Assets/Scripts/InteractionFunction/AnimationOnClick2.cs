using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimationOnClick2 : TocaFunction
{
    void Start()
    {
        TouchControl tc = (TouchControl)TocaObject.GetTocaFunction<TouchControl>();
        tc.ClickCallBacks.Add(OnClick);
    }

    public void OnClick()
    {
        gameObject.SetActive(!gameObject.activeInHierarchy);
    }
}
