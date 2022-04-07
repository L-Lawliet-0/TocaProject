using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
public class Cleaner : MonoBehaviour
{
    public bool CLEAN;

    private void Update()
    {
        if (CLEAN)
        {
            for (int i = transform.childCount - 1; i >= 0; i--)
                Deparent(transform.GetChild(i));
            CLEAN = false;

            for (int i = transform.childCount - 1; i >= 0; i--)
            {
                if (!transform.GetChild(i).GetComponent<SpriteRenderer>())
                    DestroyImmediate(transform.GetChild(i).gameObject);
                else
                {
                    // clean other components under this object
                    Component[] components = transform.GetChild(i).GetComponents<Component>();
                    for (int j = components.Length - 1; j >= 0; j--)
                    {
                        if (components[j].GetType() != typeof(SpriteRenderer) && components[j].GetType() != typeof(Transform))
                        {
                            DestroyImmediate(components[j]);
                        }
                    }
                }
            }
        }
    }

    private void Deparent(Transform tran)
    {
        for (int i = tran.childCount - 1; i >= 0; i--)
            Deparent(tran.GetChild(i));
        tran.parent = transform;
    }
}
