using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
public class Assigner : MonoBehaviour
{
    public enum Category
    {
        StaticBase
    }

    public Category m_Category;
    public bool ASSIGN;

    private void Update()
    {
        if (ASSIGN)
        {
            transform.parent = CategoryAssign.Instance.Categories[(int)m_Category];
            ASSIGN = false;
            DestroyImmediate(this);
        }
    }
}
