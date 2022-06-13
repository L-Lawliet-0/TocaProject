using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

public class SaveManager : MonoBehaviour
{
    private static SaveManager m_Instance;
    public static SaveManager Instance { get { return m_Instance; } }

    private void Awake()
    {
        m_Instance = this;
    }

    public static List<TocaObject.ObjectSaveData> LoadFromFile(string dataPath)
    {
        using (Stream file = File.Open(dataPath, FileMode.Open))
        {
            BinaryFormatter bf = new BinaryFormatter();
            return bf.Deserialize(file) as List<TocaObject.ObjectSaveData>;
        }
    }

    public static void SaveCurrentScene(string dataPath)
    {
        List<TocaObject.ObjectSaveData> datas = new List<TocaObject.ObjectSaveData>();
        TocaObject[] tocas = FindObjectsOfType<TocaObject>(true);
        foreach (TocaObject toca in tocas)
        {
            if ((!toca.transform.parent || !toca.transform.parent.name.Equals("Saveable")) && toca.GetTocaFunction<FindControl>() && toca.GetTocaFunction<MoveControl>() && !toca.GetComponentInParent<TrackControl>() && toca.TocaSave.ObjectID != 0 && toca.TocaSave.PrefabID > 0)
            {
                TocaObject.ObjectSaveData data = toca.TocaSave;
                data.x = toca.transform.position.x;
                data.y = toca.transform.position.y;
                FindControl fc = (FindControl)toca.GetTocaFunction<FindControl>();
                if (fc.CurrentAttachment)
                {
                    data.Attaching = true;
                    data.ParentObjectID = fc.CurrentAttachment.TocaObject.TocaSave.ObjectID;
                    data.ParentBaseID = fc.CurrentAttachment.BaseID;
                }
                else
                    data.Attaching = false;

                datas.Add(data);
            }
        }

        using (Stream file = File.Open(dataPath, FileMode.OpenOrCreate))
        {
            BinaryFormatter bf = new BinaryFormatter();
            bf.Serialize(file, datas);
        }
    }
}
