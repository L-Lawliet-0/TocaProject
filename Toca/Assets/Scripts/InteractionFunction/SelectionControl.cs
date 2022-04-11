using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SelectionControl : TocaFunction
{
    public bool Selected;
    public Vector3 PositionOffset; // this is the position difference between object position and touch position
    private MoveControl MoveControl;
    private FindControl FindControl;

    public Vector3 DefaultScale;
    private float ZoomScale;

    private void Awake()
    {
        DefaultScale = transform.lossyScale;
        ZoomScale = 1.2f;
        Collider2D collider = GetComponent<Collider2D>();
        if (collider)
        {
            float zh = 2f / collider.bounds.size.y;
            float zw = 2f / collider.bounds.size.x;
            ZoomScale = Mathf.Min(zh, zw);
            ZoomScale = ZoomScale < 1.2f ? 1.2f : ZoomScale;
        }
    }

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
        GlobalParameter.SetGlobalScale(transform, DefaultScale);
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
        float zoomSpeed = (DefaultScale.x * ZoomScale - transform.lossyScale.x) / .2f;
        while (transform.lossyScale.x < DefaultScale.x * ZoomScale)
        {
            GlobalParameter.SetGlobalScale(transform, transform.lossyScale + Vector3.one * Time.deltaTime * zoomSpeed);
            yield return null;
        }

        GlobalParameter.SetGlobalScale(transform, DefaultScale * ZoomScale);
    }

    private IEnumerator ScaleDown()
    {
        float zoomSpeed = (transform.lossyScale.x - DefaultScale.x) / .2f;
        while (transform.lossyScale.x > DefaultScale.x)
        {
            GlobalParameter.SetGlobalScale(transform, transform.lossyScale - Vector3.one * Time.deltaTime * zoomSpeed);
            yield return null;
        }

        GlobalParameter.SetGlobalScale(transform, DefaultScale);
    }
}
