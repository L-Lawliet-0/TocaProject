using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CharacterSelectionCtrl : MonoBehaviour
{
    private static CharacterSelectionCtrl m_Instance;
    public static CharacterSelectionCtrl Instance { get { return m_Instance; } }

    public GameObject[] AllCreationElements;

    public GameObject[] AllSelectionElements;

    // use this as data structure for data construction
    private CharacterData[] Datas;

    public Button ReturnButton;

    public ScrollRect m_ScrollRect;

    private void Awake()
    {
        m_Instance = this;
        Datas = new CharacterData[7];
    }

    private void Start()
    {
        Invoke("Init", .5f);
    }

    private bool SpeedLimitReached;
    private bool Lock = false;
    private void Update()
    {
        if (Lock)
            return;

        if (!SpeedLimitReached)
        {
            if (m_ScrollRect.Dragging)
                SpeedLimitReached = true;
            return;
        }

        if (!m_ScrollRect.Dragging)
        {
            float value = m_ScrollRect.horizontalNormalizedPosition;
            float dis = float.MaxValue;
            float target = -1;

            if (m_ScrollRect.velocity.x > 0)
            {
                for (int i = 6; i >= 0; i--)
                {
                    float temp = i * .16666f;
                    if (temp <= value)
                    {
                        target = temp;
                        break;
                    }
                }
                if (target == -1)
                    target = 0;
            }
            else
            {
                for (int i = 0; i < 7; i++)
                {
                    float temp = i * .16666f;
                    if (temp >= value)
                    {
                        target = temp;
                        break;
                    }
                }
                if (target == -1)
                    target = 1;
            }
            target = Mathf.Clamp01(target);
            SpeedLimitReached = false;

            m_ScrollRect.scrollSensitivity = 0;
            Lock = true;
            StartCoroutine("Reenable", target);
        }
    }

    private IEnumerator Reenable(float target)
    {
        float currentValue = m_ScrollRect.horizontalNormalizedPosition;
        float counter = Mathf.Abs(target - currentValue) * 3600 / m_ScrollRect.velocity.x;
        float maxC = counter;
        float mult = 1 / maxC;
        while (counter > 0)
        {

            m_ScrollRect.horizontalNormalizedPosition = Mathf.Lerp(currentValue, target, mult * (maxC - counter));
            counter -= Time.deltaTime;
            yield return null;
        }
        m_ScrollRect.horizontalNormalizedPosition = target;

        m_ScrollRect.scrollSensitivity = 1;
        Lock = false;
    }


    private void Init()
    {
        SetCreationMode(false);
    }

    public void ReturnHome()
    {
        gameObject.SetActive(false);
        Main.Instance.MainMenuButton();
    }

    public Transform[] Cells;
    public void SetCreationMode(bool active)
    {
        foreach (GameObject obj in AllCreationElements)
            obj.SetActive(active);
        foreach (GameObject obj in AllSelectionElements)
            obj.SetActive(!active);

        ReturnButton.onClick.RemoveAllListeners();
        // add listener
        if (active)
            ReturnButton.onClick.AddListener(() => SetCreationMode(false));
        else
            ReturnButton.onClick.AddListener(() => ReturnHome());

        if (!active && Datas[CurrentIndex] != null)
        {
            // read data from character creation
            CharacterCreation.Instance.GetPanelData(Datas[CurrentIndex]);
        }

        // update character preview element
        for (int i = 0; i < Datas.Length; i++)
        {
            Cells[i].GetChild(0).gameObject.SetActive(Datas[i] == null);
            Cells[i].GetChild(1).gameObject.SetActive(Datas[i] != null);
            if (Datas[i] != null)
                Cells[i].GetChild(1).GetComponent<CharacterPreview>().UpdateCharacter(Datas[i]);
        }
    }

    private int CurrentIndex;
    public void OnNewCharacterCreate(int index)
    {
        CurrentIndex = index;
        if (Datas[index] == null)
        {
            // there is not current data, create a new character
            Datas[index] = new CharacterData();
            Datas[index].RandomizeData();
        }
        else
        {
            
        }

        SetCreationMode(true);
        CharacterCreation.Instance.SetPanelData(Datas[index]);
    }
}
