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

            //Test code!!!!
            if (DefaultValues[i] > 10)
                DefaultValues[i] = 0;
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

        FindControl find = (FindControl)TocaObject.GetTocaFunction<FindControl>();
        LayerControl parentLayer = null;
        if (find && find.CurrentAttachment && find.CurrentAttachment.TocaObject)
            parentLayer = (LayerControl)find.CurrentAttachment.TocaObject.GetTocaFunction<LayerControl>();

        if (parentLayer)
        {
            if (inheirtLayer)
                myLayer = parentLayer.CurrentObjectLayer;
            if (inheirtValue)
            {
                //baseOrder = parentLayer.OrderValue + 10; // my order has to be on top of the parenting object
                baseOrder = parentLayer.OrderValue + (int)((15.36f - TocaObject.Bottom.transform.position.y) * 10);

            }
        }

        SetLayer(myLayer, baseOrder);

        // after set layer of this current object, if there's attachment on this object, reset their layers as well
        List<TocaFunction> baseControls = TocaObject.GetTocaFunctions<BaseControl>();
        foreach (TocaFunction toca in baseControls)
        {
            BaseControl bc = (BaseControl)toca;
            
            foreach (KeyValuePair<FindControl, BaseControl.AttachData> pair in bc.Attachments)
            {
                LayerControl lc = (LayerControl)pair.Key.TocaObject.GetTocaFunction<LayerControl>();
                if (lc)
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
        float y = TocaObject.Bottom.position.y;
        FindControl find = (FindControl)TocaObject.GetTocaFunction<FindControl>();
        if (find && find.CurrentAttachment)
        {
            y = find.CurrentAttachment.GetTargetPos(find, find.CurrentAttachment.Attachments[find]).y;
        }

        return -(int)(y * 1000);
    }
}
