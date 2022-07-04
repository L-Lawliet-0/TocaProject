using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpreaderControl : TocaFunction
{
    private SelectionControl m_Scelection;
    public Sprite[] Fogs;

    private List<FogStructure> m_Fogs;

    public bool Right;

    private class FogStructure
    {
        public GameObject Fog;
        public float Counter;
        public float Height;
    }

    private void Awake()
    {
        m_Fogs = new List<FogStructure>();
    }
    void Start()
    {
        m_Scelection = (SelectionControl)TocaObject.GetTocaFunction<SelectionControl>();
        StopAllCoroutines();
        StartCoroutine("Runner");
    }

    private void OnEnable()
    {
        if (m_Scelection)
            StartCoroutine("Runner");
    }

    private void OnDisable()
    {
        for (int i = m_Fogs.Count - 1; i >= 0; i--)
        {
            Destroy(m_Fogs[i].Fog);
            m_Fogs.RemoveAt(i);
        }
    }

    private void Update()
    {
        for (int i = m_Fogs.Count - 1; i >= 0; i--)
        {
            m_Fogs[i].Fog.transform.position += -Vector3.right * Time.deltaTime * 3 * (Right ? -1 : 1);
            m_Fogs[i].Fog.transform.position += Vector3.up * Time.deltaTime * m_Fogs[i].Height;
            m_Fogs[i].Height += Time.deltaTime * .1f;
            m_Fogs[i].Counter -= Time.deltaTime;

            if (m_Fogs[i].Counter <= 0)
            {
                Destroy(m_Fogs[i].Fog);
                m_Fogs.RemoveAt(i);
            }
        }
    }

    private IEnumerator Runner()
    {
        while (true)
        {
            if (m_Scelection.Selected)
            {
                GameObject obj = new GameObject();
                obj.AddComponent<SpriteRenderer>();
                obj.GetComponent<SpriteRenderer>().sprite = Fogs[Random.Range(0, Fogs.Length)];
                obj.GetComponent<SpriteRenderer>().sortingLayerName = "Selection";

                obj.transform.position = transform.position;
                FogStructure fs = new FogStructure();
                fs.Fog = obj;
                fs.Counter = 2;

                m_Fogs.Add(fs);

            }
            yield return new WaitForSeconds(Random.Range(.05f, .15f));
        }
    }
}
