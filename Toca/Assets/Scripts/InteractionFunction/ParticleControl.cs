using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ParticleControl : TocaFunction
{
    public ParticleSystem ParticleSystem;
    void Start()
    {
        GetComponent<TouchControl>().ClickCallBacks.Add(Click);
    }

    public void Click()
    {

        ParticleSystem.loop = !ParticleSystem.loop;
        if (ParticleSystem.loop)
            ParticleSystem.Play();
    }
}
