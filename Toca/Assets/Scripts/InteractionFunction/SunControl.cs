using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Experimental.Rendering.Universal;

public class SunControl : TocaFunction
{
    public Light2D GlobalLight;
    public Color DayColor, NightColor;
    public bool IsDay; // is the scene currently in day time
    public bool Switching; // is the scene currently switching between day and night
    public Transform CenterPoint;
    public float Radius;
    public GameObject Sun, Moon;
    public SpriteRenderer Day, Night;

    private void Start()
    {
        TouchControl tc = (TouchControl)TocaObject.GetTocaFunction<TouchControl>();
        tc.ClickCallBacks.Add(SwitchTime);
        Night.color = new Color(1, 1, 1, 0);
        Day.color = Color.white;
    }

    public void UpdateMajorX(float x)
    {
        CenterPoint.position += x * Vector3.right;
        float min = CameraController.Instance.transform.position.x - CameraController.Instance.Width_Half;
        float max = CameraController.Instance.transform.position.x + CameraController.Instance.Width_Half;
        float clamp = Mathf.Clamp(CenterPoint.position.x, min + 2, max - 2);
        CenterPoint.position = new Vector3(clamp, CenterPoint.position.y, CenterPoint.position.z);
        if (!Switching)
            transform.position = CenterPoint.position + new Vector3(Radius * Mathf.Cos(90 * Mathf.Deg2Rad), Radius * Mathf.Sin(90 * Mathf.Deg2Rad));
    }

    // switch from night to day or day to night
    public void SwitchTime()
    {
        if (!Switching)
        {
            Switching = true;
            StartCoroutine("Switch");
        }
    }

    private IEnumerator Switch()
    {
        Color currentColor = GlobalLight.color;
        Color targetColor = IsDay ? NightColor : DayColor;
        float angle = 90;
        float counter = 0;
        bool switched = false;

        float sign = IsDay ? -1 : 1;

        while (angle > -270)
        {
            counter += Time.deltaTime;
            angle -= Time.deltaTime * 90; // we rotate this fucker 30 degree per second
            transform.position = CenterPoint.position + new Vector3(Radius * Mathf.Cos(angle * Mathf.Deg2Rad), Radius * Mathf.Sin(angle * Mathf.Deg2Rad));
            GlobalLight.color = Color.Lerp(currentColor, targetColor, counter / 2);
            if (angle < 0 && !switched)
            {
                switched = true;
                Sun.SetActive(!IsDay);
                Moon.SetActive(IsDay);
                angle = -180;
            }

            Day.color += sign * new Color(0, 0, 0, 1) * Time.deltaTime / 2;
            Night.color -= sign * new Color(0, 0, 0, 1) * Time.deltaTime / 2;

            yield return null;
        }

        GlobalLight.color = targetColor;
        IsDay = !IsDay;
        Switching = false;
    }
}
