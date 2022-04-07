using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
 * touch handler is used to handle touch and stop touching event from input system
 */
public class TouchControl : TocaFunction
{
    private SelectionControl Selection;
    private FindControl Find;
    private MoveControl Move;
    private LayerControl Layer;

    public delegate void VoidDelegate();
    public List<VoidDelegate> TouchCallBacks; // used for register on click event
    public List<VoidDelegate> DeTouchCallBacks; // used for register on release event

    public bool CallbackOnly;

    private void Awake()
    {
        TouchCallBacks = new List<VoidDelegate>();
        DeTouchCallBacks = new List<VoidDelegate>();
    }

    private void Start()
    {
        Selection = (SelectionControl)TocaObject.GetTocaFunction<SelectionControl>();
        Find = (FindControl)TocaObject.GetTocaFunction<FindControl>();
        Move = (MoveControl)TocaObject.GetTocaFunction<MoveControl>();

        Layer = (LayerControl)TocaObject.GetTocaFunction<LayerControl>();
    }

    // need a collider to detech input
    public void OnTouch(Vector3 pos)
    {
        if (!CallbackOnly)
        {
            if (Find)
                Find.TryDetach();
            if (Selection)
                Selection.OnSelect(pos);
            if (Layer)
                Layer.TouchCallback(pos);
        }

        foreach (VoidDelegate dele in TouchCallBacks)
            dele();
    }

    public void OnDeTouch()
    {

        if (!CallbackOnly)
        {
            if (Move)
                Move.Speed = 0;

            if (Find)
                Find.TryAttach();

            if (Selection)
                Selection.OnDeselect();

            if (Layer)
                Layer.DetouchCallback();
        }
        foreach (VoidDelegate dele in DeTouchCallBacks)
            dele();
    }

    public void OnTouchPositionChanged(Vector3 worldPosition)
    {
        if (CallbackOnly)
            return;

        if (Selection)
            Selection.UpdateSelectionPos(worldPosition);
        if (Find)
            Find.SelectedUpdate();
    }

}