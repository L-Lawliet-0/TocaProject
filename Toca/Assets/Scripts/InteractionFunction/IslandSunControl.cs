using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Experimental.Rendering.Universal;
using Spine;
using Spine.Unity;

public class IslandSunControl : TocaFunction
{
    private bool Transiting;
    private bool IsDay;
    public SkeletonAnimation Island;
    public AnimationReferenceAsset SunToMoon, MoonToSun;

    void Start()
    {
        IsDay = true;
        Transiting = false;
        TouchControl tc = GetComponent<TouchControl>();
        tc.ClickCallBacks.Add(OnClick);
    }

    public void OnClick()
    {
        if (!Transiting)
        {
            Transiting = true;

            TrackEntry track;
            float duration;
            if (IsDay)
            {
                track = Island.AnimationState.SetAnimation(2, SunToMoon, false);
                duration = SunToMoon.Animation.Duration;
            }
            else
            {
                track = Island.AnimationState.SetAnimation(2, MoonToSun, false);
                duration = MoonToSun.Animation.Duration;
            }


            StartCoroutine("ColorLerp", duration / track.TimeScale);
        }
    }

    private IEnumerator ColorLerp(float time)
    {
        float timeSave = time;
        Color targetColor = IsDay ? GlobalParameter.Instance.LightColor_Night : GlobalParameter.Instance.LightColor_Day;
        Color currentColor = GlobalParameter.Instance.GlobalLight.color;

        while (time > 0)
        {
            GlobalParameter.Instance.GlobalLight.color = Color.Lerp(currentColor, targetColor, (timeSave - time) / timeSave);
            time -= Time.deltaTime;
            yield return null;
        }

        Transiting = false;
        GlobalParameter.Instance.GlobalLight.color = targetColor;
        IsDay = !IsDay;
    }
}
