using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Experimental.Rendering.Universal;

public class SunControl : TocaFunction
{
    public Color DayColor, NightColor;
    public bool IsDay; // is the scene currently in day time
    public bool Switching; // is the scene currently switching between day and night
    public Transform CenterPoint;
    public float Radius;
    public GameObject Sun, Moon;
    public SpriteRenderer[] Days, Nights;

    public float[] ActiveRanges;

    private void Start()
    {
        TouchControl tc = (TouchControl)TocaObject.GetTocaFunction<TouchControl>();
        tc.ClickCallBacks.Add(SwitchTime);
        SetSpritesColor(Nights, new Color(1, 1, 1, 0));
        SetSpritesColor(Days, Color.white);
    }

    private void SetSpritesColor(SpriteRenderer[] array, Color color)
    {
        foreach(SpriteRenderer s in array)
        {
            s.color = color;
        }
    }

    private void Update()
    {
        UpdateMajorX();
        gameObject.layer = InActiveRange() ? LayerMask.NameToLayer("Selection") : LayerMask.NameToLayer("Default");
    }

    public void UpdateMajorX()
    {
        float min = -CameraController.Instance.testWidthValue / 2 + 2;
        float max = CameraController.Instance.testWidthValue / 2 - 2;


        float clamp = Mathf.Lerp(min, max, CameraController.Instance.GetCameraOffset());
        CenterPoint.position = new Vector3(clamp, CenterPoint.position.y, CenterPoint.position.z);
        if (!Switching)
            transform.position = CenterPoint.position + new Vector3(Radius * Mathf.Cos(90 * Mathf.Deg2Rad), Radius * Mathf.Sin(90 * Mathf.Deg2Rad));
    }

    private bool InActiveRange()
    {
        bool pass = false;
        for (int i = 0; i < ActiveRanges.Length; i += 2)
        {
            if (transform.position.x >= ActiveRanges[i] && transform.position.x <= ActiveRanges[i + 1])
                pass = true;
        }

        return pass;
    }

    // switch from night to day or day to night
    public void SwitchTime()
    {
        if (!Switching)
        {
            

            Switching = true;
            IsDay = !IsDay;
            StartCoroutine("Switch");
        }
    }

    private IEnumerator Switch()
    {
        Color currentColor = GlobalParameter.Instance.GlobalLight.color;
        Color targetColor = IsDay ? DayColor : NightColor;
        float angle = 90;
        float counter = 0;
        bool switched = false;

        float sign = IsDay ? 1 : -1;
        Color dayColor = IsDay ? new Color(1, 1, 1, 0) : Color.white;
        Color nightColor = IsDay ? Color.white : new Color(1, 1, 1, 0);

        while (angle > -270)
        {
            counter += Time.deltaTime;
            angle -= Time.deltaTime * 90; // we rotate this fucker 30 degree per second
            transform.position = CenterPoint.position + new Vector3(Radius * Mathf.Cos(angle * Mathf.Deg2Rad), Radius * Mathf.Sin(angle * Mathf.Deg2Rad));
            GlobalParameter.Instance.GlobalLight.color = Color.Lerp(currentColor, targetColor, counter / 2);
            if (angle < 0 && !switched)
            {
                switched = true;
                Sun.SetActive(IsDay);
                Moon.SetActive(!IsDay);
                angle = -180;
            }

            dayColor += sign * new Color(0, 0, 0, 1) * Time.deltaTime / 2;
            nightColor -= sign * new Color(0, 0, 0, 1) * Time.deltaTime / 2;
            SetSpritesColor(Days, dayColor);
            SetSpritesColor(Nights, nightColor);

            yield return null;
        }

        GlobalParameter.Instance.GlobalLight.color = targetColor;
        Switching = false;

        if (IsDay)
        {
            SetSpritesColor(Days, Color.white);
            SetSpritesColor(Nights, new Color(1, 1, 1, 0));
        }
        else
        {
            SetSpritesColor(Days, new Color(1, 1, 1, 0));
            SetSpritesColor(Nights, Color.white);
        }

        Sun.SetActive(IsDay);
        Moon.SetActive(!IsDay);
    }
}
