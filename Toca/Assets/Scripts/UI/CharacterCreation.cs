using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CharacterCreation : MonoBehaviour
{
    public GameObject UnitPrefab, fushiPrefab, lianPrefab;

    public GameObject ResetPrefab;

    public Transform glassesRoot, kouzhaoRoot, maoziRoot, mianjuRoot;
    public Transform yanjingRoot, biziRoot, zuiRoot;

    public Transform Glasses, Kouzhao, Maozi, Mianju;
    private Transform[] AllPurples;
    private Dictionary<string, Transform> PurpleDict;

    public Sprite SelectedSprite, UnselectedSprite;
    public Sprite fushiSelectedSprite, fushiUnselectedSprite;
    public Sprite lianSelectedSprite, lianUnselectedSprite;

    public Transform[] AllYellows;

    public Transform fushiRoot;

    private void Start()
    {
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


        InitalizeContent(41, "fushi", fushiRoot, fushiPrefab, 395, 436, false);

        InitalizeContent(40, "yanjing", yanjingRoot, UnitPrefab, 395, 323, false);
        InitalizeContent(40, "bizi", biziRoot, UnitPrefab, 395, 323, false);
        InitalizeContent(41, "zui", zuiRoot, UnitPrefab, 395, 323, false);
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
            if (itemPrefix.Equals("bizi") && i >= 26)
                input += 2;
            else if (!resetButton)
                input += 1;
            else if (i == 0)
                input = -1;

            if (itemPrefix.Equals("fushi"))
                rect.GetComponent<Button>().onClick.AddListener(() => fushiButtonCallback(input));
            else
                rect.GetComponent<Button>().onClick.AddListener(() => ButtonCallback(itemPrefix, input));

            if (!resetButton || i > 0)
            {
                Image img = rect.GetChild(0).GetComponent<Image>();
                Sprite sprite = Resources.Load<Sprite>(itemPrefix  + "/" + itemPrefix + input.ToString());
                img.sprite = sprite;

                rect = rect.GetChild(0).GetComponent<RectTransform>();
                rect.sizeDelta = sprite.bounds.size * 100;
            }
        }
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

    private static Color[] SkinColors = new Color[]
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

    public void SetSkinColor(int colorIndex)
    {
        SpineUI.Instance.SetSkinColor(SkinColors[colorIndex]);
    }
}
