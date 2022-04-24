using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EatControl : TocaFunction
{
    private BaseControl BaseControl;
    private SpineControl SpineControl;

    public FindControl PendingFood;

    private void Start()
    {
        BaseControl = GetComponent<BaseControl>();
        SpineControl = (SpineControl)TocaObject.GetTocaFunction<SpineControl>();
    }

    private void Update()
    {
        if (PendingFood && PendingFood.BasePreview == BaseControl)
            SpineControl.SetOpenMouse();
        else
            SpineControl.SetDefaultMouse();

        List<FindControl> keys = new List<FindControl>(BaseControl.Attachments.Keys);
        for (int i = keys.Count - 1; i >= 0; i--)
        {
            FindControl fc = keys[i];
            fc.TryDetach();
            Destroy(fc.TocaObject.gameObject);

            // play eating animation
            SpineControl.PlayAnimation(SpineControl.Animations.Eat);
        }
    }
}
