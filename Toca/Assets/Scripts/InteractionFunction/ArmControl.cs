using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ArmControl : LimbControl
{
    private BaseControl HandBase;
    public FindControl PendingAttach;
    private void Start()
    {
        base.Init();
        HandBase = GetComponentInChildren<BaseControl>();
    }

    private void Update()
    {
        // see if theres any nearby object that this hand can reach

        if (PendingAttach && PendingAttach.BasePreview == HandBase)
        {
            // calculate the target rotation value
            // rotate the hand toward pending attach position
            Quaternion temp = transform.rotation;
            transform.up = (PendingAttach.transform.position - transform.position).normalized;
            transform.eulerAngles -= new Vector3(0, 0, 145);

            TargetAngle = transform.eulerAngles;
            transform.rotation = temp;
            transform.rotation = Quaternion.RotateTowards(transform.rotation, Quaternion.Euler(TargetAngle), 60 * Time.deltaTime);
        }
        else
            base.UpdateLimbRotation();
    }
}
