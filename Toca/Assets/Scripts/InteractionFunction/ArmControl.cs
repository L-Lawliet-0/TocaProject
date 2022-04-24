using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Spine;
using Spine.Unity;

public class ArmControl : LimbControl
{
    public BaseControl HandBase;
    public FindControl PendingAttach;
   
    public ArmControl(MoveControl move, float h_Upper, float h_Lower, float v_Upper, float v_Lower, Bone targetBone) : base(move, h_Upper, h_Lower, v_Upper, v_Lower, targetBone)
    {

    }

    public override void UpdateLimbRotation()
    {
        if (PendingAttach && PendingAttach.BasePreview == HandBase)
        {
            // try to aim the hand at
            Debug.LogError("Update in arm control");
            Vector3 origin = TargetBone.GetWorldPosition(HandBase.transform.parent);
            Vector2 offset = PendingAttach.transform.position - origin;
            offset = offset.normalized;

            // get an angle
            float angle = Mathf.Atan2(offset.y, offset.x) * Mathf.Rad2Deg;
            angle = GlobalParameter.ClampAngle(angle - 90);

            if (HandBase.MyBaseAttributes.IsLeftHand)
                angle = Mathf.Clamp(angle, 100, 170);
            else
                angle = Mathf.Clamp(angle, 190, 260);

            TargetBone.Rotation = angle;
        }
        else
            base.UpdateLimbRotation();
    }

    private void Update()
    {
        // see if theres any nearby object that this hand can reach

        /*
        if (PendingAttach && PendingAttach.BasePreview == HandBase)
        {
            // calculate the target rotation value
            // rotate the hand toward pending attach position
            Quaternion temp = transform.rotation;
            transform.up = (PendingAttach.transform.position - transform.position).normalized;
            transform.eulerAngles -= new Vector3(0, 0, 145);

            TargetAngle = transform.eulerAngles;
            transform.rotation = temp;
            transform.rotation = Quaternion.RotateTowards(transform.rotation, Quaternion.Euler(TargetAngle), 15 * Time.deltaTime);
        }
        else
            base.UpdateLimbRotation();
            */
    }
}
