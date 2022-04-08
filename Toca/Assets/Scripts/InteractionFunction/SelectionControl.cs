using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SelectionControl : TocaFunction
{
    public bool Selected;
    public Vector3 PositionOffset; // this is the position difference between object position and touch position
    private MoveControl MoveControl;
    private FindControl FindControl;

    private Vector3 DefaultScale;

    private void Start()
    {
        MoveControl = (MoveControl)TocaObject.GetTocaFunction<MoveControl>();
        FindControl = (FindControl)TocaObject.GetTocaFunction<FindControl>();

        DefaultScale = transform.localScale;
    }

    public void OnSelect(Vector3 initPos)
    {
        Selected = true;
        // update position offset
        PositionOffset = initPos - transform.position;
        transform.parent = null;

        StopAllCoroutines();
        StartCoroutine("ScaleUp");
    }

    public void OnDeselect()
    {
        Selected = false;

        StopAllCoroutines();
        StartCoroutine("ScaleDown");
    }

    private void OnDisable()
    {
        transform.localScale = DefaultScale;
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

    private IEnumerator ScaleUp()
    {
        while (transform.localScale.x < DefaultScale.x * 1.2f)
        {
            transform.localScale += Vector3.one * Time.deltaTime;
            yield return null;
        }
        transform.localScale = DefaultScale * 1.2f;
    }

    private IEnumerator ScaleDown()
    {
        while (transform.localScale.x > DefaultScale.x)
        {
            transform.localScale -= Vector3.one * Time.deltaTime;
            yield return null;
        }
        transform.localScale = DefaultScale;
    }
}
