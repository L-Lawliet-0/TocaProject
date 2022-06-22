using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// generate steam when there's attachment
/// </summary>
public class StewControl : TocaFunction
{
    private class SteamData
    {
        public float Angle;
        public float BaseX;
        public float Timer;
        public float frequency;
        public GameObject SteamFX;
    }
    private BaseControl BaseControl;
    private bool GeneratingSteam;
    private List<SteamData> SteamDatas;

    public float SteamGap = 1;
    private float Counter;
    private GameObject SFX;
    /// <summary>
    /// the dictionary contains the movecontrol and float, which indicated the angle used in sine wave function
    /// </summary>
    private void Start()
    {
        BaseControl = (BaseControl)TocaObject.GetTocaFunction<BaseControl>();
        GeneratingSteam = false;
        SteamDatas = new List<SteamData>();
    }

    private void Update()
    {
        if (!GeneratingSteam)
        {
            if (BaseControl.Attachments.Count > 0)
            {
                GeneratingSteam = true;
                SFX = SoundManager.Instance.PlaySFX(31, false, TocaObject.transform.position);
            }
        }
        else
        {
            Counter += Time.deltaTime;
            if (Counter > SteamGap)
            {
                Counter = 0;
                SteamGap = Random.Range(.5f, 1f);
                // generate a new steam effect
                SteamData steam = new SteamData();
                steam.SteamFX = Instantiate(GlobalParameter.Instance.RunTimeEffects[3], transform.position, Quaternion.identity);
                steam.BaseX = steam.SteamFX.transform.position.x;
                steam.Timer = 0;
                steam.frequency = Random.Range(.5f, 1.5f);
                SteamDatas.Add(steam);
            }

            if (BaseControl.Attachments.Count <= 0)
            {
                GeneratingSteam = false;
                Destroy(SFX);
            }
        }

        for (int i = SteamDatas.Count - 1; i >= 0; i--)
        {
            SteamData s = SteamDatas[i];
            s.Timer += Time.deltaTime;
            s.Angle += Time.deltaTime * 360;

            float x = s.BaseX + Mathf.Sin(s.Angle * Mathf.Deg2Rad) * s.frequency;
            float y = s.SteamFX.transform.position.y + Time.deltaTime * 5;
            float z = s.SteamFX.transform.position.z;
            s.SteamFX.transform.position = new Vector3(x, y, z);

            if (s.Timer > 2)
            {
                SteamDatas.RemoveAt(i);
                Destroy(s.SteamFX.gameObject);
            }
        }
    }
}
