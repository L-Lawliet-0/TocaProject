using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;

[ExecuteInEditMode]
public class OneTimerTool : MonoBehaviour
{
    public bool EXECUTE;

    void Update()
    {
        if (EXECUTE)
        {
            //GroupObjectsWithAttachingBase();
            AutoSetObjectsBase();
            EXECUTE = false;
        }    
    }

    private void SetSpriteRendererInOpenable(bool active)
    {
        // this function enbale all spriterender under openable in children
        OpenControl[] opens = FindObjectsOfType<OpenControl>();

        foreach (OpenControl oc in opens)
        {
            if (oc && oc.CloseObj && oc.CloseObj.GetComponent<SpriteRenderer>())
                oc.CloseObj.GetComponent<SpriteRenderer>().enabled = active;
        }
    }

    private void AddLimitSetter()
    {
        // this function add a base limit setter to all basecontrol
        BaseControl[] bcs = FindObjectsOfType<BaseControl>(true);
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
    }

    private void ResetBlsPos()
    {
        // this function reset all bls position
        BaseLimitSetter[] blss = FindObjectsOfType<BaseLimitSetter>(true);
        foreach (BaseLimitSetter bls in blss)
        {
            bls.transform.position = bls.transform.parent.position;
        }
    }

    private void AutoSetObjectsBase()
    {
        // this function try to set base for all find controls
        FindControl[] finds = FindObjectsOfType<FindControl>();
        foreach (FindControl find in finds)
        {
            if (!find.GetComponent<AutoSetBase>())
                find.gameObject.AddComponent<AutoSetBase>();
            // try to set a base for the object
            find.GetComponent<AutoSetBase>().SET = true; // auto set
        }
    }

    private void ResetLayers()
    {
        // this function try to reset layers
        LayerControl[] lcs = FindObjectsOfType<LayerControl>(true);
        foreach (LayerControl lc in lcs)
        {
            lc.DefaultObjectLayer = LayerControl.SortingLayers.Background;
        }

        SpriteRenderer[] srs = FindObjectsOfType<SpriteRenderer>(true);
        foreach (SpriteRenderer sr in srs)
        {
            sr.sortingOrder = 0; // reset sorting order
            sr.sortingLayerName = "Default";
        }
    }

    public Transform TargetParent;
    private void GroupBases()
    {
        TocaObject[] tocas = FindObjectsOfType<TocaObject>(true);
        foreach (TocaObject toca in tocas)
        {
            if (toca.GetComponentInChildren<BaseControl>(true))
            {
                toca.transform.SetParent(TargetParent);
            }
        }
    }

    private void GroupTocaObjects()
    {
        TocaObject[] tocas = FindObjectsOfType<TocaObject>(true);
        foreach (TocaObject toca in tocas)
        {
            toca.transform.SetParent(TargetParent);
        }
    }

    private void GroupBaseWithCover()
    {
        TocaObject[] tocas = FindObjectsOfType<TocaObject>(true);
        foreach (TocaObject toca in tocas)
        {
            BaseControl[] bcs = toca.GetComponentsInChildren<BaseControl>();
            foreach (BaseControl bc in bcs)
            {
                if (bc.MyBaseAttributes.HaveCover)
                {
                    toca.transform.SetParent(TargetParent);
                    break;
                }
            }
        }
    }

    private void ReinitializeAllTotas()
    {
        TotaObjectInitalizer[] totas = FindObjectsOfType<TotaObjectInitalizer>(true);
        foreach (TotaObjectInitalizer tota in totas)
        {
            tota.INIT = true;
        }
    }

    private void GenerateBottoms()
    {
        TocaObject[] tocas = FindObjectsOfType<TocaObject>(true);
        foreach (TocaObject toca in tocas)
        {
            if (toca.transform.childCount == 0 || !toca.transform.GetChild(0).name.ToLower().Equals("bottom"))
            {
                GameObject bottom = new GameObject();
                Bounds bounds;
                if (toca.GetComponent<SpriteRenderer>())
                {
                    // get thte bottom of the sprite position
                    bounds = toca.GetComponent<SpriteRenderer>().bounds;
                }
                else if (toca.GetComponent<Collider2D>())
                    bounds = toca.GetComponent<Collider2D>().bounds;
                else
                    bounds = new Bounds(toca.transform.position, Vector3.one);
                bottom.transform.position = bounds.center - bounds.extents.y * Vector3.up;
                bottom.transform.SetParent(toca.transform);
                bottom.transform.SetSiblingIndex(0);
                bottom.name = "Bottom";
            }
        }
    }

    public Transform PSBtemplate;
    private void ReplaceSprites()
    {
        Dictionary<string, Sprite> pairs = new Dictionary<string, Sprite>();
        string path = Application.dataPath + "/Resources/RoomOfPrincess/419";
        DirectoryInfo dir = new DirectoryInfo(path);
        FileInfo[] info = dir.GetFiles("*.*");
        foreach (FileInfo f in info)
        {
            // generate objects here
            if (!f.Name.Contains("meta")) // ignore meta file
            {
                // trim name
                string name = f.Name.Split('.')[0];
                pairs.Add(name, Resources.Load<Sprite>("RoomOfPrincess/419/" + name));
            }
        }

        // find all objects with sprite renderer in the current scene
        SpriteRenderer[] sprites = FindObjectsOfType<SpriteRenderer>(true);

        foreach (SpriteRenderer sprite in sprites)
        {
            if (pairs.ContainsKey(sprite.sprite.name))
            {
                sprite.sprite = pairs[sprite.sprite.name]; // swap sprite
            }
        }
    }

    private void AdjustPositions()
    {
        SpriteRenderer[] sprites = FindObjectsOfType<SpriteRenderer>();
        foreach (SpriteRenderer sprite in sprites)
        {
            if (sprite.GetComponent<TocaObject>())
            {
                for (int i = 0; i < PSBtemplate.childCount; i++)
                {
                    if (PSBtemplate.GetChild(i).GetComponent<SpriteRenderer>().sprite == sprite.sprite)
                    {
                        sprite.transform.position = PSBtemplate.GetChild(i).position;
                        break;
                    }
                }
            }
        }
    }

    public BaseControl BigBase;
    private FindControl[] FCs;

    private void GroupObjectsWithAttachingBase()
    {
        FCs = FindObjectsOfType<FindControl>();
        Helper(BigBase);
    }

    private void Helper(BaseControl reference)
    {
        foreach (FindControl fc in FCs)
        {
            if (fc.GetComponent<AutoSetBase>() && fc.GetComponent<AutoSetBase>().Reference == reference)
            {
                // this object is moved, try to adjust its position
                TocaObject toca = fc.GetComponentInParent<TocaObject>();
                if (toca)
                {
                    toca.transform.SetParent(TargetParent);

                    BaseControl bc = toca.GetComponentInChildren<BaseControl>();
                    // there's a base control under this object, call helper again
                    if (bc)
                        Helper(bc);
                }
            }
        }
    }

   
}
