using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

[ExecuteInEditMode]
public class TotaObjectInitalizer : MonoBehaviour
{
    public bool INIT;

    // toca object function parameter
    public bool Tota, Touch, Selection, Move, Base, Find, Open;

    // extra parameter

    private void Update()
    {
        if (INIT)
        {
            GameObject obj = gameObject;

            if (Tota)
            {
                // necessary component
                TryAddComponent<TocaObject>(obj);
                TryAddComponent<LayerControl>(obj);

                // add a bottom object
                if (transform.childCount == 0 || !transform.GetChild(0).name.ToLower().Equals("bottom"))
                {
                    GameObject bottom = new GameObject();
                    Bounds bounds;
                    if (obj.GetComponent<Collider2D>())
                        bounds = obj.GetComponent<Collider2D>().bounds;
                    else
                        bounds = new Bounds(obj.transform.position, Vector3.one);
                    bottom.transform.position = bounds.center - bounds.extents.y * Vector3.up;
                    bottom.transform.SetParent(obj.transform);
                    bottom.transform.SetSiblingIndex(0);
                    bottom.name = "Bottom";
                }
            }

            // optional component
            if (Touch)
                TryAddComponent<TouchControl>(obj);
            if (Selection)
            {
                TryAddComponent<SelectionControl>(obj);
                TryAddComponent<PolygonCollider2D>(obj);
                gameObject.layer = LayerMask.NameToLayer("Selection");
            }
            if (Move)
                TryAddComponent<MoveControl>(obj);
                
            if (Find)
            {
                // if there's a bottom attached to this gameobject
                GameObject bottom = transform.GetChild(0).gameObject;
                if (!bottom.GetComponent<FindControl>())
                {
                    Bounds bounds;
                    if (obj.GetComponent<Collider2D>())
                        bounds = obj.GetComponent<Collider2D>().bounds;
                    else
                        bounds = new Bounds(obj.transform.position, Vector3.one);
                    bottom.AddComponent<FindControl>();
                    bottom.AddComponent<BoxCollider2D>();
                    bottom.GetComponent<BoxCollider2D>().size = bounds.size;
                    bottom.GetComponent<BoxCollider2D>().offset = Vector2.up * bounds.extents.y;
                }
            }

            // assume openable object has child properly set up
            if (Open)
            {
                TryAddComponent<OpenControl>(obj);
                int o = 0;
                if (transform.GetChild(0).name.ToLower().Equals("bottom"))
                    o = 1;
                GameObject openObj = transform.GetChild(o).gameObject;
                GameObject closeObj = transform.GetChild(o + 1).gameObject;
                GetComponent<OpenControl>().OpenObj = openObj;
                GetComponent<OpenControl>().CloseObj = closeObj;
                SetUPOpenChild(openObj);
                SetUPOpenChild(closeObj);
            }

            if (Base)
            {
                if (transform.childCount == 0 || !transform.GetComponentInChildren<BaseControl>())
                {
                    GameObject b = new GameObject();
                    b.layer = LayerMask.NameToLayer("Base");
                    b.AddComponent<BaseControl>();
                    b.AddComponent<BoxCollider2D>();
                    b.transform.SetParent(obj.transform);
                    b.transform.localPosition = Vector3.zero;
                    b.name = "Base";
                    b.GetComponent<BaseControl>().SnapWithProp = true;
                    b.GetComponent<BaseControl>().IgnoreLimit = true;

                    GameObject bls = new GameObject();
                    bls.transform.SetParent(b.transform);
                    bls.transform.localPosition = Vector3.zero;
                    bls.AddComponent<BaseLimitSetter>();
                    bls.name = "LimitSetter";
                }
            }
            INIT = false;
        }
    }

    private void SetUPOpenChild(GameObject obj)
    {
        obj.layer = LayerMask.NameToLayer("Selection");
        TryAddComponent<TouchControl>(obj);
        //TryAddComponent<PolygonCollider2D>(obj);
    }

    private void TryAddComponent<T>(GameObject obj) where T : Component
    {
        if (obj.GetComponent<T>() == null)
            obj.AddComponent<T>();
    }
}
