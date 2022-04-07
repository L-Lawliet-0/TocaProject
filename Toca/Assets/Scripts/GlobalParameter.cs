using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GlobalParameter : MonoBehaviour
{
    private static GlobalParameter m_Instance;
    public static GlobalParameter Instance { get { return m_Instance; } }
    public static int Depth = 50;
    public static float ReachTime = .05f;

    public static float MaxLimbSpeed = 50, MinLimbSpeed = 1;
    public static float Acceleration = 100, StartSpeed = 20;

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

    public static T[] GetComponentAndChildren<T>(Transform root)
    {
        T[] functions1 = root.GetComponents<T>();
        T[] functions2 = root.GetComponentsInChildren<T>(true);

        T[] AllFunctions = new T[functions1.Length + functions2.Length];
        int i = 0;
        for (; i < functions1.Length; i++)
            AllFunctions[i] = functions1[i];
        for (; i < AllFunctions.Length; i++)
            AllFunctions[i] = functions2[i - functions1.Length];
        return AllFunctions;
    }

    public static float ClampAngle(float angle)
    {
        while (angle < 0)
            angle += 360;
        while (angle > 360)
            angle -= 360;
        return angle;
    }
}
