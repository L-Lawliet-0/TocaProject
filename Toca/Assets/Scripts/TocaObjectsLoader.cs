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
    public bool INITDATA;
    public bool Initialized;

    private void Awake()
    {
        if (Application.isPlaying)
        {
            m_Instance = this;
            TocaObjectsPool = new Dictionary<int, TocaObject>();
            TocaObject[] all = FindObjectsOfType<TocaObject>();
            foreach (TocaObject toca in all)
            {
                TocaObjectsPool.Add(toca.TocaSave.ObjectID, toca);
            }
            Initialized = false;
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

        if (Application.isPlaying && !Initialized)
        {
            foreach (KeyValuePair<int, TocaObject> pair in TocaObjectsPool)
            {
                pair.Value.InitalizeTocaobject();
            }
            Initialized = true;
        }
    }
}
