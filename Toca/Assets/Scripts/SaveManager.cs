using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

public class SaveManager : MonoBehaviour
{
    private static SaveManager m_Instance;
    public static SaveManager Instance { get { return m_Instance; } }
    public const int CharacterID = -50;

    private void Awake()
    {
        m_Instance = this;

        string[] fileNames = new string[]
        {
            "/gongzhufang",
            "/haijunfeng",
            "/nanhaifang",
            "/CharacterTrack",
            "/CharacterCreation",
            "/TrackProps"
        };

        // test
        // overwrite data
        foreach (string str in fileNames)
        {

            if (!File.Exists(Application.persistentDataPath + str))
            {
                
                WWW loadWWW = new WWW(Application.streamingAssetsPath + str);
                while (!loadWWW.isDone)
                {
                    // wait this shit
                }
                File.WriteAllBytes(Application.persistentDataPath + str, loadWWW.bytes);
                
                //File.Copy(Application.streamingAssetsPath + str, Application.persistentDataPath + str, true);
            }
            //File.Copy(Application.streamingAssetsPath + str, Application.persistentDataPath + str, true);
        }
    }

    public static List<TocaObject.ObjectSaveData> LoadFromFile(string dataPath)
    {
        using (Stream file = File.Open(dataPath, FileMode.Open))
        {
            BinaryFormatter bf = new BinaryFormatter();
            return bf.Deserialize(file) as List<TocaObject.ObjectSaveData>;
        }
    }

    public static void SaveToFile(string dataPath, List<TocaObject.ObjectSaveData> data)
    {
        using (Stream file = File.Open(dataPath, FileMode.Open))
        {
            BinaryFormatter bf = new BinaryFormatter();
            bf.Serialize(file, data);
        }
    }

    public static void SaveTrackProps(List<TocaObject.ObjectSaveData> datas)
    {
        using (Stream file = File.Open(Application.persistentDataPath + "/TrackProps", FileMode.Open))
        {
            BinaryFormatter bf = new BinaryFormatter();
            bf.Serialize(file, datas);
        }
    }

    public static List<List<TocaObject.ObjectSaveData>> LoadTrackData()
    {
        using (Stream file = File.Open(Application.persistentDataPath + "/CharacterTrack", FileMode.Open))
        {
            BinaryFormatter bf = new BinaryFormatter();
            return bf.Deserialize(file) as List<List<TocaObject.ObjectSaveData>>;
        }
    }

    public static CharacterData[] LoadCreationData()
    {
        using (Stream file = File.Open(Application.persistentDataPath + "/CharacterCreation", FileMode.Open))
        {
            BinaryFormatter bf = new BinaryFormatter();
            return bf.Deserialize(file) as CharacterData[];
        }
    }

    public static void SaveCreationData(CharacterData[] datas)
    {
        using (Stream file = File.Open(Application.persistentDataPath + "/CharacterCreation", FileMode.Open))
        {
            BinaryFormatter bf = new BinaryFormatter();
            bf.Serialize(file, datas);
        }
    }

    public static void SaveTrackData(List<List<TocaObject.ObjectSaveData>> datas)
    {
        using (Stream file = File.Open(Application.persistentDataPath + "/CharacterTrack", FileMode.Open))
        {
            BinaryFormatter bf = new BinaryFormatter();
            bf.Serialize(file, datas);
        }
    }

    public static void SaveCurrentScene(string dataPath)
    {
        List<TocaObject.ObjectSaveData> datas = new List<TocaObject.ObjectSaveData>();
        TocaObject[] tocas = FindObjectsOfType<TocaObject>(true);
        foreach (TocaObject toca in tocas)
        {
            if ((!toca.transform.parent || !toca.transform.parent.name.Equals("Saveable")) && toca.GetTocaFunction<FindControl>() && toca.GetTocaFunction<MoveControl>() && !toca.GetComponentInParent<TrackControl>() && toca.TocaSave.ObjectID != 0 && (toca.TocaSave.PrefabID > 0 || toca.TocaSave.PrefabID == CharacterID))
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

    public void TryUpdateSceneData(CharacterData data)
    {
        string[] dataPaths = new string[]
        {
            Application.persistentDataPath + "/gongzhufang",
            Application.persistentDataPath + "/haijunfeng",
            Application.persistentDataPath + "/nanhaifang"
        };

        foreach (string str in dataPaths)
        {
            List<TocaObject.ObjectSaveData> datas = LoadFromFile(str);
            
            foreach (TocaObject.ObjectSaveData save in datas)
            {
                if (save.My_CharacterData != null && save.My_CharacterData.UNIQUE_ID == data.UNIQUE_ID)
                {
                    save.My_CharacterData = data;
                }
            }

            using (Stream file = File.Open(str, FileMode.Open))
            {
                BinaryFormatter bf = new BinaryFormatter();
                bf.Serialize(file, datas);
            }
        }
    }
}
