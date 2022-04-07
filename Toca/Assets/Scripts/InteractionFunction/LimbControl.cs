using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LimbControl : TocaFunction
{
    public bool Active;
    private MoveControl Selectable;

    public float H_UpperLimit = -30;
    public float H_LowerLimit = 10;
    public float V_UpperLimit = -30;
    public float V_LowerLimit = 10;
    public Vector3 TargetAngle;
    public bool ReachedDesiredAngle;

    private void Start()
    {
        Init();
        ReachedDesiredAngle = true;
    }

    public void Init()
    {
        Selectable = (MoveControl)TocaObject.GetTocaFunction<MoveControl>();
        Active = true;
    }

    private void Update()
    {
        UpdateLimbRotation();
    }

    public void UpdateLimbRotation()
    {
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

                float currentAngle = GlobalParameter.ClampAngle(transform.eulerAngles.z);
                float newTarget = GlobalParameter.ClampAngle(thisX + thisY);
                float currentTarget = GlobalParameter.ClampAngle(TargetAngle.z);

                // Debug.LogError("current z: " + currentAngle + " target z: " + newTarget);

                if (!ReachedDesiredAngle)
                {
                    // if the desired has not been met yet, only change target angle if swing higher
                    if (currentTarget < currentAngle && newTarget < currentTarget)
                        TargetAngle = new Vector3(0, 0, newTarget);
                    if (currentTarget > currentAngle && newTarget > currentTarget)
                        TargetAngle = new Vector3(0, 0, newTarget);
                }
                else
                {
                    TargetAngle = new Vector3(0, 0, newTarget);
                    ReachedDesiredAngle = false;
                }
            }
            else if (ReachedDesiredAngle)
            {
                TargetAngle = Vector3.zero;
            }
        }

        //transform.eulerAngles = Vector3.Lerp(transform.eulerAngles, TargetAngle, );
        transform.rotation = Quaternion.RotateTowards(transform.rotation, Quaternion.Euler(TargetAngle), 60 * Time.deltaTime);
        float myAngle = transform.eulerAngles.z;
        if (Mathf.Abs(GlobalParameter.ClampAngle(myAngle) - GlobalParameter.ClampAngle(TargetAngle.z)) < 1)
            ReachedDesiredAngle = true;
    }
}
