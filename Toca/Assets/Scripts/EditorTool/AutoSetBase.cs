using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[ExecuteInEditMode]
public class AutoSetBase : MonoBehaviour
{
    public bool SET;
    public BaseControl Reference;

    private void Update()
    {
        if (SET)
        {
            Reference = null;
            Collider2D collider = Physics2D.OverlapBox(transform.position, GetComponent<Collider2D>().bounds.size, 0, 1 << LayerMask.NameToLayer("Base"));
            if (collider)
            {
                Reference = collider.GetComponent<BaseControl>();

                if (Reference.GetComponentInParent<TocaObject>())
                {
                    TocaObject toca = GetComponent<TocaObject>();
                    toca.TocaSave.Attaching = true;
                    toca.TocaSave.ParentObjectID = Reference.GetComponentInParent<TocaObject>().TocaSave.ObjectID;
                    toca.TocaSave.ParentBaseID = Reference.BaseID;
                }
                else
                    Reference = null;
            }

            SET = false;
        }
    }

    /*
    private void OnDrawGizmos()
    {
        if (Selection.activeGameObject == gameObject && Reference)
        {
            Gizmos.DrawSphere(Reference.transform.position, .3f);
        }
    }
    */
}
