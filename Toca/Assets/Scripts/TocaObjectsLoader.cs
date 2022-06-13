using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// a tempoary loader to load and save objects within a scene
[ExecuteInEditMode]
public class TocaObjectsLoader : MonoBehaviour
{
    private static TocaObjectsLoader m_Instance;
    public static TocaObjectsLoader Instance { get { return m_Instance; } }

    public Dictionary<int, TocaObject> TocaObjectsPool;
    public List<TocaObject> PublicPool;
    public bool INITDATA;
    public bool Initialized;

    private void Awake()
    {
        m_Instance = this;
    }

    public void InitializeGame()
    {
        TocaObjectsPool = new Dictionary<int, TocaObject>();
        TocaObject[] all = FindObjectsOfType<TocaObject>();
        PublicPool = new List<TocaObject>();
        foreach (TocaObject toca in all)
        {
            if (!toca.GetComponentInParent<TrackControl>())
            {
                PublicPool.Add(toca);
                if (!TocaObjectsPool.ContainsKey(toca.TocaSave.ObjectID))
                    TocaObjectsPool.Add(toca.TocaSave.ObjectID, toca);
            }
        }
        StartCoroutine("Init");
    }

    private IEnumerator Init()
    {
        yield return new WaitForSeconds(.1f);
        foreach (KeyValuePair<int, TocaObject> pair in TocaObjectsPool)
        {
            pair.Value.InitalizeTocaobject();
        }
    }

    public void InitializeAllTocaObjects()
    {
        TocaObject[] all = FindObjectsOfType<TocaObject>();
        foreach (TocaObject toca in all)
            toca.InitalizeSave();

        BaseControl[] bcs = FindObjectsOfType<BaseControl>();
        foreach (BaseControl bc in bcs)
            bc.BaseID = bc.GetHashCode();
    }

    private void Update()
    {
        if (INITDATA)
        {
            InitializeAllTocaObjects();
            INITDATA = false;
        }
    }
    
}
