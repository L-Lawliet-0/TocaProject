using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
[RequireComponent(typeof(BoxCollider2D))]
public class BaseLimitSetter : MonoBehaviour
{
    public bool SETLIMIT;
    public bool DESTROY;

    private void Update()
    {
        if (SETLIMIT)
        {
            BaseControl bc = transform.parent.GetComponent<BaseControl>();
            Collider2D collider = GetComponent<Collider2D>();
            if (bc && collider)
            {
                //bc.MaxObjectHeight = collider.bounds.size.y;
                bc.MaxObjectHeight = collider.bounds.center.y + collider.bounds.extents.y - bc.GetComponent<Collider2D>().bounds.center.y + bc.GetComponent<Collider2D>().bounds.extents.y;
                bc.MaxObjectWidth = collider.bounds.size.x;
            }
            SETLIMIT = false;
        }

        if (DESTROY)
        {
            DestroyImmediate(gameObject);
            DESTROY = false;
        }
    }
}
