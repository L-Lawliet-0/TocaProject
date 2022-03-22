using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LimbControl : MonoBehaviour
{
    public bool Active;
    private MoveControl Selectable;

    public float H_UpperLimit = -30;
    public float H_LowerLimit = 10;
    public float V_UpperLimit = -30;
    public float V_LowerLimit = 10;
    public Vector3 TargetAngle;

    private void Awake()
    {
        Selectable = GetComponentInParent<MoveControl>();
        Active = true;
    }

    private void Update()
    {
        if (Active)
        {
            if (Selectable.Speed > GlobalParameter.MinLimbSpeed)
            {
                // calculate the target rotation value
                float horizontalSpeed = Selectable.Speed * Selectable.Direction.x;
                float verticalSpeed = Selectable.Speed * Selectable.Direction.y;
                float angleX = 0, angleY = 0;

                if (horizontalSpeed > 0)
                {
                    angleX = Mathf.Lerp(0, H_UpperLimit, (horizontalSpeed - GlobalParameter.MinLimbSpeed) / (GlobalParameter.MaxLimbSpeed - GlobalParameter.MinLimbSpeed));
                }
                else
                {
                    angleX = Mathf.Lerp(0, H_LowerLimit, (-horizontalSpeed - GlobalParameter.MinLimbSpeed) / (GlobalParameter.MaxLimbSpeed - GlobalParameter.MinLimbSpeed));
                }

                if (verticalSpeed > 0)
                {
                    angleY = Mathf.Lerp(0, V_UpperLimit, (verticalSpeed - GlobalParameter.MinLimbSpeed) / (GlobalParameter.MaxLimbSpeed - GlobalParameter.MinLimbSpeed));
                }
                else
                {
                    angleY = Mathf.Lerp(0, V_LowerLimit, (-verticalSpeed - GlobalParameter.MinLimbSpeed) / (GlobalParameter.MaxLimbSpeed - GlobalParameter.MinLimbSpeed));
                }
                TargetAngle = new Vector3(0, 0, angleX + angleY);
            }
            else
            {
                TargetAngle = Vector3.zero;
            }
        }

        //transform.eulerAngles = Vector3.Lerp(transform.eulerAngles, TargetAngle, );
        transform.rotation = Quaternion.RotateTowards(transform.rotation, Quaternion.Euler(TargetAngle), 180 * Time.deltaTime);
    }
}
