using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CombineControl : TocaFunction
{
    private bool Combined;
    public enum CombineType
    {
        Toast,
        Carrot,
        Sausage,
        Bread,
        ChickenLeg
    }

    public CombineType MyType;
    public CombineType[] CombineWith;
    private TouchControl MyTouch;
    private void Start()
    {
        TouchControl tc = (TouchControl)TocaObject.GetTocaFunction<TouchControl>();
        tc.DeTouchCallBacks.Add(TryCombine);
        MyTouch = (TouchControl)TocaObject.GetTocaFunction<TouchControl>();
        Combined = false;
    }

    public void TryCombine()
    {
        if (Combined)
            return;
        Collider2D[] colliders = Physics2D.OverlapCircleAll(transform.position, .5f, 1 << LayerMask.NameToLayer("Selection"));
        foreach (Collider2D collider in colliders)
        {
            TouchControl tc = collider.GetComponentInParent<TouchControl>();
            CombineControl cc = (CombineControl)tc.TocaObject.GetTocaFunction<CombineControl>();

            bool meet = cc;
            if (meet)
            {
                meet = false;
                foreach (CombineType ct in CombineWith)
                {
                    if (ct == cc.MyType)
                        meet = true;
                }
            }
            if (meet)
            {
                Combined = true;
                GameObject fx = Instantiate(GlobalParameter.Instance.RunTimeEffects[4], transform.position, Quaternion.identity);
                Destroy(fx, 1f);

                GameObject obj = Instantiate(GlobalParameter.Instance.GetCombinePrefab(MyType, cc.MyType));
                obj.transform.position = transform.position;
                MoveControl mc = obj.GetComponent<MoveControl>();
                if (mc)
                    mc.TargetPosition = transform.position;
                obj.SetActive(false);
                Destroy(cc.TocaObject.gameObject);

                StartCoroutine("DelayInit", obj);

                break;
            }
        }
    }

    private IEnumerator DelayInit(GameObject obj)
    {
        yield return new WaitForSeconds(.1f);

        obj.SetActive(true);
        TouchControl tc = obj.GetComponent<TouchControl>();
        tc.OnTouch(obj.transform.position);
        yield return null;
        tc.OnDeTouch();
        Destroy(gameObject);
    }
}
