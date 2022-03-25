using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
 * layer control is used to control renderering layer of the object
 * 
 * 
 */
public class LayerControl : TocaFunction
{
    public enum SortingLayers
    {
        Background,
        Objects,
        Selection
    }
    public SortingLayers DefaultObjectLayer;
    public SortingLayers CurrentObjectLayer { get; private set; }

    public int OrderValue { get; private set; }

    SpriteRenderer[] AllRenderers;
    int[] DefaultValues;
    
    private void Start()
    {
        AllRenderers = GlobalParameter.GetComponentAndChildren<SpriteRenderer>(TocaObject.transform);
        DefaultValues = new int[AllRenderers.Length];
        for (int i = 0; i < AllRenderers.Length; i++)
        {
            DefaultValues[i] = AllRenderers[i].sortingOrder;
        }

        ResetLayer(DefaultObjectLayer, false, false);

        /*
        TouchHandler touch = (TouchHandler)TocaObject.GetTocaFunction<TouchHandler>();
        if (touch)
        {
            touch.DetouchCallbacks.Add(DetouchCallback);
            touch.OntouchCallbacks.Add(TouchCallback);
        }
        */
    }

    public void TouchCallback(Vector3 pos)
    {
        ResetLayer(SortingLayers.Selection, false, false);
    }

    public void DetouchCallback()
    {
        ResetLayer(DefaultObjectLayer, true, true);
    }

    public void ResetLayer(SortingLayers defaultLayer, bool inheirtLayer, bool inheirtValue)
    {
        // reset object layer based on all information of this object

        // 1. check if this object is currently attached on another Toca object
        SortingLayers myLayer = defaultLayer;
        int baseOrder = CalculateBaseLayerOrder();

        MoveControl mc = (MoveControl)TocaObject.GetTocaFunction<MoveControl>();
        LayerControl parentLayer = null;
        if (mc && mc.SnapTransform && mc.SnapTransform.parent.GetComponent<BaseControl>().TocaObject)
            parentLayer = (LayerControl)mc.SnapTransform.parent.GetComponent<BaseControl>().TocaObject.GetTocaFunction<LayerControl>();
        if (mc && mc.SnapTransform && parentLayer)
        {
            if (inheirtLayer)
                myLayer = parentLayer.CurrentObjectLayer;
            if (inheirtValue)
                baseOrder = parentLayer.OrderValue + 10; // my order has to be on top of the parenting object
        }

        SetLayer(myLayer, baseOrder);

        // after set layer of this current object, if there's attachment on this object, reset their layers as well
        List<TocaFunction> baseControls = TocaObject.GetTocaFunctions<BaseControl>();
        foreach (TocaFunction toca in baseControls)
        {
            BaseControl bc = (BaseControl)toca;
            foreach (FindControl fc in bc.Attachments)
            {
                LayerControl lc = (LayerControl)fc.TocaObject.GetTocaFunction<LayerControl>();
                lc.ResetLayer(lc.CurrentObjectLayer, true, true);
            }
        }
    }

    public void SetLayer(SortingLayers layer, int order)
    {
        for (int i = 0; i < AllRenderers.Length; i++)
        {
            AllRenderers[i].sortingLayerName = layer.ToString();
            AllRenderers[i].sortingOrder = order + DefaultValues[i];
            if (transform.name.Equals("can1"))
                Debug.LogError("sorting order: " + AllRenderers[i].sortingOrder);
        }
        OrderValue = order;
        CurrentObjectLayer = layer;

        if (transform.name.Equals("can1"))
            Debug.LogError("can1 layer is set to: " + CurrentObjectLayer.ToString() + " : " + OrderValue);
    }

    public int CalculateBaseLayerOrder()
    {
        // this order is based on height(depth) of the object bottom
        return -(int)(TocaObject.Bottom.position.y * 1000);
    }
}
