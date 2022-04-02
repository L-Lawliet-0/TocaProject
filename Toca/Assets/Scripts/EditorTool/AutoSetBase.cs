using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
public class AutoSetBase : MonoBehaviour
{
    public bool SET;
    public BaseControl Reference;

    private void Update()
    {
        if (SET)
        {
            Collider2D collider = Physics2D.OverlapBox(transform.position, GetComponent<Collider2D>().bounds.size, 0, 1 << LayerMask.NameToLayer("Base"));
            if (collider)
            {
                Reference = collider.GetComponent<BaseControl>();

                TocaObject toca = GetComponent<TocaObject>();
                toca.TocaSave.Attaching = true;
                toca.TocaSave.ParentObjectID = Reference.GetComponentInParent<TocaObject>().TocaSave.ObjectID;
                toca.TocaSave.ParentBaseID = Reference.BaseID;
            }

            SET = false;
        }
    }
}
