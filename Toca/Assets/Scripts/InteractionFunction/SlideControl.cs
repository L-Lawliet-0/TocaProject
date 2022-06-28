using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SlideControl : TocaFunction
{
    public BaseControl FallFloor;
    public float X_Random_Min = 0, X_Random_Max = 2, Y_Random_Min = -1, Y_Random_Max = 0;
    private class SlideData
    {
        public int TrackIndex;
        public Vector3 OverridePos;
        public Quaternion OverrideAngle;

        public SlideData(int index)
        {
            TrackIndex = index;
            OverrideAngle = Quaternion.identity;
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
                    Sliders[f].OverridePos = SlideTrack.GetChild(Sliders[f].TrackIndex).position + new Vector3(Random.Range(X_Random_Min,X_Random_Max), Random.Range(Y_Random_Min, Y_Random_Max));
                    Sliders[f].OverrideAngle = SlideTrack.GetChild(Sliders[f].TrackIndex).rotation;
                }
                else
                {
                    Sliders[f].OverridePos = SlideTrack.GetChild(Sliders[f].TrackIndex).position;
                    Sliders[f].OverrideAngle = SlideTrack.GetChild(Sliders[f].TrackIndex).rotation;
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
            if (f.IsHuman)
                mc.UpdateTargetPosition(Sliders[f].OverridePos);
            else
                mc.UpdateTargetPosition(Sliders[f].OverridePos + f.ObjectHeight / 2 * Vector3.up);
            
            mc.TocaObject.transform.rotation = Quaternion.RotateTowards(mc.TocaObject.transform.rotation, Sliders[f].OverrideAngle, Time.deltaTime * 90);


            if (Vector3.Distance(mc.TargetPosition, mc.transform.position) < .1f)
            {
                if (Sliders[f].TrackIndex >= SlideTrack.childCount - 1)
                {
                    remove.Add(f);
                }
                else
                {
                    Sliders[f].TrackIndex++;
                    if (Sliders[f].TrackIndex == SlideTrack.childCount - 1)
                    {
                        Sliders[f].OverridePos = SlideTrack.GetChild(Sliders[f].TrackIndex).position + new Vector3(Random.Range(X_Random_Min, X_Random_Max), Random.Range(Y_Random_Min, Y_Random_Max));
                        Sliders[f].OverrideAngle = SlideTrack.GetChild(Sliders[f].TrackIndex).rotation;
                    }
                    else
                    {
                        Sliders[f].OverridePos = SlideTrack.GetChild(Sliders[f].TrackIndex).position;
                        Sliders[f].OverrideAngle = SlideTrack.GetChild(Sliders[f].TrackIndex).rotation;
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

            TouchControl tc = (TouchControl)f.TocaObject.GetTocaFunction<TouchControl>();
            if (tc)
            {
                if (FallFloor)
                    f.BasePreview = FallFloor;
                tc.OnTouch(tc.TocaObject.transform.position);
                tc.OnDeTouch();
            }

        }
    }
}
