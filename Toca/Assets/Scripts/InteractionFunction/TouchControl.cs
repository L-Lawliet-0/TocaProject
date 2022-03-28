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
        if (Find)
            Find.TryDetach();
        if (Selection)
            Selection.OnSelect(pos);
        if (Layer)
            Layer.TouchCallback(pos);
    }

    public void OnDeTouch()
    {
        Debug.LogError("Positon when releasing the mouse: " + transform.position + "at time frame: " + Time.time);
        if (Move)
            Move.Speed = 0;

        if (Find)
            Find.TryAttach();

        if (Selection)
            Selection.OnDeselect();

        if (Layer)
            Layer.DetouchCallback();
    }

    public void OnTouchPositionChanged(Vector3 worldPosition)
    {
        if (Selection)
            Selection.UpdateSelectionPos(worldPosition);
        if (Find)
            Find.SelectedUpdate();
    }

}