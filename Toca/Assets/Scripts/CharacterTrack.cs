using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterTrack : MonoBehaviour
{
    private static CharacterTrack m_Instance;
    public static CharacterTrack Instance { get { return m_Instance; } }
    public Transform Track, MenuTrack;
    public Transform OpenTrack;

    private void Awake()
    {
        m_Instance = this;
    }

    private void Update()
    {
        //Track.transform.position = new Vector3(CameraController.Instance.transform.position.x, Track.transform.position.y, Track.transform.position.z);
    }

    public void SetTrack(bool active)
    {
        StartCoroutine("FadeCanvas", active);
        if (active)
            TrackControl.Instance.CharacterIn();
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
}
