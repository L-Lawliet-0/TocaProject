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

    public const int MaxCharacters = 20;

    public ScrollRect m_ScrollRect;

    private void Awake()
    {
        m_Instance = this;
    }

    private void Start()
    {
        Datas = SaveManager.LoadCreationData();
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

        float unitLength = 1f / (MaxCharacters - 1);

        if (!m_ScrollRect.Dragging)
        {
            float value = m_ScrollRect.horizontalNormalizedPosition;
            float target = -1;
            int activeIndex = 0;

            if (m_ScrollRect.velocity.x > 0)
            {
                for (int i = MaxCharacters; i >= 0; i--)
                {
                    float temp = i * unitLength;
                    if (temp <= value)
                    {
                        target = temp;
                        activeIndex = i;
                        break;
                    }
                }
                if (target == -1)
                {
                    target = 0;
                    activeIndex = 0;
                }
            }
            else
            {
                for (int i = 0; i < MaxCharacters; i++)
                {
                    float temp = i * unitLength;
                    if (temp >= value)
                    {
                        target = temp;
                        activeIndex = i;
                        break;
                    }
                }
                if (target == -1)
                {
                    target = 1;
                    activeIndex = MaxCharacters - 1;
                }
            }
            target = Mathf.Clamp01(target);

            for (int i = 0; i < Cells.Length; i++)
            {
                Cells[i].GetChild(0).GetComponent<Button>().interactable = i == activeIndex;
                Cells[i].GetChild(1).GetComponent<Button>().interactable = i == activeIndex;
            }

            SpeedLimitReached = false;

            m_ScrollRect.scrollSensitivity = 0;
            Lock = true;
            StartCoroutine("Reenable", target);
        }
    }

    private IEnumerator Reenable(float target)
    {
        float currentValue = m_ScrollRect.horizontalNormalizedPosition;
        float counter = Mathf.Abs(target - currentValue) * 11400 / Mathf.Abs(m_ScrollRect.velocity.x);
        counter = Mathf.Min(counter, 2); // max 1 second of move time
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
        CurrentIndex = -1;
        SetCreationMode(false);
        for (int i = 0; i < Cells.Length; i++)
        {
            Cells[i].GetChild(0).GetComponent<Button>().interactable = i == 0;
            Cells[i].GetChild(1).GetComponent<Button>().interactable = i == 0;
        }
    }

    public int PreScene;
    public void ReturnHome()
    {
        gameObject.SetActive(false);
        if (PreScene == -1)
            Main.Instance.MainMenuButton();
        else
            LoadingCtrl.Instance.LoadSceneFromScene(PreScene);
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
        {
            ReturnButton.onClick.AddListener(() => ReturnHome());
        }

        if (!active && CurrentIndex != -1 && Datas[CurrentIndex] != null)
        {
            // read data from character creation
            CharacterCreation.Instance.GetPanelData(Datas[CurrentIndex]);

            // only need to update character just edited
            SaveManager.Instance.TryUpdateSceneData(Datas[CurrentIndex]);
            CharacterTrack.Instance.TryUpdateTrack(Datas[CurrentIndex]);
        }

        SaveManager.SaveCreationData(Datas);

        // update character preview element
        for (int i = 0; i < Datas.Length; i++)
        {
            Cells[i].GetChild(0).gameObject.SetActive(Datas[i] == null);
            Cells[i].GetChild(1).gameObject.SetActive(Datas[i] != null);
            if (Datas[i] != null)
            {
                Cells[i].GetChild(1).GetComponent<CharacterPreview>().UpdateCharacter(Datas[i]);
            }
        }

        if (CurrentIndex == -1)
            CurrentIndex = 0;
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
            Datas[index].UNIQUE_ID = Datas[index].GetHashCode();

            // when creating a new data, add the corresponding data to track data
            CharacterTrack.Instance.AddData(Datas[index]);
        }
        else
        {
            
        }

        SetCreationMode(true);
        CharacterCreation.Instance.SetPanelData(Datas[index]);
    }

    public static HashSet<int> GetIDs()
    {
        CharacterData[] datas = SaveManager.LoadCreationData();
        HashSet<int> returnValues = new HashSet<int>();
        foreach (CharacterData cd in datas)
        {
            if (cd != null) // null check
                returnValues.Add(cd.UNIQUE_ID);
        }
        return returnValues;
    }
}
