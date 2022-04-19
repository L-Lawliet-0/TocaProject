using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SlideDoorControl : TocaFunction
{
    public enum SlideStatus
    {
        Close,
        Open,
        Sliding
    }

    public SlideStatus m_SlideStatus;

    public Transform SlideDoor; // the door will be moving
    public Transform OpenTarget;

    private Vector3 ClosePos, OpenPos;
    public float Speed = 10;

    public BaseControl[] ExposedBases;

    private void Start()
    {
        ClosePos = SlideDoor.position;
        OpenPos = OpenTarget.position;

        // register open and close event on touch control
        TouchControl openTouch = SlideDoor.GetComponent<TouchControl>();
        openTouch.ClickCallBacks.Add(OnSlideTouch);
        m_SlideStatus = SlideStatus.Close;

        SetBases(false);
    }

    public void OnSlideTouch()
    {
        if (m_SlideStatus == SlideStatus.Close)
        {
            m_SlideStatus = SlideStatus.Sliding;
            StartCoroutine("Slide",SlideStatus.Open);
        }
        else if (m_SlideStatus == SlideStatus.Open)
        {
            m_SlideStatus = SlideStatus.Sliding;
            StartCoroutine("Slide", SlideStatus.Close);
        }
    }

    private IEnumerator Slide(SlideStatus target)
    {
        if (target == SlideStatus.Open)
            SetBases(true);

        Vector3 targetPos;
        if (target == SlideStatus.Open)
            targetPos = OpenPos;
        else
            targetPos = ClosePos;

        float distance = Vector3.Distance(SlideDoor.position, targetPos);
        while (distance > .1f)
        {
            float temp = distance;
            SlideDoor.position += (targetPos - SlideDoor.position).normalized * Time.deltaTime * Speed;
            distance = Vector3.Distance(SlideDoor.position, targetPos);
            if (distance >= temp)
                break;
            yield return null;
        }

        SlideDoor.position = targetPos;
        m_SlideStatus = target;

        if (target == SlideStatus.Close)
            SetBases(false);
    }

    private void SetBases(bool value)
    {
        foreach (BaseControl bc in ExposedBases)
            bc.gameObject.SetActive(value);

        if (value)
        {
            // when open the slider, override the layer value to higher 
            SlideDoor.GetComponent<SpriteRenderer>().sortingOrder = OpenTarget.GetComponent<SpriteRenderer>().sortingOrder + 1;

            // override the layer control value for selection purpose
            ((LayerControl)SlideDoor.GetComponent<TouchControl>().TocaObject.GetTocaFunction<LayerControl>()).OrderValue = ((LayerControl)OpenTarget.GetComponent<TouchControl>().TocaObject.GetTocaFunction<LayerControl>()).OrderValue + 1;
        }
    }
}
