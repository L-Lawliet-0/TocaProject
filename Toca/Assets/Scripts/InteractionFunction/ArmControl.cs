using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ArmControl : LimbControl
{
    private BaseControl HandBase;
    private void Start()
    {
        base.Init();
        HandBase = GetComponentInChildren<BaseControl>();
    }

    private void Update()
    {
        // see if theres any nearby object that this hand can reach
        // 

        base.UpdateLimbRotation();

    }
}
