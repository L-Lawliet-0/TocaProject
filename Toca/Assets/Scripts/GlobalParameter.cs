using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GlobalParameter : MonoBehaviour
{
    private static GlobalParameter m_Instance;
    public static GlobalParameter Instance { get { return m_Instance; } }
    public static int Depth = 50;
    public static float ReachTime = .15f;

    public static float MaxLimbSpeed = 150, MinLimbSpeed = 10;
    public static float Gravity = 100;

    private void Awake()
    {
        m_Instance = this;
    }

    public Vector3 ScreenPosToGamePos(Vector3 inputPosition)
    {
        Vector3 pos = inputPosition;
        pos.z = GlobalParameter.Depth;
        pos = Camera.main.ScreenToWorldPoint(pos);
        pos.z = GlobalParameter.Depth;

        return pos;
    }
}
