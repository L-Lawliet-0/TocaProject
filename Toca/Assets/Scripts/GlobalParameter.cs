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

    public GameObject[] RunTimeEffects;


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

    public static void SetGlobalScale(Transform tran, Vector3 globalScale)
    {
        tran.localScale = Vector3.one;
        tran.localScale = new Vector3(globalScale.x / tran.lossyScale.x, globalScale.y / tran.lossyScale.y, globalScale.z / tran.lossyScale.z);
    }

    public static bool HaveComponentInHierchy<T>(TocaObject t)
    {
        if (t.GetTocaFunction<T>())
            return true;

        FindControl f = (FindControl)t.GetTocaFunction<FindControl>();
        if (f && f.CurrentAttachment)
            return HaveComponentInHierchy<T>(f.CurrentAttachment.TocaObject);

        return false;
    }

    public static bool OverrideMove(BaseControl t)
    {
        return UpdateMovement(t) || t.MyBaseAttributes.IsLeftHand || t.MyBaseAttributes.IsRightHand;
        //return t.TocaObject.GetTocaFunction<SlideControl>() || t.TocaObject.GetTocaFunction<TrashBinControl>() || t.TocaObject.GetTocaFunction<FloatControl>() || t.TocaObject.GetTocaFunction<HorseShakeControl>(); //|| t.MyBaseAttributes.IsLeftHand || t.MyBaseAttributes.IsRightHand;
    }

    public static bool UpdateMovement(BaseControl t)
    {
        return t.TocaObject.GetTocaFunction<SlideControl>() || t.TocaObject.GetTocaFunction<TrashBinControl>() || t.TocaObject.GetTocaFunction<FloatControl>() || t.TocaObject.GetTocaFunction<BedControl>(); //HaveComponentInHierchy<SlideControl>(t.TocaObject) || HaveComponentInHierchy<TrashBinControl>(t.TocaObject) || HaveComponentInHierchy<FloatControl>(t.TocaObject);
    }

    public GameObject ToastSausage, ToastCarrot, ChickenToast, ChickenBread;
    public GameObject GetCombinePrefab(CombineControl.CombineType t1, CombineControl.CombineType t2)
    {
        HashSet<CombineControl.CombineType> types = new HashSet<CombineControl.CombineType>();
        types.Add(t1);
        types.Add(t2);

        if (types.Contains(CombineControl.CombineType.Toast) && types.Contains(CombineControl.CombineType.Carrot))
            return ToastCarrot;
        else if (types.Contains(CombineControl.CombineType.Toast) && types.Contains(CombineControl.CombineType.Sausage))
            return ToastSausage;
        else if (types.Contains(CombineControl.CombineType.ChickenLeg) && types.Contains(CombineControl.CombineType.Bread))
            return ChickenBread;
        else if (types.Contains(CombineControl.CombineType.ChickenLeg) && types.Contains(CombineControl.CombineType.Toast))
            return ChickenToast;

        return null;
    }

    public GameObject CreateObject(GameObject prefab, Vector3 pos)
    {
        // create a fx at spawn position
        GameObject fx = Instantiate(RunTimeEffects[4], pos, Quaternion.identity);
        Destroy(fx, 1f);

        // make the object
        GameObject obj = Instantiate(prefab, pos, Quaternion.identity);
        MoveControl mc = obj.GetComponent<MoveControl>();
        if (mc)
            mc.TargetPosition = pos;
        obj.SetActive(false);

        StartCoroutine("DelayInit", obj);

        return obj;
    }

    private IEnumerator DelayInit(GameObject obj)
    {
        yield return new WaitForSeconds(.1f);
        obj.SetActive(true);
        TouchControl tc = obj.GetComponent<TouchControl>();
        tc.OnTouch(obj.transform.position);
        yield return null;
        tc.OnDeTouch();
    }
}
