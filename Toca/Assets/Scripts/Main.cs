using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using Spine;
using Spine.Unity;

/// <summary>
/// the main control of the whole game
/// </summary>
public class Main : MonoBehaviour
{
    private static Main m_Instance;
    public static Main Instance { get { return m_Instance; } }
    public Transform Canvas;

    public SkeletonAnimation OpeningAnimation;
    public SkeletonAnimation SceneSelection;

    private void Awake()
    {
        m_Instance = this;
    }

    private void Start()
    {
        // dont destroy these things
        DontDestroyOnLoad(CharacterTrack.Instance.Track.transform.gameObject);
        DontDestroyOnLoad(gameObject);
        DontDestroyOnLoad(Canvas.gameObject);

        // scale opening animation
        // get camera width and height
        OpeningAnimation.transform.localScale = new Vector3(CameraController.Instance.Width_Half * 2 / 30 , 1, 1);
        SceneSelection.transform.localScale = new Vector3(CameraController.Instance.Width_Half * 2 / 30, 1, 1);
    }

    private void Update()
    {
        /*
        if (Input.GetKeyDown(KeyCode.Alpha1))
            StartCoroutine("LoadScene",0);
        else if (Input.GetKeyDown(KeyCode.Alpha2))
            StartCoroutine("LoadScene", 1);
        else if (Input.GetKeyDown(KeyCode.Alpha3))
            StartCoroutine("LoadScene", 2);
        */
    }

    public void LoadSceneUI(int index)
    {
        StartCoroutine("LoadScene", index);
    }

    private IEnumerator LoadScene(int sceneIndex)
    {
        AsyncOperation ao = SceneManager.LoadSceneAsync(sceneIndex);
        while (!ao.isDone)
            yield return null;

        // scene finished
        // initialize scene data
        CameraController.Instance.Sun = FindObjectOfType<SunControl>();
        CameraController.Instance.Sun.GlobalLight = GetComponentInChildren<UnityEngine.Experimental.Rendering.Universal.Light2D>();
    }
}
