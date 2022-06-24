using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SoundManager : MonoBehaviour
{
    private static SoundManager m_Instance;
    public static SoundManager Instance { get { return m_Instance; } }

    public AudioSource BackGroundMusic;

    public AudioClip[] BGMs;
    public AudioClip[] SFXs;

    public float[] Volume_BGMs;
    public float[] Volume_SFXs;

    public GameObject SFXprefab;

    private void Awake()
    {
        m_Instance = this;
        BackGroundMusic.volume = 0;
    }

    private void Start()
    {
        PlayBGM(0);
    }

    public void PlayBGM(int index)
    {
        BackGroundMusic.clip = BGMs[index];
        BackGroundMusic.volume = Volume_BGMs[index];
        StartCoroutine("EnableAudio", BackGroundMusic);
    }

    public void StopBGM()
    {
        StartCoroutine("DisableAudio", BackGroundMusic);
    }

    private IEnumerator EnableAudio(AudioSource audio)
    {
        audio.Play();
        while (audio.volume < .95f)
        {
            audio.volume += Time.deltaTime;
            yield return null;
        }
        if (!audio.isPlaying)
            audio.Play();
    }

    private IEnumerator DisableAudio(AudioSource audio)
    {
        while (audio.volume > .05f)
        {
            audio.volume -= Time.deltaTime;
            yield return null;
        }
        audio.Stop();
    }

    public GameObject PlaySFX(int index, bool autoDestroy, Vector3 pos)
    {
        if (LoadingCtrl.Instance.Loading)
            return null; // don't play any sfx when loading 
        GameObject obj = Instantiate(SFXprefab);
        obj.transform.position = pos;

        AudioSource audio = obj.GetComponent<AudioSource>();
        audio.loop = !autoDestroy;
        audio.clip = SFXs[index];
        audio.volume = SFXoverrideVolume;//Volume_SFXs[index];

        audio.Play();

        if (autoDestroy)
            Destroy(obj, audio.clip.length);

        return obj;
    }

    public void UISfx(int index)
    {
        PlaySFX(index, true, transform.position);
    }

    public Text VolumeText, VolumeText_SFX;
    public float SFXoverrideVolume = 1;

    public void OnVolumeChange(float value)
    {
        VolumeText.text = value.ToString();
        BackGroundMusic.volume = value;
    }

    public void OnVolumeChange2(float value)
    {
        VolumeText_SFX.text = value.ToString();
        SFXoverrideVolume = value;
    }
}
