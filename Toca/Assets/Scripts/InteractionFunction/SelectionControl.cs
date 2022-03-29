using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SelectionControl : TocaFunction
{
    public bool Selected;
    public Vector3 PositionOffset; // this is the position difference between object position and touch position
    private MoveControl MoveControl;
    private FindControl FindControl;

    private void Start()
    {
        MoveControl = (MoveControl)TocaObject.GetTocaFunction<MoveControl>();
        FindControl = (FindControl)TocaObject.GetTocaFunction<FindControl>();
    }

    public void OnSelect(Vector3 initPos)
    {
        Selected = true;
        // update position offset
        PositionOffset = initPos - transform.position;
        transform.parent = null;
    }

    public void OnDeselect()
    {
        Selected = false;
    }

    public void UpdateSelectionPos(Vector3 newPos)
    {
        // when object is in selection, update its position so it moves toward target
        if (MoveControl && Selected)
        {
            if (FindControl.IsHuman)
                MoveControl.UpdateTargetPosition(newPos - PositionOffset);
            else
            {
                // Snap to bottom position
                // 
                MoveControl.UpdateTargetPosition(newPos - new Vector3(0, TocaObject.Bottom.localPosition.y * TocaObject.Bottom.lossyScale.y));
            }
        }
    }
}
