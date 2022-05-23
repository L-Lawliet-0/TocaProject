using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;

[ExecuteInEditMode]
public class SceneObjectsPlacer : MonoBehaviour
{
    public bool GENERATEOBJECTS;
    public bool CLEARALL;
    public bool INDEXFIX;
    public bool MOVEINDEX;
    public Transform Template;

    void Update()
    {

        if (GENERATEOBJECTS && transform.childCount == 0)
        {
            string path = Application.dataPath + "/Resources/Boy";

            DirectoryInfo dir = new DirectoryInfo(path);
            FileInfo[] info = dir.GetFiles("*.*");
            foreach (FileInfo f in info)
            {
                // generate objects here
                if (!f.Name.Contains("meta")) // ignore meta file
                {
                    // trim name
                    string name = f.Name.Split('.')[0];
                    GameObject obj = new GameObject();
                    obj.name = name;
                    obj.transform.SetParent(transform);
                    obj.AddComponent<SpriteRenderer>();
                    obj.GetComponent<SpriteRenderer>().sprite = Resources.Load<Sprite>("Boy/" + name);

                    PlaceObject(obj.transform);
                }

            }
            GENERATEOBJECTS = false;
        }
        else if (CLEARALL && transform.childCount > 0)
        {
            for (int i = transform.childCount - 1; i >= 0; i--)
                DestroyImmediate(transform.GetChild(i).gameObject);
            CLEARALL = false;
        }
        else if (INDEXFIX && transform.childCount > 0)
        {
            List<Transform> pendingFixes = new List<Transform>();
            for (int i = 0; i < transform.childCount; i++)
            {
                if (transform.GetChild(i).position.Equals(Vector3.zero))
                    pendingFixes.Add(transform.GetChild(i));
            }

            for (int i = 0; i < pendingFixes.Count; i++)
            {
                pendingFixes[i].SetSiblingIndex(i);
            }
            INDEXFIX = false;
        }
        else if (MOVEINDEX && transform.childCount > 0)
        {
            List<Transform> pendingFixes = new List<Transform>();
            for (int i = 0; i < transform.childCount; i++)
            {
                if (transform.GetChild(i).name.Contains("m"))
                    pendingFixes.Add(transform.GetChild(i));
            }

            for (int i = 0; i < pendingFixes.Count; i++)
            {
                pendingFixes[i].SetSiblingIndex(i);
            }
            MOVEINDEX = false;
        }
    }

    private void PlaceObject(Transform tran)
    {
        string convertedName = tran.name.Replace('-', ' ');
        for (int i = 0; i < Template.childCount; i++)
        {
            if (Template.GetChild(i).name.Equals(convertedName))
            {
                tran.position = Template.GetChild(i).position;
                tran.GetComponent<SpriteRenderer>().sortingOrder = Template.GetChild(i).GetComponent<SpriteRenderer>().sortingOrder;
                break;
            }
        }
    }
}
