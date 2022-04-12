using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SlideControl : TocaFunction
{
    public Transform SlideTrack;
    private BaseControl BaseControl;
    private Dictionary<FindControl, int> Sliders;
    private void Start()
    {
        Sliders = new Dictionary<FindControl, int>();
        BaseControl = (BaseControl)TocaObject.GetTocaFunction<BaseControl>();
    }

    private void Update()
    {
        List<FindControl> remove = new List<FindControl>();
        List<FindControl> sliderKeys = new List<FindControl>(Sliders.Keys);
        // add new attachment to the list
        foreach (FindControl f in BaseControl.Attachments.Keys)
        {
            if (!Sliders.ContainsKey(f))
            {
                // add new object to slider
                // find closest track position
                float min = float.MaxValue;
                int track = 0;
                for (int i = 0; i < SlideTrack.childCount; i++)
                {
                    float dis = Vector2.Distance(SlideTrack.GetChild(i).position, f.transform.position);
                    if (dis < min)
                    {
                        track = i;
                        min = dis;
                    }
                }
                Sliders.Add(f, track);
            }
        }

        // remove old shit that leaves the collection
        foreach (FindControl f in sliderKeys)
        {
            if (!BaseControl.Attachments.ContainsKey(f))
                remove.Add(f);
        }

        // move position
        for (int i = 0; i < sliderKeys.Count; i ++)
        {
            MoveControl mc = (MoveControl)sliderKeys[i].TocaObject.GetTocaFunction<MoveControl>();
            mc.UpdateTargetPosition(SlideTrack.GetChild(Sliders[sliderKeys[i]]).position);

            Debug.LogError("Updating position!");

            if (Vector3.Distance(mc.TargetPosition, mc.transform.position) < .1f)
            {
                if (Sliders[sliderKeys[i]] == SlideTrack.childCount - 1)
                {
                    remove.Add(sliderKeys[i]);
                }
                else
                {
                    Sliders[sliderKeys[i]]++;
                }
            }
        }

        // remove finished find control
        foreach (FindControl f in remove) 
        {
            f.TryDetach();
            Sliders.Remove(f);
        }
    }
}
