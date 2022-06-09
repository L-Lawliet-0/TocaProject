using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterSelectionCtrl : MonoBehaviour
{
    private static CharacterSelectionCtrl m_Instance;
    public static CharacterSelectionCtrl Instance { get { return m_Instance; } }

    public GameObject[] AllCreationElements;

    public GameObject[] AllSelectionElements;

    private void Awake()
    {
        m_Instance = this;
    }

    private void Start()
    {
        Invoke("Init", .5f);
    }

    private void Init()
    {
        SetCreationMode(false);
    }

    public void SetCreationMode(bool active)
    {
        foreach (GameObject obj in AllCreationElements)
            obj.SetActive(active);
        foreach (GameObject obj in AllSelectionElements)
            obj.SetActive(!active);
    }

    public void OnNewCharacterCreate(int index)
    {
        SetCreationMode(true);
    }
}
