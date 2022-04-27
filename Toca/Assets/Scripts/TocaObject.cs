using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

/*
 * attach on the root of any gameobject that uses functions 
 * 
 */
public class TocaObject : MonoBehaviour
{
    [System.Serializable]
    public class ObjectSaveData
    {
        public float x, y; // the position of the object
        public int ObjectID; // the unique id of this tocaobject

        public bool Attaching; // this toca object is attaching to another toca object
        public int ParentObjectID; // the toca object this current object is attaching
        public int ParentBaseID; // the basecontrol this current object is attaching
    }
    public ObjectSaveData TocaSave;

    // this function save the positon and toca id to itself
    public void InitalizeSave()
    {
        TocaSave.x = transform.position.x;
        TocaSave.y = transform.position.y;
        TocaSave.ObjectID = this.GetHashCode();
    }

    public void SaveData()
    {
        TocaSave.x = transform.position.x;
        TocaSave.y = transform.position.y;
    }

    // based on the current save, initalize the toca object
    public void InitalizeTocaobject()
    {
        transform.position = new Vector3(TocaSave.x, TocaSave.y, GlobalParameter.Depth);
        if (TocaSave.Attaching && TocaObjectsLoader.Instance.TocaObjectsPool.ContainsKey(TocaSave.ParentObjectID))
        {
            TocaObject toca = TocaObjectsLoader.Instance.TocaObjectsPool[TocaSave.ParentObjectID];
            List<TocaFunction> functions = toca.GetTocaFunctions<BaseControl>();
            foreach (BaseControl function in functions)
            {
                if (function.BaseID == TocaSave.ParentBaseID)
                {
                    // found the mother fucker
                    ((FindControl)GetTocaFunction<FindControl>()).BasePreview = function;
                    ((FindControl)GetTocaFunction<FindControl>()).TryAttach();
                    //function.Attach((FindControl)GetTocaFunction<FindControl>());

                    LayerControl lc = (LayerControl)GetTocaFunction<LayerControl>();
                    if (lc)
                        lc.DetouchCallback();
                    break;
                }
            }
        }
    }

    public TocaFunction[] AllFunctions;
    public Transform Bottom;
    private void Awake()
    { 
        AllFunctions = GlobalParameter.GetComponentAndChildren<TocaFunction>(transform);
        foreach (TocaFunction function in AllFunctions)
            function.TocaObject = this;

        if (transform.childCount > 0)
            Bottom = transform.GetChild(0);
        else
            Bottom = transform;
    }

    public TocaFunction GetTocaFunction<T>()
    {
        Type type = typeof(T);
        foreach (TocaFunction function in AllFunctions)
        {
            if (type == function.GetType())
                return function;
        }

        return null;
    }

    public List<TocaFunction> GetTocaFunctions<T>()
    {
        List<TocaFunction> values = new List<TocaFunction>();
        Type type = typeof(T);
        foreach (TocaFunction function in AllFunctions)
        {
            if (type == function.GetType())
                values.Add(function);
        }
        return values;
    }

    private void OnDestroy()
    {
        FindControl fc = (FindControl)GetTocaFunction<FindControl>();
        if (fc)
            fc.TryDetach();
    }
}
