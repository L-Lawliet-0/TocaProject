using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CharacterCreation : MonoBehaviour
{
    private static CharacterCreation m_Instance;
    public static CharacterCreation Instance { get { return m_Instance; } }

    public GameObject UnitPrefab, fushiPrefab, lianPrefab, toufaPrefab;

    public GameObject ResetPrefab;

    public Transform glassesRoot, kouzhaoRoot, maoziRoot, mianjuRoot;
    public Transform yanjingRoot, biziRoot, zuiRoot;
    public Transform toufaRoot;

    public Transform Glasses, Kouzhao, Maozi, Mianju;
    private Transform[] AllPurples;
    private Dictionary<string, Transform> PurpleDict, YellowDict;

    public Sprite SelectedSprite, UnselectedSprite;
    public Sprite fushiSelectedSprite, fushiUnselectedSprite;
    public Sprite lianSelectedSprite, lianUnselectedSprite;

    public Transform[] AllYellows;
    public Transform[] AllOranges;

    public Transform fushiRoot;

    private const float MajorSpeed = 2000;

    

    // this section handles major panel swipe logic
    private class MajorPanel
    {
        public Transform ContentPanel;
        public Transform Label;
        public Vector3 OpenPos, ClosePos;
        public Vector3 l_OpenPos, l_ClosePos;
        public bool Open;
        public bool Moving; // this panel is either moving from left to right or right to left
        public MajorPanel(Transform cp, Transform label)
        {
            ContentPanel = cp;
            Label = label;
            OpenPos = cp.localPosition;
            ClosePos = cp.localPosition + Vector3.right * 1300;
            l_OpenPos = label.localPosition;
            l_ClosePos = label.localPosition + Vector3.right * 1300;
            Open = true;
        }

        public void DeltaChange()
        {
            int sign = Open ? 1 : -1;
            ContentPanel.localPosition += Vector3.right * sign * MajorSpeed * Time.deltaTime;
            Label.localPosition += Vector3.right * sign * MajorSpeed * Time.deltaTime;

            if ((Open && ContentPanel.localPosition.x > ClosePos.x) || (!Open && ContentPanel.localPosition.x < OpenPos.x))
            {
                Open = !Open;
                Moving = false;
                if (Open)
                {
                    ContentPanel.localPosition = OpenPos;
                    Label.localPosition = l_OpenPos;
                }
                else
                {
                    ContentPanel.localPosition = ClosePos;
                    Label.localPosition = l_ClosePos;
                }
            }
        }
    }
    private MajorPanel Hair, Face, Clothing, Gear;
    private MajorPanel[] AllPanels;

    private void InitalizeMajorPanel()
    {
        Hair = new MajorPanel(transform.GetChild(1), transform.GetChild(2));
        Face = new MajorPanel(transform.GetChild(3), transform.GetChild(4));
        Clothing = new MajorPanel(transform.GetChild(5), transform.GetChild(6));
        Gear = new MajorPanel(transform.GetChild(7), transform.GetChild(8));
        AllPanels = new MajorPanel[]
        {
            Hair, Face, Clothing, Gear
        };
    }

    public bool Shifting()
    {
        foreach (MajorPanel mp in AllPanels)
        {
            if (mp.Moving)
                return true;
        }
        return false;
    }

    private int ActiveIndex = -1;

    public void OpenMajorPanel(int index)
    {
        // don't do anything if not done moving yet
        if (!Shifting() && ActiveIndex != index)
        {
            ActiveIndex = index;
            // open left side
            StartCoroutine("Shift",index);
        }
    }

    private IEnumerator Shift(int index)
    {
        for (int i = 0; i <= index; i++)
        {
            if (!AllPanels[i].Open)
            {
                AllPanels[i].Moving = true;
                yield return new WaitForSeconds(.2f);
            }
        }

        // close right side
        for (int i = AllPanels.Length - 1; i > index; i--)
        {
            if (AllPanels[i].Open)
            {
                AllPanels[i].Moving = true;
                yield return new WaitForSeconds(.2f);
            }
        }
    }

    private void Update()
    {
        foreach (MajorPanel mp in AllPanels)
        {
            if (mp.Moving)
                mp.DeltaChange();
        }
    }

    private void Awake()
    {
        m_Instance = this;
    }

    public void InitAllPanels()
    {
        // initalize all contents
        AllPurples = new Transform[]
        {
            Glasses, Kouzhao, Maozi, Mianju
        };
        InitalizeContent(7, "glasses", glassesRoot, UnitPrefab, 395, 323, true);
        InitalizeContent(4, "kouzhao", kouzhaoRoot, UnitPrefab, 395, 323, true);
        InitalizeContent(16, "maozi", maoziRoot, UnitPrefab, 395, 323, true);
        InitalizeContent(9, "mianju", mianjuRoot, UnitPrefab, 395, 323, true);

        SetPurpleSubpage(0);

        PurpleDict = new Dictionary<string, Transform>();
        PurpleDict.Add("glasses", glassesRoot);
        PurpleDict.Add("kouzhao", kouzhaoRoot);
        PurpleDict.Add("maozi", maoziRoot);
        PurpleDict.Add("mianju", mianjuRoot);

        YellowDict = new Dictionary<string, Transform>();
        YellowDict.Add("yanjing", yanjingRoot);
        YellowDict.Add("bizi", biziRoot);
        YellowDict.Add("zui", zuiRoot);

        InitalizeContent(41, "fushi", fushiRoot, fushiPrefab, 395, 436, false);

        InitalizeContent(36, "yanjing", yanjingRoot, lianPrefab, 395, 323, false);
        InitalizeContent(32, "bizi", biziRoot, lianPrefab, 395, 323, false);
        InitalizeContent(29, "zui", zuiRoot, lianPrefab, 395, 323, false);

        SetYellowSubpage(0);

        InitalizeContent(41, "toufa", toufaRoot, toufaPrefab, 395, 323, false);

        SetOrangeSubpage(0);

        // initalize major panel
        InitalizeMajorPanel();
    }

    public void InitalizeContent(int count, string itemPrefix, Transform rootTran, GameObject unitPrefab, float horizontalGap, float verticalGap, bool resetButton = false)
    {
        if (resetButton)
            count += 1;
        float halfDelta = unitPrefab.GetComponent<RectTransform>().sizeDelta.x / 2; //289f / 2;
        int perRow = 3;
        int rowCount = count / perRow;
        if (count % perRow > 0)
            rowCount++;
        RectTransform root = rootTran.GetComponent<RectTransform>();
        rowCount = Mathf.Max(rowCount, 3);
        root.sizeDelta = new Vector2(1220, (rowCount - 1) * verticalGap + halfDelta * 2 + 100);

        root.localPosition = new Vector3(root.localPosition.x, -root.sizeDelta.y / 2);

        float baseX = -1220 / 2 + halfDelta + 50;
        float baseY = root.sizeDelta.y / 2 - halfDelta - 50;

        Debug.LogError(root.sizeDelta);
        for (int i = 0; i < count; i++)
        {
            int y = i / 3;
            int x = i % 3;

            GameObject unit;
            if (i == 0 && resetButton)
                unit = Instantiate(ResetPrefab);
            else
                unit = Instantiate(unitPrefab);
            RectTransform rect = unit.GetComponent<RectTransform>();
            rect.SetParent(rootTran);
            rect.localPosition = new Vector3(baseX + x * horizontalGap, baseY - y * verticalGap);
            rect.localScale = Vector3.one;

            // register button
            int input = i;
            if (!resetButton)
                input += 1;
            else if (i == 0)
                input = -1;

            if (itemPrefix.Equals("fushi"))
                rect.GetComponent<Button>().onClick.AddListener(() => fushiButtonCallback(input));
            else if (itemPrefix.Equals("yanjing") || itemPrefix.Equals("bizi") || itemPrefix.Equals("zui"))
                rect.GetComponent<Button>().onClick.AddListener(() => lianButtonCallback(itemPrefix, input));
            else if (itemPrefix.Equals("toufa"))
                rect.GetComponent<Button>().onClick.AddListener(() => toufaButtonCallback(itemPrefix, input));
            else
                rect.GetComponent<Button>().onClick.AddListener(() => ButtonCallback(itemPrefix, input));

            if (!resetButton || i > 0)
            {
                if (itemPrefix.Equals("toufa"))
                {
                    SetHairUIunit(unit, input);
                }
                else
                {
                    Image img = rect.GetChild(0).GetComponent<Image>();
                    Sprite sprite = Resources.Load<Sprite>(itemPrefix + "/" + itemPrefix + input.ToString());
                    img.sprite = sprite;

                    rect = rect.GetChild(0).GetComponent<RectTransform>();
                    rect.sizeDelta = sprite.bounds.size * 100;
                }
            }
        }
    }

    private void SetHairUIunit(GameObject obj, int index)
    {
        if (houmianHairs.Contains(index))
        {
            obj.transform.GetChild(0).GetComponent<Image>().sprite = Resources.Load<Sprite>("toufa/toufahoumian" + index);
            obj.transform.GetChild(0).GetComponent<Image>().enabled = true;
        }
        else
        {
            obj.transform.GetChild(0).GetComponent<Image>().enabled = false;
        }

        obj.transform.GetChild(1).GetComponent<Image>().sprite = Resources.Load<Sprite>("toufa/" + SpineUI.Instance.GetHead());

        obj.transform.GetChild(2).GetComponent<Image>().sprite = Resources.Load<Sprite>("toufa/toufa" + index);

        if (peishiHairs.Contains(index))
        {
            obj.transform.GetChild(3).GetComponent<Image>().sprite = Resources.Load<Sprite>("toufa/peishi" + index);
            obj.transform.GetChild(3).GetComponent<Image>().enabled = true;
        }
        else
            obj.transform.GetChild(3).GetComponent<Image>().enabled = false;

        obj.transform.GetChild(0).GetComponent<Image>().color = obj.transform.GetChild(2).GetComponent<Image>().color = SpineUI.Instance.GetHairColor();
    }

    private void fushiButtonCallback(int index)
    {
        SpineUI.Instance.ChangeSkin(index);
        for (int i = 0; i < fushiRoot.childCount; i++)
        {
            fushiRoot.GetChild(i).GetComponent<Image>().sprite = index == i + 1 ? fushiSelectedSprite : fushiUnselectedSprite;
        }
    }

    private void ButtonCallback(string key, int index)
    {
        SpineUI.Instance.Pairs[key].SetAttachment(index);
        Transform tran = PurpleDict[key];
        for (int i = 1; i < tran.childCount; i++)
        {
            tran.GetChild(i).GetComponent<Image>().sprite = index == i ? SelectedSprite : UnselectedSprite;
        }
    }

    private static List<int> houmianHairs = new List<int>()
    {
        19, 20, 21, 27, 35, 36, 40
    };

    private static List<int> peishiHairs = new List<int>()
    {
        8, 9, 24
    };

    private void toufaButtonCallback(string key, int index)
    {
        SpineUI.Instance.Pairs[key].SetAttachment(index);
        if (houmianHairs.Contains(index))
            SpineUI.Instance.toufahoumian.SetAttachment(index);
        else
            SpineUI.Instance.toufahoumian.SetAttachment();
    }

    private void lianButtonCallback(string key, int index)
    {
        SpineUI.Instance.Pairs[key].SetAttachment(index);
        Transform tran = YellowDict[key];
        for (int i = 0; i < tran.childCount; i++)
        {
            int indexFix = index - 1;
            tran.GetChild(i).GetComponent<Image>().sprite = indexFix == i ? lianSelectedSprite : lianUnselectedSprite;
        }
    }

    private void SetChildrenActive(Transform tran, bool active)
    {
        for (int i = 0; i < tran.childCount; i++)
        {
            tran.GetChild(i).gameObject.SetActive(active);
        }
    }

    public void SetPurpleSubpage(int index)
    {
        for (int i = 0; i < AllPurples.Length; i++)
        {
            SetChildrenActive(AllPurples[i], i == index);
        }
    }

    public void SetYellowSubpage(int index)
    {
        for (int i = 0; i < AllYellows.Length; i++)
        {
            SetChildrenActive(AllYellows[i], i == index);
        }
    }

    public void SetOrangeSubpage(int index)
    {
        for (int i = 0; i < AllOranges.Length; i++)
        {
            SetChildrenActive(AllOranges[i], i == index);
        }
    }

    public static Color[] SkinColors = new Color[]
    {
        new Color(235f / 255f, 208f / 255f, 191f / 255f),
        new Color(255f / 255f, 214f / 255f, 186f / 255f),
        new Color(228f / 255f, 182f / 255f, 160f / 255f),
        new Color(248f / 255f, 194f / 255f, 109f / 255f),
        new Color(197f / 255f, 120f / 255f, 48f / 255f),
        new Color(218f / 255f, 144f / 255f, 74f / 255f),
        new Color(255f / 255f, 182f / 255f, 130f / 255f),
        new Color(255f / 255f, 164f / 255f, 99f / 255f),
        new Color(255f / 255f, 187f / 255f, 99f / 255f),
        new Color(195f / 255f, 135f / 255f, 96f / 255f),
        new Color(177f / 255f, 95f / 255f, 57f / 255f),
        new Color(150f / 255f, 75f / 255f, 40f / 255f),
        new Color(119f / 255f, 65f / 255f, 40f / 255f),
        new Color(173f / 255f, 106f / 255f, 87f / 255f),
        new Color(99f / 255f, 56f / 255f, 24f / 255f),
        new Color(235f / 255f, 208f / 255f, 191f / 255f),
        new Color(210f / 255f, 147f / 255f, 112f / 255f),
        new Color(205f / 255f, 164f / 255f, 137f / 255f),
        new Color(244f / 255f, 175f / 255f, 126f / 255f),
        new Color(218f / 255f, 149f / 255f, 85f / 255f),
        new Color(215f / 255f, 191f / 255f, 176f / 255f),
        new Color(142f / 255f, 75f / 255f, 37f / 255f),
    };

    public static Color[] HairColors = new Color[]
    {
         new Color(241f / 255f, 145f / 255f, 73f / 255f),
         new Color(72f / 255f, 58f / 255f, 57f / 255f),
         new Color(230f / 255f, 206f / 255f, 196f / 255f),
         new Color(233f / 255f, 192f / 255f, 173f / 255f),
         new Color(74f / 255f, 39f / 255f, 31f / 255f),
         new Color(125f / 255f, 105f / 255f, 101f / 255f),
         new Color(36f / 255f, 36f / 255f, 36f / 255f),
         new Color(217f / 255f, 217f / 255f, 217f / 255f),
         new Color(41f / 255f, 17f / 255f, 15f / 255f),
         new Color(31f / 255f, 37f / 255f, 63f / 255f),
         new Color(240f / 255f, 185f / 255f, 120f / 255f),
         new Color(210f / 255f, 194f / 255f, 181f / 255f),
         new Color(119f / 255f, 74f / 255f, 63f / 255f),
         new Color(51f / 255f, 63f / 255f, 67f / 255f),
         new Color(51f / 255f, 54f / 255f, 63f / 255f),
         new Color(58f / 255f, 40f / 255f, 40f / 255f),
         new Color(226f / 255f, 190f / 255f, 152f / 255f),
         new Color(231f / 255f, 196f / 255f, 96f / 255f),
         new Color(0f / 255f, 134f / 255f, 171f / 255f),
         new Color(207f / 255f, 183f / 255f, 219f / 255f),
         new Color(150f / 255f, 54f / 255f, 82f / 255f),
         new Color(255f / 255f, 191f / 255f, 207f / 255f),
         new Color(43f / 255f, 80f / 255f, 88f / 255f),
         new Color(88f / 255f, 50f / 255f, 91f / 255f),
         new Color(222f / 255f, 153f / 255f, 88f / 255f),
    };

    public void SetSkinColor(int colorIndex)
    {
        SpineUI.Instance.SetSkinColor(SkinColors[colorIndex]);
    }

    public void SetHairColor(int colorIndex)
    {
        SpineUI.Instance.SetHairColor(HairColors[colorIndex]);
        ResetHairElements();
    }

    public void ResetHairElements()
    {
        for (int i = 0; i < toufaRoot.childCount; i++)
        {
            SetHairUIunit(toufaRoot.GetChild(i).gameObject, i + 1);
        }
    }
}
