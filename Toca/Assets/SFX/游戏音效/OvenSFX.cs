using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OvenSFX : MonoBehaviour
{
    private OpenControl OpenControl;
    private GameObject SFX;
    // Start is called before the first frame update
    void Start()
    {
        OpenControl = GetComponent<OpenControl>();
        
    }

    // Update is called once per frame
    void Update()
    {
        if (OpenControl.Opening && SFX)
        {
            Destroy(SFX);
        }
        else if (!OpenControl.Opening && !SFX)
            SFX = SoundManager.Instance.PlaySFX(11, false, transform.position);
    }
}
