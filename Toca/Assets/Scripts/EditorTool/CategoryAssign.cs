using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
public class CategoryAssign : MonoBehaviour
{
    // automatically asign gameobject to corresponding parent object
    private static CategoryAssign m_Instance;
    public static CategoryAssign Instance { get { return m_Instance; } }
    public Transform[] Categories;
    private void Awake()
    {
        m_Instance = this;
    }

    private void Update()
    {
        m_Instance = this;
    }
}
