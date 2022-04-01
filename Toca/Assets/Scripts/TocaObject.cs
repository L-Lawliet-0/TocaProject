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
    }
    public ObjectSaveData TocaSave;

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

    public TocaFunction[] AllFunctions;
    public Transform Bottom;
    private void Awake()
    { 
        AllFunctions = GlobalParameter.GetComponentAndChildren<TocaFunction>(transform);
        foreach (TocaFunction function in AllFunctions)
            function.TocaObject = this;

        if (transform.childCount > 0)
            Bottom = transform.GetChild(0);
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
}
