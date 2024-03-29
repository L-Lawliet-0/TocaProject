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
    public bool CanCameraMove { get { return !LoadingCtrl.Instance.LoadingScreenShowing && (!LoadingCtrl.Instance.CurrentShowingObject || !LoadingCtrl.Instance.CurrentShowingObject.activeInHierarchy) && !TrackControl.Instance.LOCK && !CharacterTrack.Instance.LOCK /*&& CameraController.Instance.Sun*/; } }
    public Transform Canvas;

    public SkeletonAnimation OpeningAnimation;
    public Transform SceneSelection;
    public Transform TotaLogo;
    public GameObject MainButton, HomeButton;
    public GameObject PrivacyButton;

    private void Awake()
    {
        m_Instance = this;
    }

    private void Start()
    {
        Application.targetFrameRate = 120;
        // screen setting
        Screen.autorotateToLandscapeLeft = true;
        Screen.autorotateToLandscapeRight = true;
        Screen.autorotateToPortrait = false;
        Screen.autorotateToPortraitUpsideDown = false;
        Screen.orientation = ScreenOrientation.AutoRotation;

        // dont destroy these things
        DontDestroyOnLoad(CharacterTrack.Instance.Track.transform.gameObject);
        DontDestroyOnLoad(gameObject);
        DontDestroyOnLoad(Canvas.gameObject);
        DontDestroyOnLoad(OpeningAnimation.gameObject);
        DontDestroyOnLoad(SceneSelection.gameObject);

        // scale opening animation
        // get camera width and height
        OpeningAnimation.transform.localScale = new Vector3(Mathf.Max(1, CameraController.Instance.Width_Half * 2 / 30) , 1, 1);
        SceneSelection.transform.localScale = new Vector3(Mathf.Max(1, CameraController.Instance.Width_Half * 2 / 30), 1, 1);
        TotaLogo.transform.localScale = new Vector3(Mathf.Max(1, CameraController.Instance.Width_Half * 2 / 30), 1, 1);
        Invoke("Swap", 3);

        //LoadingCtrl.Instance.LoadUIElement(OpeningAnimation.gameObject);
    }

    public void Swap()
    {
        TotaLogo.gameObject.SetActive(false);
        OpeningAnimation.transform.gameObject.SetActive(true);
        PrivacyButton.SetActive(true);
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
        MainButton.SetActive(OpeningAnimation.gameObject.activeInHierarchy);
    }

    public void MainMenuButton()
    {
        SoundManager.Instance.UISfx(10);
        LoadingCtrl.Instance.LoadUIElement(SceneSelection.gameObject);
        PrivacyButton.SetActive(false);
    }

    public void OpenPrivacy()
    {
        Application.OpenURL("https://totagame.github.io/");
    }
}
