using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// a tempoary loader to load and save objects within a scene
[ExecuteInEditMode]
public class TocaObjectsLoader : MonoBehaviour
{
    public bool INITDATA;
    public bool FirstFrame = true;
    public void InitializeAllTocaObjects()
    {
        for (int i = 0; i < transform.childCount; i++)
        {
            TocaObject obj = transform.GetChild(i).GetComponent<TocaObject>();
            obj.InitalizeSave();
        }
    }

    private void LoadAllTocaObject()
    {
        for (int i = transform.childCount - 1; i >= 0; i--)
        {
            TocaObject obj = transform.GetChild(i).GetComponent<TocaObject>();
            TouchControl touch = (TouchControl)obj.GetTocaFunction<TouchControl>();
            if (touch)
                touch.OnDeTouch();
        }
    }

    private void Start()
    {
        if (Application.isPlaying)
            FirstFrame = true;
    }

    private void Update()
    {
        if (INITDATA)
        {
            InitializeAllTocaObjects();
            INITDATA = false;
        }

        if (FirstFrame && Application.isPlaying)
        {
            FirstFrame = false;
            LoadAllTocaObject();
        }
    }
}
