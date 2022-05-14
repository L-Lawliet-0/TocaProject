using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WheelControl : MonoBehaviour
{
    private bool Rotating;

    private void Start()
    {
        Rotating = true;
        GetComponent<TouchControl>().ClickCallBacks.Add(Flip);
    }

    public void Flip()
    {
        Rotating = !Rotating;
    }

    private void Update()
    {
        if (Rotating)
            transform.eulerAngles += Vector3.forward * 90 * Time.deltaTime;
    }
}
