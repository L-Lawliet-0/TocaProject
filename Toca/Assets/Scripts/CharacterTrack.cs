using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CharacterTrack : MonoBehaviour
{
    public GameObject CharacterPrefab;
    private static CharacterTrack m_Instance;
    public static CharacterTrack Instance { get { return m_Instance; } }
    public Transform Track, MenuTrack;
    public Transform OpenTrack;

    private List<List<TocaObject.ObjectSaveData>> Characters;
    private List<TocaObject.ObjectSaveData> TrackProps;
    private List<TocaObject> GeneratedProps;
    public int CurrentActiveGroup; // the index of the active 
    public Sprite TrackSprite;

    public bool LOCK; // when locking disable certain action

    Comparer<TocaObject.ObjectSaveData> CurrentComparer;
    public Image[] SortButtons;
    public Sprite Selected, Unselected;

    private void Awake()
    {
        m_Instance = this;

        // initalize characters
    }

    private void Start()
    {
        Characters = SaveManager.LoadTrackData();

        // load bunch of datas from track props
        TrackProps = SaveManager.LoadFromFile(Application.persistentDataPath + "/TrackProps");

        CurrentComparer = new InteractionSort();
        RearrangeDatas(CurrentComparer);
    }

    public void SaveData()
    {
        SaveManager.SaveTrackData(Characters);
    }

    public void SavePropsData(bool writeToFile)
    {
        // get all toca objects under the base
        TocaObject[] tocas = TrackControl.Instance.m_BaseControl.GetComponentsInChildren<TocaObject>();

        foreach (TocaObject toca in tocas)
        {
            if (toca.TocaSave.ObjectID != 0 && toca.TocaSave.PrefabID > 0 && !CompareObjectID(toca.TocaSave, TrackProps))
            {
                TocaObject.ObjectSaveData data = toca.TocaSave;
                data.x = toca.transform.position.x;
                data.y = toca.transform.position.y;
                FindControl fc = (FindControl)toca.GetTocaFunction<FindControl>();
                if (fc.CurrentAttachment)
                {
                    data.Attaching = true;
                    data.ParentObjectID = fc.CurrentAttachment.TocaObject.TocaSave.ObjectID;
                    data.ParentBaseID = fc.CurrentAttachment.BaseID;
                }
                else
                    data.Attaching = false;
                TrackProps.Add(data);
            }
        }

        // save this shit
        if (writeToFile)
            SaveManager.SaveTrackProps(TrackProps);
    }

    public bool CompareObjectID(TocaObject.ObjectSaveData d, List<TocaObject.ObjectSaveData> list)
    {
        foreach (TocaObject.ObjectSaveData to in list)
        {
            if (to.ObjectID == d.ObjectID)
                return true;
        }
        return false;
    }

    public void SpawnProps(List<TocaObject> characters)
    {
        HashSet<int> ids = new HashSet<int>();
        foreach (TocaObject toca in characters)
        {
            ids.Add(toca.TocaSave.ObjectID);
        }

        GeneratedProps = new List<TocaObject>();

        HashSet<int> propIds = new HashSet<int>();
        for (int i = TrackProps.Count - 1; i >= 0; i--)
        {
            if (ids.Contains(TrackProps[i].ParentObjectID))
            {
                // create props
                GameObject prefab = Resources.Load<GameObject>("Prefabs/" + TrackProps[i].PrefabID);
                GameObject obj = Instantiate(prefab); // load objects from resource folder
                obj.GetComponent<TocaObject>().TocaSave = TrackProps[i];
                GeneratedProps.Add(obj.GetComponent<TocaObject>());

                obj.transform.position = new Vector3(0, 500, 50);//out of screen

                propIds.Add(TrackProps[i].ObjectID);

                // remove it from the list
                TrackProps.RemoveAt(i);
            }
        }

        while (propIds.Count > 0)
        {
            HashSet<int> temp = new HashSet<int>();
            for (int i = TrackProps.Count - 1; i >= 0; i--)
            {
                if (propIds.Contains(TrackProps[i].ParentObjectID))
                {
                    // create props
                    GameObject prefab = Resources.Load<GameObject>("Prefabs/" + TrackProps[i].PrefabID);
                    GameObject obj = Instantiate(prefab); // load objects from resource folder
                    obj.GetComponent<TocaObject>().TocaSave = TrackProps[i];
                    GeneratedProps.Add(obj.GetComponent<TocaObject>());

                    temp.Add(TrackProps[i].ObjectID);

                    // remove it from the list
                    TrackProps.RemoveAt(i);
                }
            }
            propIds = temp;
        }
    }

    /// <summary>
    /// swap page, which has the same effect as swiping the page
    /// to left or to the right
    /// </summary>
    /// <param name="left"></param>
    public bool SwapPage(bool left)
    {
        if (LOCK)
            return false;
        LOCK = true;
        SavePropsData(false);
        RearrangeDatas(CurrentComparer);
        PassingHelper pass = new PassingHelper();
        pass.IsLeft = left;

        int max = Characters.Count - 1;
        for (int i = 0; i < PageActive.Count; i++)
        {
            if (!PageActive[i])
            {
                max = i - 1;
                break;
            }
        }

       

        if (left)
        {
            if (CurrentActiveGroup - 1 < 0)
            {
                CurrentActiveGroup = Mathf.Clamp(CurrentActiveGroup, 0, Characters.Count); // clamp active group
                LOCK = false;
                return false;
            }
            pass.newIndex = CurrentActiveGroup - 1;
            pass.Shadow = TrackControl.Instance.CharacterShadowLeft;
        }
        else
        {
            if (CurrentActiveGroup + 1 > max)
            {
                CurrentActiveGroup = Mathf.Clamp(CurrentActiveGroup, 0, Characters.Count); // clamp active group
                LOCK = false;
                return false;
            }
            pass.newIndex = CurrentActiveGroup + 1;
            pass.Shadow = TrackControl.Instance.CharacterShadowRight;
        }
        CurrentActiveGroup = pass.newIndex;
        CurrentActiveGroup = Mathf.Clamp(CurrentActiveGroup, 0, Characters.Count);

        StartCoroutine("SpawnHelper2", pass);
        return true;
    }

    public class InteractionSort : Comparer<TocaObject.ObjectSaveData>
    {
        public override int Compare(TocaObject.ObjectSaveData x, TocaObject.ObjectSaveData y)
        {
            if (y.LastModifiedTime == x.LastModifiedTime)
                return x.My_CharacterData.UNIQUE_ID - y.My_CharacterData.UNIQUE_ID;

            return (int)(y.LastModifiedTime - x.LastModifiedTime);
        }
    }

    public class SelfDressSort : Comparer<TocaObject.ObjectSaveData>
    {
        public HashSet<int> IDs;
        public SelfDressSort(HashSet<int> ids)
        {
            IDs = ids;
        }

        public override int Compare(TocaObject.ObjectSaveData x, TocaObject.ObjectSaveData y)
        {
            if (IDs.Contains(x.My_CharacterData.UNIQUE_ID) && IDs.Contains(y.My_CharacterData.UNIQUE_ID))
                return x.My_CharacterData.UNIQUE_ID - y.My_CharacterData.UNIQUE_ID;

            if (IDs.Contains(x.My_CharacterData.UNIQUE_ID))
                return -1;
            else if (IDs.Contains(y.My_CharacterData.UNIQUE_ID))
                return 1;
            return 0;
        }
    }

    public class RoundHeadFirst : Comparer<TocaObject.ObjectSaveData>
    {
        public override int Compare(TocaObject.ObjectSaveData x, TocaObject.ObjectSaveData y)
        {
            if (x.My_CharacterData.ID_tou.Equals("tou2") && y.My_CharacterData.ID_tou.Equals("tou2"))
                return x.My_CharacterData.UNIQUE_ID - y.My_CharacterData.UNIQUE_ID;

            if (x.My_CharacterData.ID_tou.Equals("tou2"))
                return -1;
            if (y.My_CharacterData.ID_tou.Equals("tou2"))
                return 1;
            return 0;
        }
    }

    public class SquareHeadFirst : Comparer<TocaObject.ObjectSaveData>
    {
        public override int Compare(TocaObject.ObjectSaveData x, TocaObject.ObjectSaveData y)
        {
            if (x.My_CharacterData.ID_tou.Equals("tou") && y.My_CharacterData.ID_tou.Equals("tou"))
                return x.My_CharacterData.UNIQUE_ID - y.My_CharacterData.UNIQUE_ID;

            if (x.My_CharacterData.ID_tou.Equals("tou"))
                return -1;
            if (y.My_CharacterData.ID_tou.Equals("tou"))
                return 1;
            return 0;
        }
    }

    private class PassingHelper
    {
        public int newIndex;
        public Transform Shadow;
        public bool IsLeft;
    }
    
    /// <summary>
    /// spawn but spawn at shdow position
    /// and move camera as well as destroy old character group
    /// </summary>
    /// <param name="index"></param>
    /// <returns></returns>
    private IEnumerator SpawnHelper2(PassingHelper passing)
    {
        // detach all first
        List<TocaObject> olds = TrackControl.Instance.DetachAllAttachments();
        List<TocaObject> tocas = new List<TocaObject>();

        for (int i = 0; i < Characters[passing.newIndex].Count; i++)
        {
            GameObject character = Instantiate(CharacterPrefab);
            TocaObject toca = character.GetComponent<TocaObject>();
            toca.TocaSave = Characters[passing.newIndex][i];
            tocas.Add(toca);

            toca.TocaSave.x = passing.Shadow.GetChild(i).position.x;
            toca.TocaSave.y = passing.Shadow.GetChild(i).position.y;

            toca.TocaSave.Attaching = false;

            toca.transform.position = new Vector3(0, -100); // so it is out of player view

            if (toca.TocaSave.ObjectID == 0)
                toca.TocaSave.ObjectID = toca.GetHashCode();
        }

        

        yield return new WaitForSeconds(.1f);

        foreach (TocaObject toca in tocas)
        {
            toca.InitalizeTocaobject();
            toca.transform.SetParent(Track);
            // disable character first
            ((MoveControl)toca.GetTocaFunction<MoveControl>()).TargetPosition = toca.transform.position;
            toca.gameObject.SetActive(false);
        }
       
        // set toca to base children

        // update position offset to give a page swapping effect
        // target position offset is 30

        float spriteWidth = CameraController.Instance.CamWidth;

        if (passing.IsLeft)
        {
            while (TrackControl.Instance.PositionOffset < spriteWidth)
            {
                TrackControl.Instance.PositionOffset += Time.deltaTime * spriteWidth;
                yield return null;
            }
            TrackControl.Instance.PositionOffset = spriteWidth;
        }
        else
        {
            while (TrackControl.Instance.PositionOffset > -spriteWidth)
            {
                TrackControl.Instance.PositionOffset -= Time.deltaTime * spriteWidth;
                yield return null;
            }
            TrackControl.Instance.PositionOffset = -spriteWidth;
        }

        // disable shadow and enable characters
        foreach (TocaObject toca in tocas)
        {
            toca.gameObject.SetActive(true);
            // attach it to track control and reset layer
            LayerControl lc = (LayerControl)toca.GetTocaFunction<LayerControl>();
            FindControl fc = (FindControl)toca.GetTocaFunction<FindControl>();
            fc.CurrentAttachment = TrackControl.Instance.m_BaseControl;
            fc.Attach(TrackControl.Instance.m_BaseControl);
            lc.ResetLayer(false);
        }

        for (int i = 0; i < passing.Shadow.childCount; i++)
        {
            passing.Shadow.GetChild(i).gameObject.SetActive(false);
        }

        // destroy old characters
        foreach (TocaObject toca in olds)
            Destroy(toca.gameObject);

        float delta = passing.IsLeft ? spriteWidth : -spriteWidth;
        // instant moving so the characters will still be in the center of the camera
        for (int i = 0; i < tocas.Count; i++)
        {
            tocas[i].transform.position += Vector3.right * delta;
        }

        TrackControl.Instance.PositionOffset = 0;

        // reenable shaodws
        for (int i = 0; i < passing.Shadow.childCount; i++)
        {
            passing.Shadow.GetChild(i).gameObject.SetActive(true);
        }

        SpawnProps(tocas);

        yield return null;

        PropLock = true;
        StartCoroutine("InitProps", tocas);
        while (PropLock)
            yield return null;

        LOCK = false;
    }

    private bool allinited(bool[] arr)
    {
        foreach (bool value in arr)
        {
            if (!value)
                return false;
        }
        return true;
    }

    private bool PropLock = false;
    private IEnumerator InitProps(List<TocaObject> tocas)
    {
        bool[] initAll = new bool[GeneratedProps.Count];

        for (int i = 0; i < GeneratedProps.Count; i ++)
        {
            //((MoveControl)GeneratedProps[i].GetTocaFunction<MoveControl>()).InstantGo = true;
            foreach (TocaObject par in tocas)
            {
                if (par.TocaSave.ObjectID == GeneratedProps[i].TocaSave.ParentObjectID)
                {
                    GeneratedProps[i].InitalizeTocaobject(par);
                    initAll[i] = true;
                    break;
                }
            }
        }

        yield return new WaitForSeconds(.1f);

        while (!allinited(initAll))
        {
            for (int i = 0; i < GeneratedProps.Count; i++)
            {
                if (!initAll[i])
                {
                    foreach (TocaObject par in GeneratedProps)
                    {
                        if (par.TocaSave.ObjectID == GeneratedProps[i].TocaSave.ParentObjectID)
                        {
                            if (par.GetTocaFunction<StandControl>() || par.GetComponent<StandControl>())
                            {
                                StandControl stand = (StandControl)par.GetTocaFunction<StandControl>();
                                if (!stand)
                                    stand = par.GetComponent<StandControl>();
                                if (stand.BaseControl)
                                {
                                    stand.BaseControl.gameObject.SetActive(true);
                                    stand.ForceDown();
                                }
                            }
                            GeneratedProps[i].InitalizeTocaobject(par);
                            initAll[i] = true;
                            break;
                        }
                    }
                }
            }

            yield return new WaitForSeconds(.1f);
        }

        PropLock = false;
    }

    /// <summary>
    /// spwan a group of spine characters into the scene
    /// </summary>
    /// <param name="index"></param>
    public void SpawnCharacters(int index)
    {
        Debug.LogError("not lock 2");
        if (LOCK)
            return;
        LOCK = true;
        RearrangeDatas(CurrentComparer);
        CurrentActiveGroup = index;
        CurrentActiveGroup = Mathf.Clamp(CurrentActiveGroup, 0, Characters.Count);
        StartCoroutine("SpawnHelper", index);
        Debug.LogError("not lock 3");
    }

    private IEnumerator SpawnHelper(int index)
    {
        List<TocaObject> olds = TrackControl.Instance.DetachAllAttachments();
        foreach (TocaObject toca in olds)
            Destroy(toca.gameObject);

        float bound = CameraController.Instance.Width_Half + 2;
        Vector3 leftPos = new Vector3(CameraController.Instance.transform.position.x - bound, TrackControl.Instance.m_BaseControl.transform.position.y, GlobalParameter.Depth);
        Vector3 rightPos = new Vector3(CameraController.Instance.transform.position.x + bound, TrackControl.Instance.m_BaseControl.transform.position.y, GlobalParameter.Depth);

        List<TocaObject> tocas = new List<TocaObject>();
        for (int i = 0; i < Characters[index].Count; i++)
        {
            GameObject character = Instantiate(CharacterPrefab);
            TocaObject toca = character.GetComponent<TocaObject>();
            toca.TocaSave = Characters[index][i];
            tocas.Add(toca);

            if (i < Characters[index].Count / 2)
            {
                toca.TocaSave.x = leftPos.x;
                toca.TocaSave.y = leftPos.y;
            }
            else
            {
                toca.TocaSave.x = rightPos.x;
                toca.TocaSave.y = rightPos.y;
            }

            toca.TocaSave.Attaching = false;

            toca.transform.position = new Vector3(0, 100);
            if (toca.TocaSave.ObjectID == 0)
                toca.TocaSave.ObjectID = toca.GetHashCode();
        }

        // spawn objects
        SpawnProps(tocas);

        yield return new WaitForSeconds(.1f);

        foreach (TocaObject toca in tocas)
        {
            toca.InitalizeTocaobject();
        }

        PropLock = true;
        StartCoroutine("InitProps", tocas);

        while (PropLock)
            yield return null;

        foreach (TocaObject toca in tocas)
        {
            FindControl fc = (FindControl)toca.GetTocaFunction<FindControl>();
            fc.CurrentAttachment = TrackControl.Instance.m_BaseControl;
            fc.Attach(fc.CurrentAttachment);

            LayerControl lc = (LayerControl)toca.GetTocaFunction<LayerControl>();
            lc.ResetLayer(false);
        }

        TrackControl.Instance.CharacterIn();
        LOCK = false;
    }

    public void UpdateCharacters(List<TocaObject> tocas)
    {
        if (LOCK)
            return;

        Characters[CurrentActiveGroup].Clear();
        foreach (TocaObject toca in tocas)
        {
            Characters[CurrentActiveGroup].Add(toca.TocaSave);
        }

        if (Characters[CurrentActiveGroup].Count <= 0)
        {
            if (Characters.Count > 1)
            {
                //int temp = CurrentActiveGroup;
                Characters.RemoveAt(CurrentActiveGroup);
                SwapPage(CurrentActiveGroup > 0);
            }
        }
    }

    private void Update()
    {
        //Track.transform.position = new Vector3(CameraController.Instance.transform.position.x, Track.transform.position.y, Track.transform.position.z);
        Debug.LogError("Length : " + Characters.Count);
        for (int i = 0; i < Characters.Count; i++)
            Debug.LogError(Characters[i].Count);
    }

    public void SetTrackElement(bool active)
    {
        OpenTrack.gameObject.SetActive(active);
        MenuTrack.gameObject.SetActive(active);
        Track.gameObject.SetActive(active);
    }

    public void SetTrack(bool active)
    {
        if (TrackControl.Instance.LOCK)
            return;

        Debug.LogError("not lock");
        TrackControl.Instance.m_BaseControl.IgnoreLimit = active;
        StartCoroutine("FadeCanvas", active);

        if (active)
        {
            SpawnCharacters(0);
            SoundManager.Instance.UISfx(9);
        }
        else
        {
            SavePropsData(false);
            TrackControl.Instance.CharacterOut();
        }
    }

    public void DestroyCharacters()
    {
        List<TocaObject> olds = TrackControl.Instance.DetachAllAttachments();
        foreach (TocaObject toca in olds)
            Destroy(toca.gameObject);
    }

    private IEnumerator FadeCanvas(bool active)
    {
        Debug.LogError("not lock1");
        int menuSign = active ? 1 : -1;
        int openSign = active ? -1 : 1;

        CanvasGroup track_Cg = MenuTrack.GetComponent<CanvasGroup>();
        CanvasGroup open_Cg = OpenTrack.GetComponent<CanvasGroup>();

        track_Cg.interactable = active;
        open_Cg.interactable = !active;
        track_Cg.blocksRaycasts = active;
        open_Cg.blocksRaycasts = !active;

        float counter = 1;
        float speed = 1.805f * 2f;

        while (counter > 0)
        {
            if (LoadingCtrl.Instance.LoadingScreenShowing)
                break;

            CameraController.Instance.transform.position += Vector3.up * openSign * speed * Time.deltaTime;
            track_Cg.alpha += menuSign * Time.deltaTime;
            open_Cg.alpha += openSign * Time.deltaTime;

            counter -= Time.deltaTime;
            yield return null;
        }

        CameraController.Instance.transform.position = new Vector3(CameraController.Instance.transform.position.x, active ? 7.68f - speed : 7.68f, CameraController.Instance.transform.position.z);
        track_Cg.alpha = active ? 1 : 0;
        open_Cg.alpha = active ? 0 : 1;
    }

    public void SortCharacters(int method)
    {
        if (LOCK || TrackControl.Instance.LOCK)
            return; // dont sort if locked

        LOCK = true;

        Comparer<TocaObject.ObjectSaveData> comparer = null;
        switch (method)
        {
            case 0:
                comparer = new InteractionSort();
                break;
            case 1:
                comparer = new SelfDressSort(CharacterSelectionCtrl.GetIDs());
                break;
            case 2:
                comparer = new RoundHeadFirst();
                break;
            case 3:
                comparer = new SquareHeadFirst();
                break;
        }

        if (CurrentComparer != null && comparer.GetType() == CurrentComparer.GetType())
        {
            LOCK = false;
            return;
        }

        SavePropsData(false);
        RearrangeDatas(comparer);
        CurrentComparer = comparer;
        for (int i = 0; i < SortButtons.Length; i++)
        {
            SortButtons[i].sprite = i == method ? Selected : Unselected;
        }
        CurrentActiveGroup = 0;
        StartCoroutine("SpawnHelper3");
    }

    public void ForceSort()
    {
        Comparer<TocaObject.ObjectSaveData> comparer = new SelfDressSort(CharacterSelectionCtrl.GetIDs());
        RearrangeDatas(comparer);
        CurrentComparer = comparer;
        for (int i = 0; i < SortButtons.Length; i++)
        {
            SortButtons[i].sprite = i == 1 ? Selected : Unselected;
        }
        CurrentActiveGroup = 0;
    }

    private IEnumerator SpawnHelper3()
    {
        // detach all first
        List<TocaObject> olds = TrackControl.Instance.DetachAllAttachments();

        // destroy old characters
        foreach (TocaObject toca in olds)
            Destroy(toca.gameObject);

        List<TocaObject> tocas = new List<TocaObject>();

        float startPos = TrackControl.Instance.transform.position.x - (Characters[CurrentActiveGroup].Count - 1) * 3.5f / 2;

        for (int i = 0; i < Characters[CurrentActiveGroup].Count; i++)
        {
            GameObject character = Instantiate(CharacterPrefab);
            TocaObject toca = character.GetComponent<TocaObject>();
            toca.TocaSave = Characters[CurrentActiveGroup][i];
            tocas.Add(toca);

            toca.TocaSave.x = startPos + i * 3.5f;
            toca.TocaSave.y = TrackControl.Instance.m_BaseControl.transform.position.y;

            toca.TocaSave.Attaching = false;

            toca.transform.position = new Vector3(0, -100); // so it is out of player view

            if (toca.TocaSave.ObjectID == 0)
                toca.TocaSave.ObjectID = toca.GetHashCode();
        }

        SpawnProps(tocas);

        yield return new WaitForSeconds(.1f);

        foreach (TocaObject toca in tocas)
        {
            toca.InitalizeTocaobject();
            toca.transform.SetParent(Track);
            // disable character first
            ((MoveControl)toca.GetTocaFunction<MoveControl>()).TargetPosition = toca.transform.position;
            toca.gameObject.SetActive(false);
        }
        
        // disable shadow and enable characters
        foreach (TocaObject toca in tocas)
        {
            toca.gameObject.SetActive(true);
            // attach it to track control and reset layer
            LayerControl lc = (LayerControl)toca.GetTocaFunction<LayerControl>();
            FindControl fc = (FindControl)toca.GetTocaFunction<FindControl>();
            fc.CurrentAttachment = TrackControl.Instance.m_BaseControl;
            fc.Attach(TrackControl.Instance.m_BaseControl);
            lc.ResetLayer(false);
        }

        PropLock = true;
        StartCoroutine("InitProps", tocas);
        while (PropLock)
            yield return null;

        

        LOCK = false;
    }

    // This section defines page logic

    public const int CountPerpage = 6; // standard count per page

    public List<bool> PageActive;

    public void RearrangeDatas(Comparer<TocaObject.ObjectSaveData> comparer = null)
    {
        PageActive = new List<bool>();
        bool pageActive = true;
        List<TocaObject.ObjectSaveData> all = new List<TocaObject.ObjectSaveData>();
        for (int i = 0; i < Characters.Count; i++)
        {
            for (int j = 0; j < Characters[i].Count; j++)
            {
                all.Add(Characters[i][j]);
            }
        }

        Characters.Clear();
        List<TocaObject.ObjectSaveData> temp = new List<TocaObject.ObjectSaveData>();

        if (comparer != null)
            all.Sort(comparer);

        bool reached = true;
        if (comparer != null && comparer.GetType() != typeof(InteractionSort))
            reached = false;

        for (int i = 0; i < all.Count; i++)
        {
            if (!reached)
            {
                bool condition = false;
                if (comparer.GetType() == typeof(SelfDressSort) && !((SelfDressSort)comparer).IDs.Contains(all[i].My_CharacterData.UNIQUE_ID))
                    condition = true;
                else if (comparer.GetType() == typeof(RoundHeadFirst) && !all[i].My_CharacterData.ID_tou.Equals("tou2"))
                    condition = true;
                else if (comparer.GetType() == typeof(SquareHeadFirst) && !all[i].My_CharacterData.ID_tou.Equals("tou"))
                    condition = true;

                if (condition)
                {
                    reached = true;
                    pageActive = false;
                    if (temp.Count > 0 || Characters.Count == 0)
                    {
                        Characters.Add(temp);
                        temp = new List<TocaObject.ObjectSaveData>();
                        PageActive.Add(true);
                    }
                }
            }

            temp.Add(all[i]);
            if (temp.Count >= CountPerpage || i == all.Count - 1)
            {
                Characters.Add(temp);
                temp = new List<TocaObject.ObjectSaveData>();
                PageActive.Add(pageActive);
            }
        }

        if (Characters.Count == 0)
        {
            Characters.Add(new List<TocaObject.ObjectSaveData>());
            PageActive.Add(true);
        }
    }

    public void AddData(CharacterData data)
    {
        TocaObject.ObjectSaveData toca = new TocaObject.ObjectSaveData();
        toca.PrefabID = -50;
        toca.My_CharacterData = data;
        if (Characters.Count == 0)
            Characters.Add(new List<TocaObject.ObjectSaveData>());
        Characters[0].Add(toca);
        RearrangeDatas(CurrentComparer);
        SaveData();
    }

    public void TryUpdateTrack(CharacterData data)
    {
        for (int i = 0; i < Characters.Count; i++)
        {
            for (int j = 0; j < Characters[i].Count; j++)
            {
                if (Characters[i][j].My_CharacterData.UNIQUE_ID == data.UNIQUE_ID)
                {
                    Characters[i][j].My_CharacterData = data;
                }
            }
        }

        SaveData();
    }

    /// <summary>
    /// add character datas from other scene into the track
    /// </summary>
    /// <param name="thisScene"></param>
    public void AddDataFromOtherScene(int thisScene)
    {
        for (int i = 1; i < 4; i++)
        {
            if (i != thisScene) // don't add any data from this scene
            {
                List<TocaObject.ObjectSaveData> datas = new List<TocaObject.ObjectSaveData>();
                if (i == 1)
                    datas = SaveManager.LoadFromFile(Application.persistentDataPath + "/gongzhufang");
                else if (i == 2)
                    datas = SaveManager.LoadFromFile(Application.persistentDataPath + "/haijunfeng");
                else if (i == 3)
                    datas = SaveManager.LoadFromFile(Application.persistentDataPath + "/nanhaifang");

                // add datas to characters
                foreach (TocaObject.ObjectSaveData data in datas)
                {
                    if (data.PrefabID == SaveManager.CharacterID)
                    {
                        Characters[0].Add(data);
                    }
                }
            }
        }
    }

    /// <summary>
    /// assuming scene data is up to date
    /// remove character data from other scene if it stays in this scene
    /// remove character data from the track if it's in any scene
    /// </summary>
    /// <param name="thisScene"></param>
    public void TrimData(int thisScene)
    {
        List<TocaObject.ObjectSaveData> characters = new List<TocaObject.ObjectSaveData>();
        List<TocaObject.ObjectSaveData> sceneData = new List<TocaObject.ObjectSaveData>();
        if (thisScene == 1)
            sceneData = SaveManager.LoadFromFile(Application.persistentDataPath + "/gongzhufang");
        else if (thisScene == 2)
            sceneData = SaveManager.LoadFromFile(Application.persistentDataPath + "/haijunfeng");
        else if (thisScene == 3)
            sceneData = SaveManager.LoadFromFile(Application.persistentDataPath + "/nanhaifang");

        foreach (TocaObject.ObjectSaveData data in sceneData)
        {
            if (data.PrefabID == SaveManager.CharacterID)
                characters.Add(data);
        }

        for (int i = 1; i < 4; i++)
        {
            if (i != thisScene) // don't add any data from this scene
            {
                List<TocaObject.ObjectSaveData> datas;
                string path = "";
                if (i == 1)
                    path = Application.persistentDataPath + "/gongzhufang";
                else if (i == 2)
                    path = Application.persistentDataPath + "/haijunfeng";
                else if (i == 3)
                    path =  Application.persistentDataPath + "/nanhaifang";
                datas = SaveManager.LoadFromFile(path);

                // traverse data and remove data
                for (int j = datas.Count - 1; j >= 0; j--)
                {
                    if (CompareID(datas[j], characters))
                    {
                        datas.RemoveAt(j);
                    }
                }

                SaveManager.SaveToFile(path, datas);
            }
        }

        // removed character data from the track
        string[] paths = new string[]
        {
            Application.persistentDataPath + "/gongzhufang",
            Application.persistentDataPath + "/haijunfeng",
            Application.persistentDataPath + "/nanhaifang"
        };
        List<TocaObject.ObjectSaveData> sceneCharacters = new List<TocaObject.ObjectSaveData>();
        foreach (string path in paths)
        {
            List<TocaObject.ObjectSaveData> temp = SaveManager.LoadFromFile(path);
            foreach (TocaObject.ObjectSaveData data in temp)
            {
                if (data.PrefabID == SaveManager.CharacterID)
                    sceneCharacters.Add(data);
            }
        }

        // trim character data
        for (int i = 0; i < Characters.Count; i++)
        {
            for (int j = Characters[i].Count - 1; j >= 0; j --)
            {
                if (CompareID(Characters[i][j], sceneCharacters))
                {
                    Characters[i].RemoveAt(j);
                }
            }
        }
    }

    public bool CompareID(TocaObject.ObjectSaveData d, List<TocaObject.ObjectSaveData> list)
    {
        if (d.PrefabID != SaveManager.CharacterID)
            return false;

        foreach(TocaObject.ObjectSaveData to in list)
        {
            if (to.My_CharacterData.UNIQUE_ID == d.My_CharacterData.UNIQUE_ID)
                return true;
        }
        return false;
    }
}
