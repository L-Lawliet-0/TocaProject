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

                GlobalParameter.Instance.CreateObject(GlobalParameter.Instance.GetCombinePrefab(MyType, cc.MyType), transform.position);
                SoundManager.Instance.PlaySFX(32, true, TocaObject.transform.position);

                Destroy(cc.TocaObject.gameObject);
                Destroy(gameObject);
                break;
            }
        }
    }
}
