using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DragRotateControl : TocaFunction
{
    private Vector3 PositionLastFrame;
    public float MinAngle, MaxAngle;
    public float Angle_Save;
    private void Start()
    {
        Angle_Save = 0;
        TouchControl tc = GetComponent<TouchControl>();
        tc.TouchCallBacksV3.Add(OnTouch);
        tc.PositionChangeCallBacksV3.Add(OnPositionChanged);
    }

    // take the incoming position, and find which zone the v3 is in
    // left bottom, right bottom, left top, right top
    // then try to change the rotation, if the mouse position is out of range
    // cancel this updatement
    public void OnTouch(Vector3 pos)
    {
        PositionLastFrame = pos;
    }

    public void OnPositionChanged(Vector3 pos)
    {
        Debug.LogError("callback");
        Vector3 offset = pos - PositionLastFrame;

        // get absolute value from position difference
        float delta_X = offset.x * 10;
        float delta_Y = offset.y * 10;

        if (pos.y > transform.position.y)
            delta_X = -delta_X;

        if (pos.x < transform.position.x)
            delta_Y = -delta_Y;

        Angle_Save += delta_X + delta_Y;
        Angle_Save = Mathf.Clamp(Angle_Save, MinAngle, MaxAngle);

        transform.eulerAngles = Vector3.forward * Angle_Save;

        PositionLastFrame = pos;
    }

}
