using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveControl : TocaFunction
{
    public Vector3 TargetPosition;
    public Vector3 Direction;
    public float Speed;
    public bool InstantGo;
    public bool Shaked; // does this object already shaked in this selection
    private bool Shaking;

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
        if (!CanMove())
            transform.position = targetPos;
        if (Shaking)
            tempTargetSave = targetPos;
        else
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
                if (!Shaked)
                {
                    if (!GlobalParameter.OverrideMove(Find.CurrentAttachment) && gameObject.activeInHierarchy)
                    {
                        StopCoroutine("ObjectShake");
                        StartCoroutine("ObjectShake");
                        tempTargetSave = TargetPosition;
                        Shaking = true;
                    }
                    Shaked = true;
                }
            }
        }
    }

    private void OnEnable()
    {

    }

    private void OnDisable()
    {
        StopAllCoroutines();
        if (Find && Find.CurrentAttachment && Find.CurrentAttachment.Attachments[Find].Timer >= 1)
            return;

        if (Shaking)
        {
            Shaking = false;
            transform.position = tempTargetSave;
            TargetPosition = tempTargetSave;
        }
        else
            transform.position = TargetPosition;
    }

    public bool CanMove()
    {
        if (Shaking)
        {
            CurrentMoveMode = MoveMode.Linear;
            Speed = 5;
            return true;
        }

        if (Select && Select.Selected)
        {
            CurrentMoveMode = MoveMode.TimeReach;
            return true;
        }
        
        if (Find && Find.CurrentAttachment)
        {
            /*
            if (Find.CurrentAttachment.TocaObject.GetTocaFunction<FloatControl>())
            {
                CurrentMoveMode = MoveMode.Linear;
                Speed = 10;
            }
            else*/ if ((!Find.Arrived || Find.CurrentAttachment.TocaObject.GetTocaFunction<SlideControl>() || Find.CurrentAttachment.TocaObject.GetTocaFunction<TrashBinControl>()))
                CurrentMoveMode = MoveMode.Freefall;
            else
                return false;
            return true;
        }

        float angle = GlobalParameter.ClampAngle(transform.eulerAngles.z);
        if (angle > 10 && angle < 350)
            return true;


        return false;
    }

    private float GrandChange;
    private Vector3 tempTargetSave;
    private IEnumerator ObjectShake()
    {
        // speed = 10
        // 
        Vector3 targetSave = TargetPosition;

        TargetPosition = targetSave + Vector3.up * .5f;

        yield return new WaitForSeconds(.1f);

        TargetPosition = tempTargetSave;

        yield return new WaitForSeconds(.1f);

        transform.position = tempTargetSave;
        Shaking = false;
    }
}
