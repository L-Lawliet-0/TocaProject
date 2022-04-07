using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
public class OneTimerTool : MonoBehaviour
{
    public bool EXECUTE;

    void Update()
    {
        if (EXECUTE)
        {
            /*
            // this function enbale all spriterender under openable in children
            for (int i = 0; i < transform.childCount; i++)
            {
                OpenControl oc = transform.GetChild(i).GetComponent<OpenControl>();
                if (oc && oc.CloseObj && oc.CloseObj.GetComponent<SpriteRenderer>())
                    oc.CloseObj.GetComponent<SpriteRenderer>().enabled = false;
            }
            */

            // this function add a base limit setter to all basecontrol in children
            /*
            BaseControl[] bcs = GetComponentsInChildren<BaseControl>(true);
            foreach (BaseControl bc in bcs)
            {
                if (bc.transform.childCount == 0 || !bc.transform.GetChild(0).GetComponent<BaseLimitSetter>())
                {
                    GameObject obj = new GameObject();
                    obj.transform.SetParent(bc.transform);
                    obj.transform.SetSiblingIndex(0);
                    obj.AddComponent<BaseLimitSetter>();
                    obj.layer = LayerMask.NameToLayer("Default");
                    obj.transform.position = bc.transform.position;
                }
            }
            */

            // this function reset bls position
            /*
            BaseLimitSetter[] blss = GetComponentsInChildren<BaseLimitSetter>(true);
            foreach (BaseLimitSetter bls in blss)
            {
                bls.transform.position = bls.transform.parent.position;
            }
            */

            // this function try to set base for all find controls
            /*
            for (int i = 0; i < transform.childCount; i++)
            {
                Transform child = transform.GetChild(i);
                if (child.GetComponentInChildren<FindControl>())
                {
                    if (!child.GetComponent<AutoSetBase>())
                        child.gameObject.AddComponent<AutoSetBase>();
                    // try to set a base for the object
                    child.GetComponent<AutoSetBase>().SET = true; // auto set
                }

            }
            */

            LayerControl[] lcs = FindObjectsOfType<LayerControl>();
            foreach (LayerControl lc in lcs)
            {
                lc.DefaultObjectLayer = LayerControl.SortingLayers.Background;
            }

            EXECUTE = false;
        }    
    }
}
