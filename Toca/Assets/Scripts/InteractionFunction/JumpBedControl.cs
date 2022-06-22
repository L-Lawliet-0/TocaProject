using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class JumpBedControl : TocaFunction
{
    private const float Gravity = -9.8f;
    private const float BounceForceMin = 5, BounceForceMax = 10;
    private BaseControl m_BaseControl;

    public Sprite Jumped_Sprite, DefaultSprite; // the jumped sprite
    private SpriteRenderer m_SpriteRenderer;

    private class BedStruture
    {
        public MoveControl MC;
        public float BounceBase;
        public float Velocity;
    }
    private Dictionary<FindControl, BedStruture> Dict;

        

    private void Start()
    {
        Dict = new Dictionary<FindControl, BedStruture>();
        m_BaseControl = (BaseControl)TocaObject.GetTocaFunction<BaseControl>(); // get the base control
        m_SpriteRenderer = GetComponent<SpriteRenderer>();
        DefaultSprite = m_SpriteRenderer.sprite;
    }

    private void Update()
    {
        List<FindControl> keys = new List<FindControl>(m_BaseControl.Attachments.Keys);
        foreach (FindControl fc in keys)
        {
            if (!Dict.ContainsKey(fc))
            {
                BedStruture bs = new BedStruture();
                bs.MC = (MoveControl)fc.TocaObject.GetTocaFunction<MoveControl>();
                bs.BounceBase = m_BaseControl.GetTargetPos(fc, m_BaseControl.Attachments[fc]).y;
                if (!fc.TocaObject.GetTocaFunction<SpineControl>())
                    bs.BounceBase += fc.ObjectHeight / 2;
                bs.Velocity = Random.Range(BounceForceMin, BounceForceMax);

                Dict.Add(fc, bs);
            }
        }

        // remove shit not there
        List<FindControl> remove = new List<FindControl>();
        keys = new List<FindControl>(Dict.Keys);
        foreach(FindControl fc in keys)
        {
            if (!m_BaseControl.Attachments.ContainsKey(fc))
                remove.Add(fc);
        }

        foreach (FindControl fc in remove)
            Dict.Remove(fc);

        // update
        foreach (KeyValuePair<FindControl, BedStruture> pair in Dict)
        {
            // update velocity
            pair.Value.Velocity += Gravity * Time.deltaTime;
            if (pair.Value.Velocity < 0 && pair.Key.TocaObject.transform.position.y < pair.Value.BounceBase)
            {
                pair.Value.Velocity = Random.Range(BounceForceMin, BounceForceMax);
                m_SpriteRenderer.sprite = Jumped_Sprite;
                SoundManager.Instance.PlaySFX(30, true, TocaObject.transform.position);
                CancelInvoke();
                Invoke("SwapBack", .3f);
            }

            // update position
            pair.Key.TocaObject.transform.position = pair.Key.TocaObject.transform.position + pair.Value.Velocity * Time.deltaTime * Vector3.up;
            Debug.LogError("updated position");
        }
    }

    private void SwapBack()
    {
        m_SpriteRenderer.sprite = DefaultSprite;
    }
}
