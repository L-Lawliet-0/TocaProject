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
    public TocaFunction[] AllFunctions;
    public Transform Bottom;
    private void Awake()
    { 
        AllFunctions = GlobalParameter.GetComponentAndChildren<TocaFunction>(transform);
        foreach (TocaFunction function in AllFunctions)
            function.TocaObject = this;
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
