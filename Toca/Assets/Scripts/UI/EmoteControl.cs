using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EmoteControl : MonoBehaviour
{
    private static EmoteControl m_Instance;
    public static EmoteControl Instance { get { return m_Instance; } }
    public bool Active;
    public bool Showing;
    public SpineControl CurrentSpine;
    private void Awake()
    {
        m_Instance = this;
    }

    private void Update()
    {
        if (Active && CurrentSpine)
        {
            transform.position = GlobalParameter.Instance.GamePosToScreenPos(CurrentSpine.transform.position + Vector3.up * 2);
        }
    }

    public void SetActive(bool active, Vector3 headPos, SpineControl spine)
    {
        Debug.LogError(active);
        if (Active != active && !Showing)
        {
            CurrentSpine = spine;
            transform.position = headPos;
            Showing = true;
            Active = active;
            StartCoroutine("SetActiveHelper");
        }
    }

    private IEnumerator SetActiveHelper()
    {
        for (int i = 0; i < transform.childCount; i++)
        {
            transform.GetChild(i).gameObject.SetActive(Active);
            yield return new WaitForSeconds(.1f);
        }
        Showing = false;
    }

    public void SetEmote(int index)
    {
        if (Active && CurrentSpine)
        {
            switch (index)
            {
                case 0:
                    CurrentSpine.SetBiaoqing("xiao");
                    break;
                case 1:
                    CurrentSpine.SetBiaoqing("xingxingyan");
                    break;
                case 2:
                    CurrentSpine.SetBiaoqing("chan");
                    break;
                case 3:
                    CurrentSpine.SetBiaoqing("jingkong");
                    break;
                case 4:
                    CurrentSpine.SetBiaoqing("ku");
                    break;
                case 5:
                    CurrentSpine.SetBiaoqing("shengqi");
                    break;
                case 6:
                    CurrentSpine.SetBiaoqing("nanshou");
                    break;
            }
        }
    }

    public void EmoteCheck(Vector3 inputPos)
    {
        Debug.LogError(Vector2.Distance(inputPos, transform.position));
        if (Vector2.Distance(inputPos, transform.position) > 500)
            SetActive(false, transform.position, null);
    }
}
