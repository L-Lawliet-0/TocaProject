using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using UnityEngine.Experimental.Rendering.Universal;
using System.Runtime.Serialization.Formatters.Binary;

[ExecuteInEditMode]
public class OneTimerTool : MonoBehaviour
{
    public bool EXECUTE;

    public CharacterData[] CharacterReferences;

    void Update()
    {
        if (EXECUTE)
        {
            //ResetLayers();
            //GroupObjectsWithAttachingBase();
            //AutoSetObjectsBase();
            //GroupSaveableObjects();
            //RenameChildren();
            //AssignPrefabID();
            //SaveSceneData();
            //FindObjectsNameWithComponent();
            //GroupFindControlWithoutBases();
            //GroupBases();
            //CreateUIitems();
            //RearrangeUIChildren();
            //GenerateNewTrackData();
            //GenerateNewSelectionData();
            //GenerateNewTrackSaveData();
            //InitializeSingleObjectSave();
            //ReAssignPrefabsID();
            //InitRef();
            GetAdjustedVolume();
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

    private void GroupFindControlWithoutBases()
    {
        TocaObject[] tocas = FindObjectsOfType<TocaObject>(true);
        foreach (TocaObject toca in tocas)
        {
            if (toca.GetComponentInChildren<AutoSetBase>() && !toca.GetComponentInChildren<AutoSetBase>().Reference)
            {
                toca.transform.SetParent(TargetParent);
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

    private void FindObjectsNameWithComponent()
    {
        Light2D[] lights = FindObjectsOfType<Light2D>(true);
        foreach (Light2D l in lights)
        {
            Debug.LogError(l.name);
        }

    }

    private void SortChildBasedOnOrder()
    {
        List<GameObject> objs = new List<GameObject>();
        for (int i = 0; i < transform.childCount; i++)
            objs.Add(transform.GetChild(i).gameObject);
        objs.Sort(new HeightCompare());

        for (int i = 0; i < objs.Count; i++)
        {
            objs[i].transform.SetSiblingIndex(i);
        }
    }

    public class HeightCompare : Comparer<GameObject>
    {
        public override int Compare(GameObject x, GameObject y)
        {
            return x.GetComponent<SpriteRenderer>().sortingOrder - y.GetComponent<SpriteRenderer>().sortingOrder;
        }
    }

    private void CreateUIitems()
    {
        for (int i = transform.childCount + 1; i < 22; i++)
        {
            GameObject newobj = Instantiate(transform.GetChild(0).gameObject);
            RectTransform rect = newobj.GetComponent<RectTransform>();
            rect.SetParent(transform);
            rect.localPosition = transform.GetChild(0).GetComponent<RectTransform>().localPosition + new Vector3(238 * (i % 5), -248 * (i / 5));
        }
    }

    private void RearrangeUIChildren()
    {
        Vector3 startLocal = transform.GetChild(0).localPosition;

        int[] newOrder = new int[]
        {
            2,1,3,6,17,
            16,8,9,7,14,
            18,20,21,15,22,
            5,13,12,11,10,
            4,19
        };

        int[] newOrder2 = new int[]
        {
            9, 22, 1, 2, 5,
            4, 25, 11, 20, 19,
            16, 7, 3, 10, 23,
            24, 15, 6, 8, 21,
            17, 13, 14, 12, 18
        };

        Dictionary<int, Transform> dict = new Dictionary<int, Transform>();

        for (int i = 0; i < transform.childCount; i++)
            dict.Add(newOrder2[i], transform.GetChild(i));

        List<int> keys = new List<int>(dict.Keys);
        keys.Sort();
 
        for (int i = 0; i < keys.Count; i++)
        {
            dict[keys[i]].SetSiblingIndex(i);
        }

        for (int i = 0; i < transform.childCount; i++)
        {
           transform.GetChild(i).localPosition = startLocal + new Vector3(238 * (i % 5), -248 * (i / 5));
        }
    }

    private void GroupSaveableObjects()
    {
        TocaObject[] tocas = FindObjectsOfType<TocaObject>(true);
        foreach (TocaObject toca in tocas)
        {
            if (toca.GetComponentInChildren<FindControl>(true) && toca.GetComponentInChildren<MoveControl>(true))
            {
                toca.transform.SetParent(TargetParent);
            }
        }
    }

    private void RenameChildren()
    {
        int startName = 247;

        for (int i = 0; i < transform.childCount; i++)
        {
            int name = startName + i;
            transform.GetChild(i).name = name.ToString();
        }
    }

    public int startID;
    private void AssignPrefabID()
    {
        for (int i = 0; i < transform.childCount; i++)
        {
            if (transform.GetChild(i).GetComponent<SpineControl>())
                transform.GetChild(i).GetComponent<TocaObject>().TocaSave.PrefabID = SaveManager.CharacterID;
            else
                transform.GetChild(i).GetComponent<TocaObject>().TocaSave.PrefabID = startID + i;
        }
    }

    public string fileName;
    private void SaveSceneData()
    {
        // create save
        List<TocaObject.ObjectSaveData> datas = new List<TocaObject.ObjectSaveData>();
        for (int i = 0; i < transform.childCount; i++)
        {
            datas.Add(transform.GetChild(i).GetComponent<TocaObject>().TocaSave);
        }

        string dataPath = Application.persistentDataPath + "/" + fileName;

        using (Stream file = File.Open(dataPath, FileMode.OpenOrCreate))
        {
            BinaryFormatter bf = new BinaryFormatter();
            bf.Serialize(file, datas);
        }
    }

    private List<TocaObject.ObjectSaveData> LoadFromFile()
    {
        // if (!File.Exists(Application.persistentDataPath + "/save"))
            

        using (Stream file = File.Open(Application.persistentDataPath + "/save", FileMode.Open))
        {
            BinaryFormatter bf = new BinaryFormatter();
            return bf.Deserialize(file) as List<TocaObject.ObjectSaveData>;
        }
    }

    private void GenerateNewTrackData()
    {
        // generate 7 characters at the start of the game
        List<List<TocaObject.ObjectSaveData>> Characters = new List<List<TocaObject.ObjectSaveData>>();
        for (int i = 0; i < 1; i++)
        {
            List<TocaObject.ObjectSaveData> d = new List<TocaObject.ObjectSaveData>();
            for (int j = 0; j < 6; j++)
            {
                TocaObject.ObjectSaveData toca = new TocaObject.ObjectSaveData();
                toca.PrefabID = SaveManager.CharacterID;
                toca.My_CharacterData = CharacterReferences[j];
                d.Add(toca);
            }
            Characters.Add(d);
        }

        using (Stream file = File.Open(Application.persistentDataPath + "/CharacterTrack", FileMode.OpenOrCreate))
        {
            BinaryFormatter bf = new BinaryFormatter();
            bf.Serialize(file, Characters);
        }
    }

    private void GenerateNewSelectionData()
    {
        CharacterData[] datas = new CharacterData[CharacterSelectionCtrl.MaxCharacters];

        using (Stream file = File.Open(Application.persistentDataPath + "/CharacterCreation", FileMode.OpenOrCreate))
        {
            BinaryFormatter bf = new BinaryFormatter();
            bf.Serialize(file, datas);
        }
    }

    private void GenerateNewTrackSaveData()
    {
        List<TocaObject.ObjectSaveData> datas = new List<TocaObject.ObjectSaveData>();

        using (Stream file = File.Open(Application.persistentDataPath + "/TrackProps", FileMode.OpenOrCreate))
        {
            BinaryFormatter bf = new BinaryFormatter();
            bf.Serialize(file, datas);
        }
    }

    private void InitializeSingleObjectSave()
    {
        TocaObject toca = GetComponent<TocaObject>();
        toca.TocaSave.ObjectID = toca.GetHashCode();
        toca.TocaSave.x = toca.transform.position.x;
        toca.TocaSave.y = toca.transform.position.y;

        if (toca.TocaSave.PrefabID == SaveManager.CharacterID && toca.TocaSave.My_CharacterData.UNIQUE_ID == 0)
        {
            toca.TocaSave.My_CharacterData.UNIQUE_ID = toca.TocaSave.My_CharacterData.GetHashCode();
        }
    }

    private void ReAssignPrefabsID()
    {
        for (int i = 1; i < 347; i++)
        {
            GameObject obj = Resources.Load<GameObject>("Prefabs/" + i.ToString());
            obj.GetComponent<TocaObject>().TocaSave.PrefabID = i;
        }
    }

    private void InitRef()
    {
        for (int i = 0; i < 12; i++)
        {
            //CharacterReferences[i].ID_skinColor -= 1;
            //CharacterReferences[i].ID_hairColor -= 1;
            CharacterReferences[i].UNIQUE_ID = CharacterReferences[i].GetHashCode();
        }
    }

    public AudioClip TestClip;
    public float CalculateClipLoudness(AudioClip testClip)
    {
        int sampleSize = testClip.frequency;
        Debug.LogError(sampleSize);
        float[] datas = new float[sampleSize];
        bool vv = testClip.GetData(datas, sampleSize);

        float loudness = 0;
        foreach (float value in datas)
        {
            loudness += Mathf.Abs(value);
        }

        loudness /= sampleSize;

        return loudness;
    }

    public void GetAdjustedVolume()
    {
        float referenceVolume = 0.03049393f;
        float thisVolume = CalculateClipLoudness(TestClip);
        Debug.LogError(.5f * (referenceVolume / thisVolume));
    }
}
