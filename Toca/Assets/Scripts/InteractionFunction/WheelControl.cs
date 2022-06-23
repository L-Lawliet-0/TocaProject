using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WheelControl : MonoBehaviour
{
    private bool Rotating;
    public bool PauseBGM = false;
    public float RotationSpeed = 90;

    private void Start()
    {
        Rotating = true;
        GetComponent<TouchControl>().ClickCallBacks.Add(Flip);
    }

    public void Flip()
    {
        Rotating = !Rotating;

        if (PauseBGM)
        {
            if (Rotating)
                SoundManager.Instance.BackGroundMusic.UnPause();
            else
                SoundManager.Instance.BackGroundMusic.Pause();
        }
    }

    private void Update()
    {
        if (Rotating)
            transform.eulerAngles += Vector3.forward * RotationSpeed * Time.deltaTime;
    }
}
