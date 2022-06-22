using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ParticleControl : TocaFunction
{
    public ParticleSystem ParticleSystem;
    private GameObject SFX;
    void Start()
    {
        GetComponent<TouchControl>().ClickCallBacks.Add(Click);

        ParticleSystem.loop = true;
        Click();
    }

    public void Click()
    {

        ParticleSystem.loop = !ParticleSystem.loop;
        if (ParticleSystem.loop)
        {
            ParticleSystem.Play();
            SFX = SoundManager.Instance.PlaySFX(29, false, TocaObject.transform.position);
        }
        else
            Destroy(SFX);
    }
}
