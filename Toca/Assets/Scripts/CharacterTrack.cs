using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterTrack : MonoBehaviour
{
    public GameObject CharacterPrefab;
    private static CharacterTrack m_Instance;
    public static CharacterTrack Instance { get { return m_Instance; } }
    public Transform Track, MenuTrack;
    public Transform OpenTrack;

    private List<List<CharacterData>> Characters;
    public int CurrentActiveGroup; // the index of the active 
    public Sprite TrackSprite;

    public bool LOCK; // when locking disable certain action

    private void Awake()
    {
        m_Instance = this;

        // initalize characters
    }

    private void Start()
    {
        Characters = SaveManager.LoadTrackData();
    }

    public void SaveData()
    {
        SaveManager.SaveTrackData(Characters);
    }

    /// <summary>
    /// swap page, which has the same effect as swiping the page
    /// to left or to the right
    /// </summary>
    /// <param name="left"></param>
    public void SwapPage(bool left)
    {
        if (LOCK)
            return;
        LOCK = true;
        RearrangeDatas();
        PassingHelper pass = new PassingHelper();
        pass.IsLeft = left;
        if (left)
        {
            pass.newIndex = CurrentActiveGroup - 1 < 0 ? Characters.Count - 1 : CurrentActiveGroup - 1;
            pass.Shadow = TrackControl.Instance.CharacterShadowLeft;
        }
        else
        {
            pass.newIndex = CurrentActiveGroup + 1 > Characters.Count - 1 ? 0 : CurrentActiveGroup + 1;
            pass.Shadow = TrackControl.Instance.CharacterShadowRight;
        }
        CurrentActiveGroup = pass.newIndex;
        CurrentActiveGroup = Mathf.Clamp(CurrentActiveGroup, 0, Characters.Count);

        StartCoroutine("SpawnHelper2", pass);
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
            toca.TocaSave.My_CharacterData = Characters[passing.newIndex][i];
            tocas.Add(toca);

            toca.TocaSave.x = passing.Shadow.GetChild(i).position.x;
            toca.TocaSave.y = passing.Shadow.GetChild(i).position.y;

            toca.transform.position = new Vector3(0, -100); // so it is out of player view

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

        LOCK = false;
    }

    /// <summary>
    /// spwan a group of spine characters into the scene
    /// </summary>
    /// <param name="index"></param>
    public void SpawnCharacters(int index)
    {
        if (LOCK)
            return;
        LOCK = true;
        RearrangeDatas();
        CurrentActiveGroup = index;
        CurrentActiveGroup = Mathf.Clamp(CurrentActiveGroup, 0, Characters.Count);
        StartCoroutine("SpawnHelper", index);
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
            toca.TocaSave.My_CharacterData = Characters[index][i];
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
            toca.transform.position = new Vector3(0, 100);
            toca.TocaSave.ObjectID = toca.GetHashCode();
        }

        yield return new WaitForSeconds(.1f);

        foreach (TocaObject toca in tocas)
        {
            toca.InitalizeTocaobject();
        }

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
            Characters[CurrentActiveGroup].Add(toca.TocaSave.My_CharacterData);
        }

        if (Characters[CurrentActiveGroup].Count <= 0)
        {
            if (Characters.Count > 1)
            {
                //int temp = CurrentActiveGroup;
                Characters.RemoveAt(CurrentActiveGroup);
                SwapPage(false);
            }
        }
    }

    private void Update()
    {
        //Track.transform.position = new Vector3(CameraController.Instance.transform.position.x, Track.transform.position.y, Track.transform.position.z);
    }

    public void SetTrackElement(bool active)
    {
        OpenTrack.gameObject.SetActive(active);
        MenuTrack.gameObject.SetActive(active);
        Track.gameObject.SetActive(active);
    }

    public void SetTrack(bool active)
    {
        TrackControl.Instance.m_BaseControl.IgnoreLimit = active;
        StartCoroutine("FadeCanvas", active);

        if (active)
        {
            SpawnCharacters(0);
        }
        else
        {
            TrackControl.Instance.CharacterOut();
        }
    }

    private IEnumerator FadeCanvas(bool active)
    {
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

    // This section defines page logic

    public const int CountPerpage = 7; // standard count per page

    public void RearrangeDatas()
    {
        List<CharacterData> all = new List<CharacterData>();
        for (int i = 0; i < Characters.Count; i++)
        {
            for (int j = 0; j < Characters[i].Count; j++)
            {
                all.Add(Characters[i][j]);
            }
        }

        Characters.Clear();
        List<CharacterData> temp = new List<CharacterData>();
        for (int i = 0; i < all.Count; i++)
        {
            temp.Add(all[i]);
            if (temp.Count >= 7 || i == all.Count - 1)
            {
                Characters.Add(temp);
                temp = new List<CharacterData>();
            }
        }

        if (Characters.Count == 0)
            Characters.Add(new List<CharacterData>());
    }

    public void AddData(CharacterData data)
    {
        if (Characters.Count == 0)
            Characters.Add(new List<CharacterData>());
        Characters[0].Add(data);
        RearrangeDatas();
        SaveManager.SaveTrackData(Characters);
    }

    public void TryUpdateTrack(CharacterData data)
    {
        for (int i = 0; i < Characters.Count; i++)
        {
            for (int j = 0; j < Characters[i].Count; j++)
            {
                if (Characters[i][j].UNIQUE_ID == data.UNIQUE_ID)
                {
                    Characters[i][j] = data;
                }
            }
        }

        SaveManager.SaveTrackData(Characters);
    }
}
