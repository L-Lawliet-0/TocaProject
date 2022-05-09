using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Spine;
using Spine.Unity;
using System;

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

    public int OrderValue { get; set; }

    SpriteRenderer[] AllRenderers;
    SkeletonPartsRenderer[] AllPartsRenderers;
    SkeletonAnimation SkeletonAnimation;
    
    int[] DefaultValues;
    int[] PartsDefaultValues;
    
    private void Start()
    {
        AllRenderers = GlobalParameter.GetComponentAndChildren<SpriteRenderer>(TocaObject.transform);
        AllPartsRenderers = GlobalParameter.GetComponentAndChildren<SkeletonPartsRenderer>(TocaObject.transform);
        SkeletonAnimation = GetComponent<SkeletonAnimation>();
        if (!SkeletonAnimation)
            SkeletonAnimation = GetComponentInChildren<SkeletonAnimation>();
        DefaultValues = new int[AllRenderers.Length];
        PartsDefaultValues = new int[AllPartsRenderers.Length];
        for (int i = 0; i < AllRenderers.Length; i++)
        {
            DefaultValues[i] = AllRenderers[i].sortingOrder;
        }

        for (int i = 0; i < AllPartsRenderers.Length; i++)
        {
            PartsDefaultValues[i] = AllPartsRenderers[i].GetComponent<MeshRenderer>().sortingOrder;
        }

        /*
        BaseControl bc = (BaseControl)TocaObject.GetTocaFunction<BaseControl>();
        if (bc && bc.MyBaseAttributes.HaveCover)
        {
            for (int i = 0; i < AllRenderers.Length; i++)
            {
                if (AllRenderers[i] == bc.Cover)
                {
                    //if (name.Equals("drawer"))
                    //{
                        DefaultValues[i] = (int)(bc.GetYRange() * 100) + 2;
                    //}
                }
            }
        }
        */
        //ResetLayer(DefaultObjectLayer, false, false);
        ResetLayer(false);

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
        ResetLayer(true);
        //ResetLayer(SortingLayers.Selection, false, false);
    }

    public void DetouchCallback()
    {
        ResetLayer(false);
        //ResetLayer(DefaultObjectLayer, true, true);
    }

    /// <summary>
    /// 如果此处进行selection图层更改，即更改当前图层和其attachment下面的sorting layer即可，无需重新计算layer order
    /// 如果此处进行图层的重新计算，遍历到attachments的最上层，进行一次排序计算后即可
    /// 
    /// </summary>
    /// <param name="selection"></param>
    public void ResetLayer(bool selection)
    {
        if (selection)
        {
            // 简单遍历改变selection图层顺序
            SetLayer(SortingLayers.Selection, OrderValue);

            List<TocaFunction> baseControls = TocaObject.GetTocaFunctions<BaseControl>();
            foreach (TocaFunction toca in baseControls)
            {
                BaseControl bc = (BaseControl)toca;

                foreach (KeyValuePair<FindControl, BaseControl.AttachData> pair in bc.Attachments)
                {
                    LayerControl lc = (LayerControl)pair.Key.TocaObject.GetTocaFunction<LayerControl>();
                    if (lc)
                        lc.ResetLayer(true);
                }
            }
        }
        else
        {
            LayerControl upper = GetParent(this); // get the upper most layer control
            List<LayerControl> sortedLayers = SortAllLayers(upper);

            // now the whole list is sorted by "appear in the back" -> "appear in the front" order
            // and we start to assign sorting order values into the layer control
            int baseValue = upper.CalculateBaseLayerOrder(); // we get the base value
            for (int i = 0; i < sortedLayers.Count; i++)
            {
                if (sortedLayers[i])
                {
                    //FindControl fc = (FindControl)sortedLayers[i].TocaObject.GetTocaFunction<FindControl>();
                    //if (fc && fc.CurrentAttachment && fc.CurrentAttachment.MyBaseAttributes.InheirtLayer)
                        sortedLayers[i].SetLayer(sortedLayers[i].DefaultObjectLayer, GetBaseLayer(sortedLayers[i]) + i * 2);
                    //else
                    //    sortedLayers[i].SetLayer(sortedLayers[i].DefaultObjectLayer, sortedLayers[i].CalculateBaseLayerOrder());
                }
            }
        }
    }

    private int GetBaseLayer(LayerControl l)
    {
        FindControl fc = (FindControl)l.TocaObject.GetTocaFunction<FindControl>();

        if (fc && fc.CurrentAttachment && fc.CurrentAttachment.MyBaseAttributes.InheirtLayer)
            return GetBaseLayer((LayerControl)fc.CurrentAttachment.TocaObject.GetTocaFunction<LayerControl>());

        return l.CalculateBaseLayerOrder();
    }

    private LayerControl GetParent(LayerControl l)
    {
        FindControl fc = (FindControl)l.TocaObject.GetTocaFunction<FindControl>();

        if (!fc || !fc.CurrentAttachment)
            return l;

        LayerControl upper = (LayerControl)fc.CurrentAttachment.TocaObject.GetTocaFunction<LayerControl>();
        return GetParent(upper);
    }

    /// <summary>
    /// sort all layers under this attachments and basecontrol
    /// </summary>
    /// <param name="baseLayer"></param>
    /// <returns></returns>
    private List<LayerControl> SortAllLayers(LayerControl baseLayer)
    {
        List<LayerControl> values = new List<LayerControl>();
        values.Add(baseLayer); // this current base layer is in front, so it appears in the back of its object

        // get the base control under this game object
        List<TocaFunction> bcs = baseLayer.TocaObject.GetTocaFunctions<BaseControl>();
        HeightCompare heightCompare = new HeightCompare();
        heightCompare.PureHeight = true;
        bcs.Sort(heightCompare); // sort base control based on their pure height values

        heightCompare.PureHeight = false;
        foreach (BaseControl bc in bcs)
        {
            int countBefore = values.Count;
            List<FindControl> fcs = new List<FindControl>(bc.Attachments.Keys);
            fcs.Sort(heightCompare); // sort find controls

            foreach (FindControl fc in fcs)
            {
                LayerControl lc = (LayerControl)fc.TocaObject.GetTocaFunction<LayerControl>();
                values.AddRange(SortAllLayers(lc)); // add sorted layers
            }
            int count = values.Count - countBefore;
            if (bc.MyBaseAttributes.HaveCover)
            {
                bc.LayerCache = (count + 1) * 2;
                values.Add(null); // add a null object as cover layer
            }
        }

        // sort base control

        return values;
    }

    public class HeightCompare : Comparer<TocaFunction>
    {
        public bool PureHeight = false;
        public override int Compare(TocaFunction x, TocaFunction y)
        {
            float x_y = helper(x);
            float y_y = helper(y);

            if (x_y == y_y)
                return 0;
            else if (x_y < y_y)
                return 1;
            return -1;
        }

        private float helper(TocaFunction t)
        {
            if (PureHeight)
                return t.transform.position.y;

            // this order is based on height(depth) of the object bottom
            float y = t.TocaObject.Bottom.position.y;

            FindControl find = (FindControl)t.TocaObject.GetTocaFunction<FindControl>();
            if (find && find.CurrentAttachment)
            {
                y = find.CurrentAttachment.GetTargetPos(find, find.CurrentAttachment.Attachments[find]).y;

                SpineControl sc = (SpineControl)find.TocaObject.GetTocaFunction<SpineControl>();
                if (sc)
                    y -= sc.HipOffset;
            }

            return y; // return the pending or actual height
        }
    }

    public void SetLayer(SortingLayers layer, int order)
    {
        for (int i = 0; i < AllRenderers.Length; i++)
        {
            AllRenderers[i].sortingLayerName = layer.ToString();
            AllRenderers[i].sortingOrder = order + DefaultValues[i];
        }

        for (int i = 0; i < AllPartsRenderers.Length; i++)
        {
            AllPartsRenderers[i].GetComponent<MeshRenderer>().sortingLayerName = layer.ToString();
            AllPartsRenderers[i].GetComponent<MeshRenderer>().sortingOrder = order + PartsDefaultValues[i];
        }

        if (SkeletonAnimation)
        {
            SkeletonAnimation.GetComponent<MeshRenderer>().sortingLayerName = layer.ToString();
            SkeletonAnimation.GetComponent<MeshRenderer>().sortingOrder = order;
        }

        OrderValue = order;
        CurrentObjectLayer = layer;


        BaseControl bc = (BaseControl)TocaObject.GetTocaFunction<BaseControl>();
        if (bc && bc.MyBaseAttributes.HaveCover)
        {
            if (bc.Cover)
                bc.Cover.sortingOrder = OrderValue + bc.LayerCache;
            if (bc.Cover2)
                bc.Cover2.sortingOrder = OrderValue + bc.LayerCache;
        }
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

        //y = (int)(y * 10); // take consideration of only 1 decimal pint

        // y is in range of 0 ~ 16
        // so .1 = 100
        // so base layer is in range -16000 ~ 0
        return -(int)(y * 1000);
    }
}
