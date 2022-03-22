using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
 * touch handler is used to handle touch and stop touching event from input system
 */
public class TouchHandler : MonoBehaviour
{
    public List<V3callback> OntouchCallbacks, OnpositionchangeCallbacks;
    public List<Voidcallback> DetouchCallbacks;
    private void Awake()
    {
        OntouchCallbacks = new List<V3callback>();
        OnpositionchangeCallbacks = new List<V3callback>();
        DetouchCallbacks = new List<Voidcallback>();
    }

    // need a collider to detech input
    public void OnTouch(Vector3 touchPos)
    {
        foreach (V3callback callback in OntouchCallbacks)
            callback(touchPos);
    }

    public void OnDeTouch()
    {
        foreach (Voidcallback callback in DetouchCallbacks)
            callback();
    }

    public void OnTouchPositionChanged(Vector3 worldPosition)
    {
        foreach (V3callback callback in OnpositionchangeCallbacks)
            callback(worldPosition);
    }

    public delegate void V3callback(Vector3 v3);
    public delegate void Voidcallback();
}