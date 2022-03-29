using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveControl : TocaFunction
{
    public Vector3 TargetPosition;
    public Vector3 Direction;
    public float Speed;

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
        Vector3 newDir = (TargetPosition - transform.position).normalized;

        if (Vector3.Distance(transform.position, TargetPosition) < .1f || Vector3.Dot(Direction, newDir) <= 0)
        {
            transform.position = TargetPosition;

            if (Find && Find.CurrentAttachment && !Find.Arrived)
            {
                Find.Arrived = true;
                TocaObject.transform.parent = Find.CurrentAttachment.transform;
                Speed = 0;
            }
        }
    }

    public bool CanMove()
    {
        if (Select && Select.Selected)
        {
            CurrentMoveMode = MoveMode.TimeReach;
            return true;
        }
        
        if (Find && Find.CurrentAttachment && !Find.Arrived)
        {
            CurrentMoveMode = MoveMode.Freefall;
            return true;
        }

        return false;
    }
}
