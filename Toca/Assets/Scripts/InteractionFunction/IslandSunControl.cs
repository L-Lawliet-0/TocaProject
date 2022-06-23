using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Experimental.Rendering.Universal;
using Spine;
using Spine.Unity;

public class IslandSunControl : TocaFunction
{
    private int counter = 2;
    public bool Transiting;
    public bool IsDay;
    public SkeletonAnimation Island;
    public AnimationReferenceAsset SunToMoon, MoonToSun;

    private static IslandSunControl m_Instance;
    public static IslandSunControl Instance { get { return m_Instance; } }

    private void Awake()
    {
        m_Instance = this;
    }

    void Start()
    {
        IsDay = true;
        Transiting = false;
        TouchControl tc = GetComponent<TouchControl>();
        tc.ClickCallBacks.Add(OnClick);
    }

    public IslandAnimation IslandAnimation;
    public void OnClick()
    {
        if (!Transiting)
        {
            IslandAnimation.GetComponent<SkeletonAnimation>().Skeleton.SetAttachment("xingxing", "xingxing");
            Transiting = true;

            TrackEntry track;
            float duration;
            if (IsDay)
            {
                track = Island.AnimationState.SetAnimation(counter++, SunToMoon, false);
                duration = SunToMoon.Animation.Duration;
            }
            else
            {
                track = Island.AnimationState.SetAnimation(counter++, MoonToSun, false);
                duration = MoonToSun.Animation.Duration;
            }
            IsDay = !IsDay;

            if (gameObject.activeInHierarchy)
                StartCoroutine("ColorLerp", duration / track.TimeScale);
            else
            {
                Transiting = false;
                Color targetColor = IsDay ? GlobalParameter.Instance.LightColor_Day : GlobalParameter.Instance.LightColor_Night;
                GlobalParameter.Instance.GlobalLight.color = targetColor;
            }
        }
    }

    private IEnumerator ColorLerp(float time)
    {
        float timeSave = time;
        Color targetColor = IsDay ? GlobalParameter.Instance.LightColor_Day : GlobalParameter.Instance.LightColor_Night;
        Color currentColor = GlobalParameter.Instance.GlobalLight.color;

        while (time > 0)
        {
            GlobalParameter.Instance.GlobalLight.color = Color.Lerp(currentColor, targetColor, (timeSave - time) / timeSave);
            time -= Time.deltaTime;
            yield return null;
        }

        Transiting = false;
        GlobalParameter.Instance.GlobalLight.color = targetColor;
       
    }
}
