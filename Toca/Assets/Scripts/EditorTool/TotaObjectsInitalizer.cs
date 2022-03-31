using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
public class TotaObjectsInitalizer : MonoBehaviour
{
    public bool INIT;
    public bool REVERT;

    private void Update()
    {
        if (INIT)
        {
            for (int i = 0; i < transform.childCount; i++)
            {
                GameObject obj = transform.GetChild(i).gameObject;
                // add necessary toca functions
                obj.AddComponent<TocaObject>();
                obj.AddComponent<LayerControl>();
                obj.AddComponent<TouchControl>();
                obj.AddComponent<SelectionControl>();
                obj.AddComponent<MoveControl>();
                obj.AddComponent<PolygonCollider2D>();
                obj.layer = LayerMask.NameToLayer("Selection");

                // add find control and attach it to bottom
                GameObject bottom = new GameObject();
                Bounds bounds = obj.GetComponent<PolygonCollider2D>().bounds;
                bottom.transform.position = bounds.center - bounds.extents.y * Vector3.up;
                bottom.transform.SetParent(obj.transform);
                bottom.transform.SetSiblingIndex(0);
                bottom.AddComponent<FindControl>();
                bottom.AddComponent<BoxCollider2D>();
                bottom.GetComponent<BoxCollider2D>().size = bounds.size;
                bottom.GetComponent<BoxCollider2D>().offset = Vector2.up * bounds.extents.y;
                bottom.name = "Bottom";
            }
            INIT = false;
        }
        else if (REVERT)
        {
            for (int i = 0; i < transform.childCount; i++)
            {
                GameObject obj = transform.GetChild(i).gameObject;
                if (obj.GetComponent<TocaObject>())
                {
                    DestroyImmediate(obj.GetComponent<TocaObject>());
                    DestroyImmediate(obj.GetComponent<LayerControl>());
                    DestroyImmediate(obj.GetComponent<TouchControl>());
                    DestroyImmediate(obj.GetComponent<SelectionControl>());
                    DestroyImmediate(obj.GetComponent<MoveControl>());
                    DestroyImmediate(obj.GetComponent<PolygonCollider2D>());

                    DestroyImmediate(obj.transform.GetChild(0).gameObject);
                }
            }
            REVERT = false;
        }

    }
}
