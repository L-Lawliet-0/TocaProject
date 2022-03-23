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
    private void Awake()
    {
        // find all toca functions attached to this toca object
        TocaFunction[] functions1 = GetComponents<TocaFunction>();
        TocaFunction[] functions2 = GetComponentsInChildren<TocaFunction>();

        AllFunctions = new TocaFunction[functions1.Length + functions2.Length];
        int i = 0;
        for (; i < functions1.Length; i++)
            AllFunctions[i] = functions1[i];
        for (; i < AllFunctions.Length; i++)
            AllFunctions[i] = functions2[i - functions1.Length];
        foreach (TocaFunction function in AllFunctions)
            function.TocaObject = this;
    }

    public TocaFunction GetTocaFunction(Type type)
    {
        foreach (TocaFunction function in AllFunctions)
        {
            if (type == function.GetType())
                return function;
        }

        return null;
    }
}
