using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using System;

public class LoadingCtrl : MonoBehaviour
{
    private static LoadingCtrl m_Instance;
    public static LoadingCtrl Instance { get { return m_Instance; } }

    private CanvasGroup m_CanvasGroup;
    public Image LoadingFill;
    public bool Loading;
    public bool LoadingScreenShowing { get { return m_CanvasGroup.interactable; } }

    public GameObject CurrentShowingObject; // the current, if any, UI spine object shwoing
    public GameObject PendingObject; // the pending object, if any, ui spine object shwoing

    private void Awake()
    {
        m_Instance = this;
        m_CanvasGroup = GetComponent<CanvasGroup>();
        SetActive(false); // set loading screen to false
        Loading = false;
    }

    public void SetActive(bool active)
    {
        m_CanvasGroup.alpha = active ? 1 : 0;
        m_CanvasGroup.interactable = active;
        m_CanvasGroup.blocksRaycasts = active;
    }

    public void LoadUIElement(GameObject pendingObject)
    {
        if (Loading || (CameraController.Instance.Sun && CameraController.Instance.Sun.Switching))
            return;

        if (CameraController.Instance.Sun)
            CameraController.Instance.Sun.Switching = true;

        Loading = true;
        LoadingFill.fillAmount = 0;
        PendingObject = pendingObject;
        StartCoroutine("LoadElement");
    }

    private IEnumerator LoadElement()
    {
        if (CurrentScene == 1)
            SaveManager.SaveCurrentScene(Application.persistentDataPath + "/gongzhufang");
        else if (CurrentScene == 2)
            SaveManager.SaveCurrentScene(Application.persistentDataPath + "/haijunfeng");
        else if (CurrentScene == 3)
            SaveManager.SaveCurrentScene(Application.persistentDataPath + "/nanhaifang");

        CharacterTrack.Instance.TrimData(CurrentScene); // trim data

        bool switchTime = false;

        if (CurrentScene == 1 || CurrentScene == 2 || CurrentScene == 3)
        {
            SunControl sc = FindObjectOfType<SunControl>();
            if (sc && sc.IsDay != IslandSunControl.Instance.IsDay)
                switchTime = true;

            CharacterTrack.Instance.SavePropsData(true);
            CharacterTrack.Instance.SaveData();
        }

        CurrentScene = -1;

        StartCoroutine("FadeLoadingScreen", true);
        while (!LoadingScreenShowing)
            yield return null;

        CharacterTrack.Instance.DestroyCharacters();

        // set object in hierarchy
        if (CurrentShowingObject)
            CurrentShowingObject.SetActive(false);
        if (PendingObject)
            PendingObject.SetActive(true);
        CurrentShowingObject = PendingObject;
        CameraController.Instance.ResetCamera();

        CharacterTrack.Instance.SetTrackElement(false);
        Main.Instance.HomeButton.SetActive(false);
        SceneManager.LoadScene(5);

        SoundManager.Instance.PlayBGM(5);

        // fake loading
        while (LoadingFill.fillAmount < .99f)
        {
            LoadingFill.fillAmount += Time.deltaTime;
            yield return null;
        }
        LoadingFill.fillAmount = 1;

        if (switchTime)
            IslandSunControl.Instance.OnClick();

        StartCoroutine("FadeLoadingScreen", false);
        while (LoadingScreenShowing)
            yield return null;

        Loading = false;
       
    }

    /// <summary>
    /// Load a actual scene
    /// </summary>
    public void LoadScene(int index)
    {
        if (Loading || IslandSunControl.Instance.Transiting)
            return; // dont do loading if it's already loading
        LoadingFill.fillAmount = 0;
        Loading = true;
        StartCoroutine("LoadScene1", index);
    }

    private IEnumerator FadeLoadingScreen(bool active)
    {
        if (active)
            SoundManager.Instance.StopBGM();
        int sign = active ? 1 : -1;
        float counter = 2;
        float speed = 1 / counter;

        while (counter > 0)
        {
            m_CanvasGroup.alpha += sign * Time.deltaTime * speed;
            counter -= Time.deltaTime;
            yield return null;
        }

        SetActive(active);
    }

    private static Dictionary<int, float> SceneWidth = new Dictionary<int, float>()
    {
        {1, 90f },
        {2, 69.59f },
        {3, 90f }
    };

    public Vector3 FocusPosition;
    private IEnumerator FocusScreen()
    {
        // move the center to the center and expand the screen
        Vector3 scaleSave = Main.Instance.SceneSelection.transform.localScale;
        Vector3 direction = Camera.main.ScreenToWorldPoint(new Vector3(Screen.width / 2, Screen.height / 2, GlobalParameter.Depth)) - FocusPosition;
        
        direction.z = 0;

        float speed = direction.magnitude / .5f;

        direction = direction.normalized;
        
        

        float counter = .5f;
        while (counter > 0)
        {
            Main.Instance.SceneSelection.transform.localScale += Vector3.one * Time.deltaTime * 4;
            Main.Instance.SceneSelection.transform.position += direction * Time.deltaTime * speed * Main.Instance.SceneSelection.localScale.y;
            counter -= Time.deltaTime;
            yield return null;
        }

        while (Main.Instance.SceneSelection.gameObject.activeInHierarchy)
            yield return null;
        Main.Instance.SceneSelection.transform.position = Vector3.up * 7.68f;
        Main.Instance.SceneSelection.localScale = scaleSave;
    }

    public int CurrentScene = -1; // the current scene index
    private IEnumerator LoadScene1(int sceneIndex)
    {
        StartCoroutine("FadeLoadingScreen",true);

        if (sceneIndex >= 1 && sceneIndex <= 4)
            StartCoroutine("FocusScreen");

        while (!LoadingScreenShowing)
            yield return null;

        CharacterTrack.Instance.DestroyCharacters();
        CharacterTrack.Instance.SetTrackElement(false);

        if (CurrentShowingObject)
            CurrentShowingObject.SetActive(false);

        AsyncOperation ao = SceneManager.LoadSceneAsync(sceneIndex);
        while (!ao.isDone)
        {
            LoadingFill.fillAmount = ao.progress;
            yield return null;
        }

        // load dynamic object into the scene
        List<TocaObject.ObjectSaveData> datas = null;
        if (sceneIndex == 1)
            datas = SaveManager.LoadFromFile(Application.persistentDataPath + "/gongzhufang");
        else if (sceneIndex == 2)
            datas = SaveManager.LoadFromFile(Application.persistentDataPath + "/haijunfeng");
        else if (sceneIndex == 3)
            datas = SaveManager.LoadFromFile(Application.persistentDataPath + "/nanhaifang");

        CharacterTrack.Instance.AddDataFromOtherScene(sceneIndex);

        List<TocaObject> tocas = new List<TocaObject>();

        if (datas != null)
        {
            foreach (TocaObject.ObjectSaveData data in datas)
            {
                GameObject prefab;
                if (data.PrefabID == SaveManager.CharacterID)
                    prefab = CharacterTrack.Instance.CharacterPrefab;
                else
                    prefab = Resources.Load<GameObject>("Prefabs/" + data.PrefabID);
                GameObject obj = Instantiate(prefab); // load objects from resource folder
                obj.GetComponent<TocaObject>().TocaSave = data;

                obj.transform.position = new Vector3(data.x, data.y, GlobalParameter.Depth);

                tocas.Add(obj.GetComponent<TocaObject>());
            }
        }

        CharacterTrack.Instance.SpawnProps(tocas);

        if (TocaObjectsLoader.Instance)
            TocaObjectsLoader.Instance.InitializeGame();

        // scene is done loading, 

        LoadingFill.fillAmount = 1;

        if (sceneIndex >= 1 && sceneIndex <= 3)
            yield return new WaitForSeconds(2);
        else
            yield return new WaitForSeconds(.5f);

        // scene finished
        // initialize scene data
        if (sceneIndex != 4)
        {
            CameraController.Instance.Sun = FindObjectOfType<SunControl>();
            CameraController.Instance.ResetWidth(SceneWidth[sceneIndex]);

            TrackControl.Instance.Reset();
            CharacterTrack.Instance.SetTrackElement(true);

            if (!OpenTrack)
                CharacterTrack.Instance.SetTrack(false);
            Main.Instance.HomeButton.SetActive(true);

            if (!IslandSunControl.Instance.IsDay)
                CameraController.Instance.Sun.SwitchTime();
        }
        else
        {
            CharacterSelectionCtrl.Instance.PreScene = CurrentScene;
            yield return new WaitForSeconds(1f);
        }

        

        if (sceneIndex == 1)
            GlobalParameter.Instance.GlobalLight.intensity = .75f;
        else
            GlobalParameter.Instance.GlobalLight.intensity = 1;

        StartCoroutine("FadeLoadingScreen", false);

        Loading = false;
        CurrentScene = sceneIndex;

        if (OpenTrack && sceneIndex >= 1 && sceneIndex <= 3)
        {
            Debug.LogError("set track true");
            OpenTrack = false;
            CharacterTrack.Instance.SetTrack(true);
        }

        SoundManager.Instance.PlayBGM(sceneIndex);

        AdsManager.Instance.ShowAds();
    }

    private bool OpenTrack = false;
    public void LoadSceneFromScene(int newScene)
    {
        if (Loading)
            return;
        LoadingFill.fillAmount = 0;
        Loading = true;

        if (CurrentScene == 1)
            SaveManager.SaveCurrentScene(Application.persistentDataPath + "/gongzhufang");
        else if (CurrentScene == 2)
            SaveManager.SaveCurrentScene(Application.persistentDataPath + "/haijunfeng");
        else if (CurrentScene == 3)
            SaveManager.SaveCurrentScene(Application.persistentDataPath + "/nanhaifang");

        CharacterTrack.Instance.TrimData(CurrentScene);

        if (CurrentScene == 1 || CurrentScene == 2 || CurrentScene == 3)
        {
            SunControl sc = FindObjectOfType<SunControl>();
            if (sc && sc.IsDay != IslandSunControl.Instance.IsDay)
                IslandSunControl.Instance.OnClick();

            CharacterTrack.Instance.SavePropsData(true);
            CharacterTrack.Instance.SaveData();
        }
        
        Main.Instance.HomeButton.SetActive(false);
        if (newScene != 4)
            CharacterTrack.Instance.ForceSort();

        OpenTrack = true;

        StartCoroutine("LoadScene1", newScene);
    }

    public IList<string> LetterCombinations(string digits)
    {
        Dictionary<char, char[]> Map = new Dictionary<char, char[]>();
        Map.Add('2', new char[] { 'a', 'b', 'c' });
        Map.Add('3', new char[] { 'd', 'e', 'f' });
        Map.Add('4', new char[] { 'g', 'h', 'i' });
        Map.Add('5', new char[] { 'j', 'k', 'l' });
        Map.Add('6', new char[] { 'm', 'n', 'o' });
        Map.Add('7', new char[] { 'p', 'q', 'r', 's' });
        Map.Add('8', new char[] { 't', 'u', 'v' });
        Map.Add('9', new char[] { 'w', 'x', 'y', 'z' });

        List<string> values = new List<string>();

        if (digits.Length > 0)
            Append(Map, "", values, digits, 0);

        return values;
    }

    private void Append(Dictionary<char, char[]> map, string result, List<string> returnValues, string digits, int index)
    {
        if (index >= digits.Length)
        {
            returnValues.Add(result);
            return;
        }

        char digit = digits[index];
        char[] list = map[digit];

        foreach (char letter in list)
        {
            result += letter;
            Append(map, result, returnValues, digits, index + 1);
            result = result.Substring(0, result.Length - 1);
        }
    }

    public bool IsBoomerang(int[][] points)
    {
        // case where x are all equal
        if (points[0][0] == points[1][0] && points[1][0] == points[2][0])
            return false;

        // check all points distinct
        if (ComparePoint(points[0], points[1]))
            return false;
        if (ComparePoint(points[1], points[2]))
            return false;
        if (ComparePoint(points[0], points[2]))
            return false;

        float slope1, slope2, slope3;

        if (points[1][0] == points[0][0])
            return true;
        else
            slope1 = (float)(points[1][1] - points[0][1]) / (float)(points[1][0] - points[0][0]);

        if (points[2][0] == points[0][0])
            return true;
        else
            slope2 = (float)(points[2][1] - points[0][1]) / (float)(points[2][0] - points[0][0]);

        if (points[2][0] == points[1][0])
            return true;
        else
            slope3 = (float)(points[2][1] - points[1][1]) / (float)(points[2][0] - points[1][0]);

        return Math.Abs(slope1) != Math.Abs(slope2) || Math.Abs(slope2) != Math.Abs(slope3) || Math.Abs(slope1) != Math.Abs(slope3);
    }

    public bool ComparePoint(int[] p1, int[] p2)
    {
        return p1[0] == p2[0] && p1[1] == p1[1];
    }

    public class MyCalendarThree
    {
        private List<(int, int)> Ranges;
        private List<int> Counts;
        public MyCalendarThree()
        {
            Ranges = new List<(int, int)>();
            Counts = new List<int>();
        }

        public int Book(int start, int end)
        {
            Counts.Add(1);

            for (int i = 0; i < Ranges.Count; i++)
            {
                // iterate through the ranges
                if (InRange((start, end), Ranges[i]))
                {
                    Counts[i]++;
                    Counts[Counts.Count - 1]++;
                }
            }

            Ranges.Add((start, end));
            int max = Counts[0];
            foreach (int count in Counts)
            {
                if (count > max)
                    max = count;
            }
            return max;
        }

        // does two range have itersection
        private bool InRange((int, int) t1, (int, int) t2)
        {
           
            return !(t2.Item1 >= t1.Item2 || t1.Item1 >= t2.Item2);
        }
    }

}
