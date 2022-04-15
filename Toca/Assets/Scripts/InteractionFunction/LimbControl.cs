using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Spine;
using Spine.Unity;

public class LimbControl
{
    public bool Active;
    private MoveControl Selectable;

    public float H_UpperLimit = -30;
    public float H_LowerLimit = 10;
    public float V_UpperLimit = -30;
    public float V_LowerLimit = 10;
    public float TargetAngle;
    public bool ReachedDesiredAngle;
    public Bone TargetBone;
    private float DefaultRotation;
    private float RotationSpeed = 90;

    public LimbControl(MoveControl move, float h_Upper, float h_Lower, float v_Upper, float v_Lower, Bone targetBone)
    {
        // default rotation of the bone
        DefaultRotation = GlobalParameter.ClampAngle(targetBone.Rotation);
        targetBone.Rotation = DefaultRotation;

        Selectable = move;
        Active = true;
        H_UpperLimit = h_Upper;
        H_LowerLimit = h_Lower;
        V_UpperLimit = v_Upper;
        V_LowerLimit = v_Lower;
        TargetBone = targetBone;
        ReachedDesiredAngle = true;
    }

    public virtual void UpdateLimbRotation()
    {
        if (this is ArmControl)
            Debug.LogError("update in limb control");
        if (Active)
        {
            if (Selectable.Speed > GlobalParameter.MinLimbSpeed)
            {
                // calculate the target rotation value
                float horizontalSpeed = Selectable.Speed * Selectable.Direction.x;
                float verticalSpeed = Selectable.Speed * Selectable.Direction.y;
                float thisX = 0, thisY = 0;

                if (horizontalSpeed > 0)
                {
                    thisX = Mathf.Lerp(0, H_UpperLimit, (horizontalSpeed - GlobalParameter.MinLimbSpeed) / (GlobalParameter.MaxLimbSpeed - GlobalParameter.MinLimbSpeed));
                }
                else
                {
                    thisX = Mathf.Lerp(0, H_LowerLimit, (-horizontalSpeed - GlobalParameter.MinLimbSpeed) / (GlobalParameter.MaxLimbSpeed - GlobalParameter.MinLimbSpeed));
                }

                if (verticalSpeed > 0)
                {
                    thisY = Mathf.Lerp(0, V_UpperLimit, (verticalSpeed - GlobalParameter.MinLimbSpeed) / (GlobalParameter.MaxLimbSpeed - GlobalParameter.MinLimbSpeed));
                }
                else
                {
                    thisY = Mathf.Lerp(0, V_LowerLimit, (-verticalSpeed - GlobalParameter.MinLimbSpeed) / (GlobalParameter.MaxLimbSpeed - GlobalParameter.MinLimbSpeed));
                }

                float currentAngle = TargetBone.Rotation;
                float newTarget = GlobalParameter.ClampAngle(thisX + thisY + DefaultRotation);
                float currentTarget = GlobalParameter.ClampAngle(TargetAngle);

                // Debug.LogError("current z: " + currentAngle + " target z: " + newTarget);

                if (!ReachedDesiredAngle)
                {
                    // if the desired has not been met yet, only change target angle if swing higher
                    if (currentTarget < currentAngle && newTarget < currentTarget)
                        TargetAngle = newTarget;
                    if (currentTarget > currentAngle && newTarget > currentTarget)
                        TargetAngle = newTarget;
                }
                else
                {
                    TargetAngle = newTarget;
                    ReachedDesiredAngle = false;
                }
            }
            else if (ReachedDesiredAngle)
            {
                TargetAngle = DefaultRotation;
            }
        }

        //transform.eulerAngles = Vector3.Lerp(transform.eulerAngles, TargetAngle, );
        //transform.rotation = Quaternion.RotateTowards(transform.rotation, Quaternion.Euler(TargetAngle), 60 * Time.deltaTime);
        float direction = TargetAngle > TargetBone.Rotation ? 1 : -1;
        if (TargetAngle == TargetBone.Rotation)
            direction = 0;
        TargetBone.Rotation += direction * RotationSpeed * Time.deltaTime;

        if (Mathf.Abs(TargetBone.Rotation - TargetAngle) <= RotationSpeed * Time.deltaTime)
        {
            ReachedDesiredAngle = true;
            TargetBone.Rotation = TargetAngle;
        }
    }
}
