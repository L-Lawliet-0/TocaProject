using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveControl : TocaFunction
{
    public Vector3 TargetPosition;
    public Transform SnapTransform;
    public Vector3 PositionOffset;
    public bool Selected { get; private set; }
    public bool Snapping { get; private set; }
    public bool Shaking { get; private set; }
    public Vector3 Direction;
    public float Speed;
    public float InteractionRadius;

    private void Start()
    {
        // register event
        TouchHandler touch = (TouchHandler)TocaObject.GetTocaFunction<TouchHandler>();
        touch.OntouchCallbacks.Add(OnSelect);
        touch.DetouchCallbacks.Add(OnDeSelect);
        touch.OnpositionchangeCallbacks.Add(UpdateTargetPosition);
    }

    // function called when gameobject is being selected by player
    public void OnSelect(Vector3 initalPosition)
    {
        if (SnapTransform)
        {
            SnapTransform.parent.GetComponent<BaseControl>().Detach((FindControl)TocaObject.GetTocaFunction<FindControl>());
            TocaObject.transform.parent = null;
            Destroy(SnapTransform.gameObject);
            SnapTransform = null;
        }

        LayerControl lc = (LayerControl)TocaObject.GetTocaFunction<LayerControl>();
        if (lc)
            lc.TouchCallback(initalPosition);

        TargetPosition = initalPosition;
        PositionOffset = TargetPosition - transform.position;
        Selected = true;
        Snapping = false;
        Shaking = false;
    }

    // function called when gameobject is deselected by player
    public void OnDeSelect()
    {
        Selected = false;
        Speed = 0;
        Direction = Vector3.zero;

        // if object has findcontrol, snap object to find base
        FindControl find = (FindControl)TocaObject.GetTocaFunction<FindControl>();
        if (find)
        {
            Snapping = true;
            SnapTransform = find.FindBase();
            TargetPosition = SnapTransform.position;
            TargetPosition.z = GlobalParameter.Depth;
            PositionOffset = TocaObject.Bottom.position - transform.position;

            Direction = (TargetPosition - (transform.position + PositionOffset)).normalized;
            Speed = 0;

            SnapTransform.parent.GetComponent<BaseControl>().Attach((FindControl)TocaObject.GetTocaFunction<FindControl>());
        }

        LayerControl lc = (LayerControl)TocaObject.GetTocaFunction<LayerControl>();
        if (lc)
            lc.DetouchCallback();
    }

    // called when target position is changed by player
    public void UpdateTargetPosition(Vector3 position)
    {
        // update target position
        // get new direction
        TargetPosition = position;
        Direction = (TargetPosition - (transform.position + PositionOffset)).normalized;
        Speed = (TargetPosition - (transform.position + PositionOffset)).magnitude / GlobalParameter.ReachTime;
        //TargetPosition = position;
    }

    private void Update()
    {
        // if the object is selected, update the position to slerp toward target position
        if (Selected && Vector3.Distance(transform.position + PositionOffset, TargetPosition) > .1f)
        {
            transform.position += Direction * Speed * Time.deltaTime;
            Vector3 localDir = (TargetPosition - (transform.position + PositionOffset)).normalized;
            if (Vector3.Dot(localDir, Direction) <= 0)
                transform.position = TargetPosition - PositionOffset;
        }
        else if (Snapping)
        {
            TargetPosition = SnapTransform.position;
            Vector3 localDir = (TargetPosition - (transform.position + PositionOffset)).normalized;
            if (Vector3.Dot(localDir, Direction) <= 0)
            {
                transform.position = TargetPosition - PositionOffset;
                Snapping = false;
                Direction = Vector3.zero;
                Speed = 0;

                //SnapTransform.parent.GetComponent<BaseControl>().Attach((FindControl)TocaObject.GetTocaFunction<FindControl>());
                TocaObject.transform.parent = SnapTransform;
                Shaking = true;
                StartCoroutine("Shake");
            }
            else
            {
                transform.position += Direction * Speed * Time.deltaTime;
                Speed += Time.deltaTime * GlobalParameter.Acceleration;
            }
        }
    }

    private IEnumerator Shake()
    {
        float speedUp = 10;
        float counter = .1f;

        while (counter > 0 && Shaking)
        {
            transform.position += Vector3.up * speedUp * Time.fixedDeltaTime;
            counter -= Time.fixedDeltaTime;

            yield return new WaitForFixedUpdate();
        }
        counter = .1f;

        while (counter > 0 && Shaking)
        {
            transform.position -= Vector3.up * speedUp * Time.fixedDeltaTime;
            counter -= Time.fixedDeltaTime;

            yield return new WaitForEndOfFrame();
        }
    }
}
