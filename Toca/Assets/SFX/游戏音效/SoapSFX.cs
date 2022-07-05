using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoapSFX : MonoBehaviour
{
    private GameObject SFX;
    private TouchControl Tc;
    private SelectionControl m_Sc;
    public ParticleSystem Particle;

    private void Start()
    {
        Tc = GetComponent<TouchControl>();
        Tc.TouchCallBacks.Add(Select);
        Tc.DeTouchCallBacks.Add(DeSelect);

        m_Sc = GetComponent<SelectionControl>();
    }

    private void Update()
    {
        if (SFX)
            SFX.transform.position = transform.position;

        if (m_Sc && Particle)
        {
            if (m_Sc.Selected && !Particle.isPlaying)
            {
                Particle.loop = true;
                Particle.Play();
            }
            else if (!m_Sc.Selected && Particle.isPlaying)
                Particle.loop = false;
        }
    }

    private void Select()
    {
        if (!SFX)
            SFX = SoundManager.Instance.PlaySFX(28, false, transform.position);
    }

    private void DeSelect()
    {
        Destroy(SFX);
    }
}
