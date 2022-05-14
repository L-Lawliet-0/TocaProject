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
        //SET = false;
        if (SET)
        {
            /*
            Reference = null;
            Collider2D[] colliders = Physics2D.OverlapBoxAll(transform.position, GetComponent<Collider2D>().bounds.size, 0, 1 << LayerMask.NameToLayer("Base"));
            foreach (Collider2D collider in colliders)
            {
                Reference = collider.GetComponent<BaseControl>();

                if (Reference.GetComponentInParent<TocaObject>() && Reference.GetComponentInParent<TocaObject>() != GetComponentInParent<TocaObject>())
                {
                    TocaObject toca = GetComponentInParent<TocaObject>();
                    toca.TocaSave.Attaching = true;
                    toca.TocaSave.ParentObjectID = Reference.GetComponentInParent<TocaObject>().TocaSave.ObjectID;
                    toca.TocaSave.ParentBaseID = Reference.BaseID;
                    break;
                }
                else
                    Reference = null;
            }
            */

            if (!Reference)
            {
                TocaObject toca = GetComponentInParent<TocaObject>();
                toca.TocaSave.Attaching = false;
            }
            else
            {
                TocaObject toca = GetComponentInParent<TocaObject>();
                toca.TocaSave.Attaching = true;
                toca.TocaSave.ParentObjectID = Reference.GetComponentInParent<TocaObject>().TocaSave.ObjectID;
                toca.TocaSave.ParentBaseID = Reference.BaseID;
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
