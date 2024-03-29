using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpreadControl : StateControl
{
    public int FXindex;

    private Transform FXref;
    private bool InControl;

    private GameObject SFX;

    private void Awake()
    {
        // assume bottle is in standing pose, calcualte the top reference position
        FXref = new GameObject().transform;
        FXref.position = GetComponent<Collider2D>().bounds.center + GetComponent<Collider2D>().bounds.extents.y * Vector3.up;
        FXref.SetParent(transform);
    }

    // control the particle effect and bottle rotation when in selection and deselection
    private void Start()
    {
        base.RegisterStateEvent();
    }

    private void Update()
    {
        if (SFX)
            SFX.transform.position = TocaObject.transform.position;
    }

    public override void OnSelection()
    {
        ForceEnd();
        InControl = true;
        StopAllCoroutines();
        StartCoroutine("SelectionLoop");

        SFX = SoundManager.Instance.PlaySFX(20, false, TocaObject.transform.position);
    }

    public override void OnDeselect()
    {
        InControl = false;
        Destroy(SFX);
    }

    private IEnumerator SelectionLoop()
    {
        // create particle effect
        GameObject fx = Instantiate(GlobalParameter.Instance.RunTimeEffects[FXindex]);
        fx.transform.SetParent(FXref);
        fx.transform.localPosition = Vector3.zero;
        fx.transform.localRotation = Quaternion.identity;

        while (InControl)
        {
            while (GlobalParameter.ClampAngle(transform.eulerAngles.z) < 120)
            {
                transform.eulerAngles += Vector3.forward * Time.deltaTime * 240;
                yield return null;
            }
            transform.eulerAngles = 120 * Vector3.forward;
            yield return null;
        }

        while (GlobalParameter.ClampAngle(transform.eulerAngles.z) < 270)
        {
            transform.eulerAngles -= Vector3.forward * Time.deltaTime * 240;
            yield return null;
        }

        while (!FindControl.Arrived)
            yield return null;

        ForceEnd(true);
    }

    public override void ForceEnd(bool resetLayer = false)
    {
        transform.eulerAngles = Vector3.zero;
        base.ForceEnd(resetLayer);

        if (FXref.childCount > 0)
            Destroy(FXref.GetChild(0).gameObject);
        
    }
}
