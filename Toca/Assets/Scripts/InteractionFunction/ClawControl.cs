using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ClawControl : TocaFunction
{
    private Vector3 PositionLastFrame;
    public float DragRange;
    private float PosMin, PosMax;
    private bool Grabing;
    public Transform DetachOnClick;

    public Transform Claw; // the claw used to grab shit
    public Transform GiftSpawnPos;
    public GameObject[] GiftPool;

    void Start()
    {
        Grabing = false;
        TouchControl tc = (TouchControl)TocaObject.GetTocaFunction<TouchControl>();
        tc.TouchCallBacksV3.Add(OnTouch);
        tc.PositionChangeCallBacksV3.Add(OnPositionChanged);
        tc.DeTouchCallBacks.Add(OnRelease);
        PosMin = TocaObject.transform.position.x - DragRange;
        PosMax = TocaObject.transform.position.x + DragRange;
    }

    public void OnTouch(Vector3 pos)
    {
        PositionLastFrame = pos;
        if (DetachOnClick.parent == transform)
            DetachOnClick.parent = null;
    }

    public void OnPositionChanged(Vector3 pos)
    {
        if (!Grabing)
        {
            float delta = pos.x - PositionLastFrame.x;
            float temp = TocaObject.transform.position.x + delta;
            temp = Mathf.Clamp(temp, PosMin, PosMax);
            TocaObject.transform.position = new Vector3(temp, TocaObject.transform.position.y, TocaObject.transform.position.z);

            PositionLastFrame = pos;
        }
    }

    public void OnRelease()
    {
        if (!Grabing)
        {
            Grabing = true;
            StartCoroutine("Grab");
        }
    }

    private IEnumerator Grab()
    {
        Vector3 posSave = Claw.position;
        float counter = .5f;

        while (counter > 0)
        {
            Claw.position -= Vector3.up * Time.deltaTime;
            counter -= Time.deltaTime;
            yield return null;
        }

        counter = .5f;
        while (counter > 0)
        {
            Claw.position += Vector3.up * Time.deltaTime;
            counter -= Time.deltaTime;
            yield return null;
        }

        Claw.position = posSave;
        Grabing = false;

        GlobalParameter.Instance.CreateObject(GiftPool[Random.Range(0, GiftPool.Length)], GiftSpawnPos.position);
    }
}