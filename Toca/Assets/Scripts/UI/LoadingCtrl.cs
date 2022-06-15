using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

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
        if (Loading)
            return;
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

        if (CurrentScene == 1 || CurrentScene == 2 || CurrentScene == 3)
        {
            SunControl sc = FindObjectOfType<SunControl>();
            if (sc && sc.IsDay != IslandSunControl.Instance.IsDay)
                IslandSunControl.Instance.OnClick();

            CharacterTrack.Instance.SavePropsData(true);
            CharacterTrack.Instance.SaveData();
        }

        CharacterTrack.Instance.DestroyCharacters();

        CurrentScene = -1;

        StartCoroutine("FadeLoadingScreen", true);
        while (!LoadingScreenShowing)
            yield return null;

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

        // fake loading
        while (LoadingFill.fillAmount < .99f)
        {
            LoadingFill.fillAmount += Time.deltaTime;
            yield return null;
        }
        LoadingFill.fillAmount = 1;

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
        if (Loading)
            return; // dont do loading if it's already loading
        LoadingFill.fillAmount = 0;
        Loading = true;
        StartCoroutine("LoadScene1", index);
    }

    private IEnumerator FadeLoadingScreen(bool active)
    {
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
        Debug.LogError(Camera.main.ScreenToWorldPoint(new Vector3(Screen.width / 2, Screen.height / 2, GlobalParameter.Depth)));
        
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

    private int CurrentScene = -1; // the current scene index
    private IEnumerator LoadScene1(int sceneIndex)
    {
        StartCoroutine("FadeLoadingScreen",true);

        if (sceneIndex >= 1 && sceneIndex <= 4)
            StartCoroutine("FocusScreen");

        while (!LoadingScreenShowing)
            yield return null;

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
            }
        }

        if (TocaObjectsLoader.Instance)
            TocaObjectsLoader.Instance.InitializeGame();

        // scene is done loading, 

        LoadingFill.fillAmount = 1;
        yield return new WaitForSeconds(.5f);

        // scene finished
        // initialize scene data
        if (sceneIndex != 4)
        {
            CameraController.Instance.Sun = FindObjectOfType<SunControl>();
            CameraController.Instance.ResetWidth(SceneWidth[sceneIndex]);

            CharacterTrack.Instance.SetTrackElement(true);
            CharacterTrack.Instance.SetTrack(false);
            Main.Instance.HomeButton.SetActive(true);

            if (!IslandSunControl.Instance.IsDay)
                CameraController.Instance.Sun.SwitchTime();
        }
        else
            yield return new WaitForSeconds(1f);

        if (sceneIndex == 1)
            GlobalParameter.Instance.GlobalLight.intensity = .75f;
        else
            GlobalParameter.Instance.GlobalLight.intensity = 1;

        StartCoroutine("FadeLoadingScreen", false);

        Loading = false;
        CurrentScene = sceneIndex;
    }
}
