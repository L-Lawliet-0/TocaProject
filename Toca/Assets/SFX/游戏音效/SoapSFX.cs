using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoapSFX : MonoBehaviour
{
    private GameObject SFX;
    private TouchControl Tc;

    private void Start()
    {
        Tc = GetComponent<TouchControl>();
        Tc.TouchCallBacks.Add(Select);
        Tc.DeTouchCallBacks.Add(DeSelect);
    }

    private void Update()
    {
        if (SFX)
            SFX.transform.position = transform.position;
    }

    private void Select()
    {
        if (!SFX)
            SFX = SoundManager.Instance.PlaySFX(28, false, transform.position);
    }

    private void DeSelect()
    {
        Destroy(SFX);
    }
}
