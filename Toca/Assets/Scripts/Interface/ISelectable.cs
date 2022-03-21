using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ISelectable : MonoBehaviour
{
    public Vector3 TargetPosition;
    public Vector3 PositionOffset;
    public bool Selected { get; private set; }
    public bool Snapping { get; private set; }

    public Vector3 Direction;
    public float Speed;
    public Transform Bottom;

    // function called when gameobject is being selected by player
    public void OnSelect(Vector3 initalPosition)
    {
        TargetPosition = initalPosition;
        PositionOffset = TargetPosition - transform.position;
        Selected = true;
        Snapping = false;
    }

    // function called when gameobject is deselected by player
    public void OnDeSelect()
    {
        Selected = false;
        Speed = 0;
        Direction = Vector3.zero;

        // do a ray cast toward the bottom to detect ground
        RaycastHit2D hit = Physics2D.Raycast(Bottom.transform.position, -Vector2.up, 9999, 1 << LayerMask.NameToLayer("Ground"));
        if (hit)
        {
            Snapping = true;

            TargetPosition = hit.point;
            TargetPosition.z = GlobalParameter.Depth;
            PositionOffset = Bottom.position - transform.position;

            Direction = (TargetPosition - (transform.position + PositionOffset)).normalized;
            Speed = 0;
        }
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
                transform.position = TargetPosition - PositionOffset;
            else
                transform.position += Direction * Speed * Time.deltaTime;
        }
        else if (Snapping)
        {
            Vector3 localDir = (TargetPosition - (transform.position + PositionOffset)).normalized;
            if (Vector3.Dot(localDir, Direction) < 0)
            {
                transform.position = TargetPosition - PositionOffset;
                Snapping = false;
                Direction = Vector3.zero;
                Speed = 0;
            }
            else
            {
                transform.position += Direction * Speed * Time.deltaTime;
                Speed += Time.deltaTime * GlobalParameter.Gravity;
            }
        }
    }
}
