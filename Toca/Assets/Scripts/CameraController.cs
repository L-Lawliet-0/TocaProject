using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    private static CameraController m_Instance;
    public static CameraController Instance { get { return m_Instance; } }
    private Camera m_Camera;
    private const float testWidthValue = 90; // max res = 9000, pixel mult is 100
    private float x_Min, x_Max;
    private float camera_Y;
    private float Target_X_Pixel;
    private const float Speed = 100;
    public float POSTOPIXEL;

    private void Awake()
    {
        m_Instance = this;
    }

    private void Start()
    {
        // get camera view width
        m_Camera = GetComponent<Camera>();

        float width = m_Camera.orthographicSize * 2 * m_Camera.aspect; // this is the width of the current camera
        float half_Target_Width = testWidthValue / 2;
        float half_Width = width / 2;

        x_Min = -half_Target_Width + half_Width;
        x_Max = half_Target_Width - half_Width;

        camera_Y = transform.position.y;

        Target_X_Pixel = 0;

        POSTOPIXEL = 1 / (m_Camera.ScreenToWorldPoint(Vector3.right).x - m_Camera.ScreenToWorldPoint(Vector3.zero).x);
    }

    public void UpdateCameraX(float x)
    {
        // the incoming x is in pixel
        Target_X_Pixel -= x;
        Target_X_Pixel = Mathf.Clamp(Target_X_Pixel, x_Min * POSTOPIXEL, x_Max * POSTOPIXEL);
    }

    private void Update()
    {
        // converted pixel into position
        float target = Target_X_Pixel / POSTOPIXEL;


        if (transform.position.x != target)
        {
            int sign = transform.position.x > target ? -1 : 1;

            transform.position += Vector3.right * Time.deltaTime * Speed * sign;

            int afterSign = transform.position.x > target ? -1 : 1;

            if (sign != afterSign)
                transform.position = new Vector3(target, camera_Y);
        }
    }
}
