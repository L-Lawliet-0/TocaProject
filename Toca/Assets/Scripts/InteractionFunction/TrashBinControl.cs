using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TrashBinControl : TocaFunction
{
    private BaseControl BaseControl;
    private Dictionary<MoveControl, bool> Trashes;
    private Vector3 UpPos;

    private void Start()
    {
        BaseControl = (BaseControl)TocaObject.GetTocaFunction<BaseControl>();
        Trashes = new Dictionary<MoveControl, bool>();
    }

    private void Update()
    {
        List<MoveControl> remove = new List<MoveControl>();
        List<MoveControl> moveKeys = new List<MoveControl>(Trashes.Keys);
        // add new attachment to the list
        foreach (FindControl f in BaseControl.Attachments.Keys)
        {
            MoveControl mc = (MoveControl)f.TocaObject.GetTocaFunction<MoveControl>();

            if (!Trashes.ContainsKey(mc))
                Trashes.Add(mc, false);
        }

        // remove old shit that leaves the collection
        foreach (MoveControl mc in moveKeys)
        {
            FindControl fc = (FindControl)mc.TocaObject.GetTocaFunction<FindControl>();
            if (!BaseControl.Attachments.ContainsKey(fc))
                remove.Add(mc);
        }

        foreach (MoveControl mc in remove)
            Trashes.Remove(mc);

        // refresh keys
        moveKeys = new List<MoveControl>(Trashes.Keys);

        if (Trashes.Count > 0)
            Debug.LogError(moveKeys[0].transform.localScale);

        UpPos = transform.position + Vector3.up * 2;

        // move position
        for (int i = 0; i < moveKeys.Count; i++)
        {
            if (Trashes[moveKeys[i]])
            {
                // this trash already reach the top, drop to bottom
                moveKeys[i].UpdateTargetPosition(transform.position);
                moveKeys[i].transform.localScale -= Time.deltaTime * Vector3.one * 5;
                if (moveKeys[i].transform.localScale.x < .01f)
                {
                    Destroy(moveKeys[i].TocaObject.gameObject);
                    Trashes.Remove(moveKeys[i]);
                    FindControl fc = (FindControl)moveKeys[i].TocaObject.GetTocaFunction<FindControl>();
                    BaseControl.Attachments.Remove(fc);
                }
            }
            else
            {
                // this trash hasn't reach the top yet
                LayerControl lc = (LayerControl)moveKeys[i].TocaObject.GetTocaFunction<LayerControl>();
                if (lc)
                    lc.SetLayer(LayerControl.SortingLayers.Selection, lc.OrderValue);
                if (Vector3.Distance(moveKeys[i].transform.position, UpPos) < .1f)
                {
                    Trashes[moveKeys[i]] = true; // this object reaches the top
                    lc.SetLayer(lc.DefaultObjectLayer, lc.OrderValue);
                }
                else
                    moveKeys[i].UpdateTargetPosition(UpPos);
            }
        }
    }
}
