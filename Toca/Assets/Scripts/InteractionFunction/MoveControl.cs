using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveControl : TocaFunction
{
    public Vector3 TargetPosition;
    public Vector3 Direction;
    public float Speed;
    public bool InstantGo;

    public enum MoveMode
    {
        TimeReach,
        Linear,
        Freefall
    }
    public MoveMode CurrentMoveMode;

    private FindControl Find;
    private SelectionControl Select;

    private void Start()
    {
        Find = (FindControl)TocaObject.GetTocaFunction<FindControl>();
        Select = (SelectionControl)TocaObject.GetTocaFunction<SelectionControl>();
        TargetPosition = transform.position;
        InstantGo = false;
    }

    public void UpdateTargetPosition(Vector3 targetPos)
    {
        TargetPosition = targetPos;
    }

    // handle object movement logic, clsing to target every frame
    private void Update()
    {
        if (!CanMove())
            return;
        Direction = (TargetPosition - transform.position).normalized;
        switch (CurrentMoveMode)
        {
            case MoveMode.TimeReach:
                Speed = (TargetPosition - transform.position).magnitude / GlobalParameter.ReachTime;
                break;
            case MoveMode.Freefall:
                Speed += Time.deltaTime * GlobalParameter.Acceleration;
                break;
        }

        transform.position += Direction * Speed * Time.deltaTime;
        if (InstantGo)
            transform.position = TargetPosition;
        Vector3 newDir = (TargetPosition - transform.position).normalized;

        if (Vector3.Distance(transform.position, TargetPosition) < .1f || Vector3.Dot(Direction, newDir) <= 0)
        {
            transform.position = TargetPosition;

            if (Find && Find.CurrentAttachment && !Find.Arrived)
            {
                Find.Arrived = true;
                TocaObject.transform.parent = Find.CurrentAttachment.transform;
                Speed = 0;

                //transform.position -= Vector3.up * GrandChange;
                //GrandChange = 0;
                StopCoroutine("ObjectShake");
                StartCoroutine("ObjectShake");
            }
        }
    }

    private void OnDisable()
    {
        //transform.position -= Vector3.up * GrandChange;
        //GrandChange = 0;
    }

    public bool CanMove()
    {
        if (Select && Select.Selected)
        {
            CurrentMoveMode = MoveMode.TimeReach;
            return true;
        }
        
        if (Find && Find.CurrentAttachment && (!Find.Arrived || Find.CurrentAttachment.TocaObject.GetTocaFunction<SlideControl>() || Find.CurrentAttachment.TocaObject.GetTocaFunction<TrashBinControl>()))
        {
            CurrentMoveMode = MoveMode.Freefall;
            return true;
        }

        float angle = GlobalParameter.ClampAngle(transform.eulerAngles.z);
        if (angle > 10 && angle < 350)
            return true;

        return false;
    }

    private float GrandChange;
    private IEnumerator ObjectShake()
    {
        float speed = 10;
        float count = .1f;
        GrandChange = 0;
        while (count > 0)
        {
            transform.position += Vector3.up * Time.fixedDeltaTime * speed;
            GrandChange += Time.fixedDeltaTime * speed;
            count -= Time.fixedDeltaTime;
            yield return new WaitForFixedUpdate();
        }

        count = .1f;
        while (count > 0)
        {
            transform.position -= Vector3.up * Time.fixedDeltaTime * speed;
            GrandChange -= Time.fixedDeltaTime * speed;
            count -= Time.fixedDeltaTime;
            yield return new WaitForFixedUpdate();
        }
    }
}
