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

    private void Awake()
    {
        m_Instance = this;

        // initalize characters
        Characters = new List<List<CharacterData>>();
        for (int i = 0; i < 3; i++)
        {
            List<CharacterData> d = new List<CharacterData>();
            for (int j = 0; j < 7; j++)
            {
                CharacterData temp = new CharacterData();
                temp.RandomizeData();
                d.Add(temp);
            }
            Characters.Add(d);
        }
    }

    /// <summary>
    /// swap page, which has the same effect as swiping the page
    /// to left or to the right
    /// </summary>
    /// <param name="left"></param>
    public void SwapPage(bool left)
    {
        PassingHelper pass = new PassingHelper();
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

        StartCoroutine("SpawnHelper2", pass);
    }

    private class PassingHelper
    {
        public int newIndex;
        public Transform Shadow;
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
        }

        yield return new WaitForSeconds(.1f);

        foreach (TocaObject toca in tocas)
            toca.InitalizeTocaobject();
    }

    /// <summary>
    /// spwan a group of spine characters into the scene
    /// </summary>
    /// <param name="index"></param>
    public void SpawnCharacters(int index)
    {
        CurrentActiveGroup = index;
        StartCoroutine("SpawnHelper", index);
    }

    private IEnumerator SpawnHelper(int index)
    {
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
        }

        yield return new WaitForSeconds(.1f);

        foreach (TocaObject toca in tocas)
            toca.InitalizeTocaobject();

        foreach (TocaObject toca in tocas)
        {
            FindControl fc = (FindControl)toca.GetTocaFunction<FindControl>();
            fc.CurrentAttachment = TrackControl.Instance.m_BaseControl;
            fc.Attach(fc.CurrentAttachment);

            LayerControl lc = (LayerControl)toca.GetTocaFunction<LayerControl>();
            lc.ResetLayer(false);
        }

        TrackControl.Instance.CharacterIn();
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
        StartCoroutine("FadeCanvas", active);

        if (active)
        {
            SpawnCharacters(0);
            TrackControl.Instance.CharacterIn();
        }
        else
            TrackControl.Instance.CharacterOut();
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

}
