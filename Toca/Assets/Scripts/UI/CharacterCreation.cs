using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterCreation : MonoBehaviour
{
    public GameObject UnitPrefab;
    public Transform ContentRoot;

    private void Start()
    {
        InitalizeContent(9);
    }

    public void InitalizeContent(int count)
    {
        float halfDelta = 289f / 2;
        int perRow = 3;
        int rowCount = count / 3;
        if (count % 3 > 0)
            rowCount++;
        RectTransform root = ContentRoot.GetComponent<RectTransform>();
        root.sizeDelta = new Vector2(1220, (rowCount - 1) * 323 + 289 + 100);

        float baseX = -1220 / 2 + halfDelta + 50;
        float baseY = root.sizeDelta.y / 2 - halfDelta - 50;

        Debug.LogError(root.sizeDelta);
        for (int i = 0; i < count; i++)
        {
            int y = i / 3;
            int x = i % 3;
            GameObject unit = Instantiate(UnitPrefab);
            RectTransform rect = unit.GetComponent<RectTransform>();
            rect.SetParent(ContentRoot);
            rect.localPosition = new Vector3(baseX + x * 395, baseY - y * 323);
            rect.localScale = Vector3.one;
        }
    }
}
