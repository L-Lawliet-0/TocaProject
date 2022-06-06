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

    private IEnumerator LoadScene1(int sceneIndex)
    {
        StartCoroutine("FadeLoadingScreen",true);
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

        // scene finished
        // initialize scene data
        CameraController.Instance.Sun = FindObjectOfType<SunControl>();
        CameraController.Instance.Sun.GlobalLight = GetComponentInChildren<UnityEngine.Experimental.Rendering.Universal.Light2D>();

        CharacterTrack.Instance.SetTrackElement(true);
        CharacterTrack.Instance.SetTrack(false);
        Main.Instance.HomeButton.SetActive(true);

        StartCoroutine("FadeLoadingScreen", false);

        Loading = false;
    }
}
