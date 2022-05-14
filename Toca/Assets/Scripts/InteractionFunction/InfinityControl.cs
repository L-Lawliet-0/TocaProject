using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InfinityControl : MonoBehaviour
{
    public GameObject[] ObjectsList;
    public Vector3[] Positions;
    public Quaternion[] Rotations;
    public GameObject Prefab;

    private void Start()
    {
        Positions = new Vector3[ObjectsList.Length];
        Rotations = new Quaternion[ObjectsList.Length];
        for (int i = 0; i < ObjectsList.Length; i++)
        {
            Positions[i] = ObjectsList[i].transform.position;
            Rotations[i] = ObjectsList[i].transform.rotation;
            ObjectsList[i] = null;
        }
    }

    void Update()
    {
        for (int i = 0; i < ObjectsList.Length; i++)
        {
            if (!ObjectsList[i] || ObjectsList[i].transform.parent != transform)
            {
                if (ObjectsList[i])
                    ObjectsList[i].GetComponent<SpriteRenderer>().enabled = true;
                GameObject clone = Instantiate(Prefab, Positions[i], Rotations[i], transform);
                ObjectsList[i] = clone;
            }
        }
    }
}
