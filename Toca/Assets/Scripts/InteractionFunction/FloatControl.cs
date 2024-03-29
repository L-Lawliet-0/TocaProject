using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FloatControl : TocaFunction
{
    private class floatStruct
    {
        public float Angle;
        public Vector3 ArrivedPos;
        public bool Arrived;
        public GameObject Ripple;
        public LayerControl TargetLayer;
        public BaseControl CoverBase;
    }
    public float FloatSpeed = 180;
    public float frequency = .07f;
    private BaseControl BaseControl;
    /// <summary>
    /// the dictionary contains the movecontrol and float, which indicated the angle used in sine wave function
    /// </summary>
    private Dictionary<MoveControl, floatStruct> Floaters;

    private void Start()
    {
        BaseControl = (BaseControl)TocaObject.GetTocaFunction<BaseControl>();
        Floaters = new Dictionary<MoveControl, floatStruct>();
    }

    private void Update()
    {
        List<MoveControl> remove = new List<MoveControl>();
        List<MoveControl> moveKeys = new List<MoveControl>(Floaters.Keys);
        // add new attachment to the list
        foreach (FindControl f in BaseControl.Attachments.Keys)
        {
            MoveControl mc = (MoveControl)f.TocaObject.GetTocaFunction<MoveControl>();

            if (!Floaters.ContainsKey(mc))
            {
                floatStruct fl = new floatStruct();
                fl.Angle = 0;
                fl.Arrived = false;
                Floaters.Add(mc, fl);
            }
        }

        // remove old shit that leaves the collection
        foreach (MoveControl mc in moveKeys)
        {
            FindControl fc = (FindControl)mc.TocaObject.GetTocaFunction<FindControl>();
            if (!BaseControl.Attachments.ContainsKey(fc))
                remove.Add(mc);
        }

        foreach (MoveControl mc in remove)
        {
            Destroy(Floaters[mc].Ripple);
            Floaters.Remove(mc);
        }

        // refresh keys
        moveKeys = new List<MoveControl>(Floaters.Keys);

        // move position
        for (int i = 0; i < moveKeys.Count; i++)
        {
            if (!Floaters[moveKeys[i]].Arrived)
            {
                FindControl fc = (FindControl)moveKeys[i].TocaObject.GetTocaFunction<FindControl>();
                if (fc.Arrived)
                {
                    SoundManager.Instance.PlaySFX(33, true, TocaObject.transform.position);
                    Floaters[moveKeys[i]].Arrived = true;
                    Floaters[moveKeys[i]].ArrivedPos = BaseControl.CalculateTargetPos(fc, BaseControl.Attachments[fc]); //moveKeys[i].transform.position;
                    StartCoroutine("DelayCreate", moveKeys[i]);
                }
            }
            else
            {
                // adjust each position in the list
                Floaters[moveKeys[i]].Angle += Time.deltaTime * FloatSpeed;
                Vector3 newPos = Floaters[moveKeys[i]].ArrivedPos + Vector3.up * Mathf.Sin(Floaters[moveKeys[i]].Angle * Mathf.Deg2Rad) * frequency;

                // move the position of the movecontrol
                moveKeys[i].UpdateTargetPosition(newPos);
                //moveKeys[i].TocaObject.transform.position = newPos;
                if (Floaters[moveKeys[i]].TargetLayer)
                {
                    Floaters[moveKeys[i]].Ripple.GetComponent<SpriteRenderer>().sortingOrder = Floaters[moveKeys[i]].TargetLayer.OrderValue + 1 + (Floaters[moveKeys[i]].CoverBase ? Floaters[moveKeys[i]].CoverBase.LayerCache : 0);
                    
                }
            }
        }
    }

    public void ResetArrivePos(MoveControl mc, Vector3 newPos)
    {
        if (Floaters.ContainsKey(mc))
            Floaters[mc].ArrivedPos = newPos;
    }

    private IEnumerator DelayCreate(MoveControl mc)
    {
        yield return new WaitForSeconds(.2f);
        CreateRipple(mc);
    }

    private void CreateRipple(MoveControl mc)
    {
        if (mc && Floaters.ContainsKey(mc))
        {
            FindControl fc = (FindControl)mc.TocaObject.GetTocaFunction<FindControl>();

            if (fc.ObjectWidth <= 11f)
            {

                Floaters[mc].Ripple = Instantiate(GlobalParameter.Instance.RunTimeEffects[2], fc.BottomCenter, Quaternion.identity);

                float mult = (fc.ObjectWidth + .5f) / 11.01f;
                Floaters[mc].Ripple.GetComponent<SpriteRenderer>().size = new Vector2(11 * mult, 1.92f * mult);

                LayerControl lc = (LayerControl)fc.TocaObject.GetTocaFunction<LayerControl>();
                Floaters[mc].Ripple.GetComponent<SpriteRenderer>().sortingOrder = lc.OrderValue + 1;
                Floaters[mc].TargetLayer = lc;

                BaseControl bc = (BaseControl)fc.TocaObject.GetTocaFunction<BaseControl>();
                if (bc && bc.MyBaseAttributes.HaveCover)
                {
                    Floaters[mc].CoverBase = bc;
                    Floaters[mc].Ripple.GetComponent<SpriteRenderer>().sortingOrder = lc.OrderValue + bc.LayerCache + 1;
                }
            }
        }
    }
}
