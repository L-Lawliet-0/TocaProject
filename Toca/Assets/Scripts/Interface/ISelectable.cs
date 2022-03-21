using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ISelectable : MonoBehaviour
{
    public Vector3 TargetPosition;
    public Vector3 PositionOffset;
    public bool Selected { get; private set; }

    public Vector3 Direction;
    public float Speed;

    // function called when gameobject is being selected by player
    public void OnSelect(Vector3 initalPosition)
    {
        TargetPosition = initalPosition;
        PositionOffset = TargetPosition - transform.position;
        Selected = true;
    }

    // function called when gameobject is deselected by player
    public void OnDeSelect()
    {
        Selected = false;
        Speed = 0;
        Direction = Vector3.zero;
    }

    // called when target position is changed by player
    public void UpdateTargetPosition(Vector3 position)
    {
        // update target position
        // get new direction
        Direction = (TargetPosition - (transform.position + PositionOffset)).normalized;
        Speed = (TargetPosition - (transform.position + PositionOffset)).magnitude / GlobalParameter.ReachTime;
        Debug.LogError(Speed);
        TargetPosition = position;
    }

    private void Update()
    {
        // if the object is selected, update the position to slerp toward target position
        if (Selected && Vector3.Distance(transform.position + PositionOffset, TargetPosition) > .1f)
        {
            Vector3 localDir = (TargetPosition - (transform.position + PositionOffset)).normalized;
            if (Vector3.Dot(localDir, Direction) < 0)
            {
                transform.position = TargetPosition - PositionOffset;
            }
            else
            {
                transform.position += Direction * Speed * Time.deltaTime;
            }
        }
    }
}
