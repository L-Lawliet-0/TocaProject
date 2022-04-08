using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PosLog : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        Log(transform.position.ToString());
    }

    // Update is called once per frame
    void Update()
    {
        Log(transform.position.ToString());
    }

    private void Log(string log)
    {
        Debug.LogError("Debugging: " + log);
    }
}
