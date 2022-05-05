using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SlideControl : TocaFunction
{
    private class SlideData
    {
        public int TrackIndex;
        public Vector3 OverridePos;

        public SlideData(int index)
        {
            TrackIndex = index;
        }
    }
    public Transform SlideTrack;
    private BaseControl BaseControl;
    private Dictionary<FindControl, SlideData> Sliders;
    private void Start()
    {
        Sliders = new Dictionary<FindControl, SlideData>();
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
                Sliders.Add(f, new SlideData(track));
                if (Sliders[f].TrackIndex == SlideTrack.childCount - 1)
                {
                    Sliders[f].OverridePos = SlideTrack.GetChild(Sliders[f].TrackIndex).position + new Vector3(Random.Range(0f,2f), Random.Range(-1f, 0));
                }
                else
                {
                    Sliders[f].OverridePos = SlideTrack.GetChild(Sliders[f].TrackIndex).position;
                }
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
            FindControl f = sliderKeys[i];
            MoveControl mc = (MoveControl)f.TocaObject.GetTocaFunction<MoveControl>();
            mc.UpdateTargetPosition(Sliders[f].OverridePos);


            if (Vector3.Distance(mc.TargetPosition, mc.transform.position) < .1f)
            {
                if (Sliders[f].TrackIndex == SlideTrack.childCount - 1)
                {
                    remove.Add(f);
                }
                else
                {
                    Sliders[f].TrackIndex++;
                    if (Sliders[f].TrackIndex == SlideTrack.childCount - 1)
                    {
                        Sliders[f].OverridePos = SlideTrack.GetChild(Sliders[f].TrackIndex).position + new Vector3(Random.Range(0f, 2f), Random.Range(-1f, 0f));
                    }
                    else
                    {
                        Sliders[f].OverridePos = SlideTrack.GetChild(Sliders[f].TrackIndex).position;
                    }
                }
            }
        }

        // remove finished find control
        foreach (FindControl f in remove) 
        {
            f.TryDetach();
            Sliders.Remove(f);
            // recalculate the layer
            LayerControl lc = (LayerControl)f.TocaObject.GetTocaFunction<LayerControl>();
            if (lc)
                lc.DetouchCallback();

            MoveControl mc = (MoveControl)f.TocaObject.GetTocaFunction<MoveControl>();
            if (mc)
                mc.Speed = 0;
        }
    }
}
