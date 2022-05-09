using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EatControl : TocaFunction
{
    private BaseControl BaseControl;
    private SpineControl SpineControl;
    private CatControl CatControl;

    public FindControl PendingFood;
    public WaterFillControl PendingWater;
    public BottleControl PendingBottle;

    private void Start()
    {
        BaseControl = GetComponent<BaseControl>();
        SpineControl = (SpineControl)TocaObject.GetTocaFunction<SpineControl>();
        CatControl = (CatControl)TocaObject.GetTocaFunction<CatControl>();
    }

    private void Update()
    {
        if (SpineControl)
        {
            if (PendingFood && PendingFood.BasePreview == BaseControl)
                SpineControl.SetOpenMouse();
            else if (PendingWater)
            {
                if (PendingWater.WaterFilled)
                    SpineControl.SetOpenMouse();
                else
                    PendingWater = null;
            }
            else if (PendingBottle && PendingBottle.Mouth == BaseControl)
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

        if (CatControl)
        {
            if (PendingFood && PendingFood.BasePreview == BaseControl)
                CatControl.SetMouth(true);
            else if (PendingWater)
            {
                if (PendingWater.WaterFilled)
                    CatControl.SetMouth(true);
                else
                    PendingWater = null;
            }
            else if (PendingBottle && PendingBottle.Mouth == BaseControl)
                CatControl.SetMouth(true);
            else
                CatControl.SetMouth(false);

            List<FindControl> keys = new List<FindControl>(BaseControl.Attachments.Keys);
            for (int i = keys.Count - 1; i >= 0; i--)
            {
                FindControl fc = keys[i];
                fc.TryDetach();
                Destroy(fc.TocaObject.gameObject);

                // play eating animation
                CatControl.PlayEat();
            }
        }
    }
}
