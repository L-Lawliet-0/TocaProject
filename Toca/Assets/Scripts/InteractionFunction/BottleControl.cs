using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BottleControl : StateControl
{
    private Transform BottleTop;
    private bool InControl;
    private bool Shaking;

    private Collider2D CupCollider;
    private BaseControl Mouth;
    public Color LiquidColor;

    private void Awake()
    {
        // assume bottle is in standing pose, calcualte the top reference position
        BottleTop = new GameObject().transform;
        BottleTop.position = GetComponent<Collider2D>().bounds.center + GetComponent<Collider2D>().bounds.extents.y * Vector3.up;
        BottleTop.SetParent(transform);
    }

    // control the particle effect and bottle rotation when in selection and deselection
    private void Start()
    {
        base.RegisterStateEvent();
        FindControl = (FindControl)TocaObject.GetTocaFunction<FindControl>();
        LayerControl = (LayerControl)TocaObject.GetTocaFunction<LayerControl>();
    }

    public override void OnSelection()
    {
        ForceEnd();
        InControl = true;
        Shaking = false;
        StopAllCoroutines();
        StartCoroutine("SelectionLoop");
    }

    public override void OnDeselect()
    {
        InControl = false;
        StopCoroutine("Shake");
    }

    private IEnumerator SelectionLoop()
    {
        while (InControl)
        {
            // what conditions will the bottle start shaking
            // get close to child mouth
            // get close to cup

            Collider2D[] colliders = Physics2D.OverlapCircleAll(BottleTop.position, .1f, 1 << LayerMask.NameToLayer("WaterDrop"));
            CupCollider = null;
            float minDis = float.MaxValue;
            foreach (Collider2D c in colliders)
            {
                float dis = Vector3.Distance(BottleTop.position, c.transform.parent.position);
                if (dis < minDis)
                {
                    minDis = dis;
                    CupCollider = c;
                }
            }

            colliders = Physics2D.OverlapCircleAll(BottleTop.position, .1f, 1 << LayerMask.NameToLayer("Base"));

            Mouth = null;
            foreach (Collider2D c in colliders)
            {
                BaseControl bc = c.GetComponent<BaseControl>();
                if (bc && bc.MyBaseAttributes.IsMouth)
                {
                    Mouth = bc;
                    break;
                }
            }

            if (!Shaking && (Mouth || CupCollider))
            {
                Shaking = true;
                StartCoroutine("Shake");
            }
            else if (Shaking && !Mouth && !CupCollider)
            {
                Shaking = false;
                StopCoroutine("Shake");
                transform.eulerAngles = Vector3.zero;
            }

            yield return null;
        }

        while (GlobalParameter.ClampAngle(transform.eulerAngles.z) < 270)
        {
            transform.eulerAngles -= Vector3.forward * Time.deltaTime * 240;
            yield return null;
        }

        transform.eulerAngles = Vector3.zero;
        ForceEnd(true);
    }

    private IEnumerator Shake()
    {
        while (true)
        {
            transform.eulerAngles = new Vector3(0, 0, 1);
            float angle = Random.Range(30, 60);
            while (GlobalParameter.ClampAngle(transform.eulerAngles.z) < angle)
            {
                transform.eulerAngles += Vector3.forward * Time.deltaTime * 45;
                yield return null;
            }

            if (CupCollider)
                CupCollider.GetComponent<WaterFillControl>().Fill(LiquidColor);

            while (GlobalParameter.ClampAngle(transform.eulerAngles.z) < 270)
            {
                transform.eulerAngles -= Vector3.forward * Time.deltaTime * 45;
                yield return null;
            }

            yield return null;
        }
    }
}
