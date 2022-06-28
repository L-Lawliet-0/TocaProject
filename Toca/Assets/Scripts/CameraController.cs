using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    private static CameraController m_Instance;
    public static CameraController Instance { get { return m_Instance; } }
    private Camera m_Camera;
    public float testWidthValue = 90; // max res = 9000, pixel mult is 100
    private float x_Min, x_Max;
    private float camera_Y;
    private float Target_X_Pixel;

    public float CamWidth;

    private const float MaxSpeed = 40;
    private float Speed = MaxSpeed;
    public float POSTOPIXEL;
    public float Width_Half;

    private float acceleration;

    private void Awake()
    {
        m_Instance = this;
    }

    private void Start()
    {
        // get camera view width
        m_Camera = GetComponent<Camera>();

        CamWidth = m_Camera.orthographicSize * 2 * m_Camera.aspect; // this is the width of the current camera
        TrackControl.Instance.OffsetRange = CamWidth / 4;
        TrackControl.Instance.CharacterShadowLeft.transform.localPosition = new Vector3(-CamWidth, -1.5f);
        TrackControl.Instance.CharacterShadowRight.transform.localPosition = new Vector3(CamWidth, -1.5f);

        float half_Target_Width = testWidthValue / 2;
        float half_Width = CamWidth / 2;
        Width_Half = half_Width;

        x_Min = -half_Target_Width + half_Width;
        x_Max = half_Target_Width - half_Width;

        camera_Y = transform.position.y;

        Target_X_Pixel = 0;

        POSTOPIXEL = 1 / (m_Camera.ScreenToWorldPoint(Vector3.right).x - m_Camera.ScreenToWorldPoint(Vector3.zero).x);
    }

    public void ResetWidth(float newWidth)
    {
        testWidthValue = newWidth;

        float width = m_Camera.orthographicSize * 2 * m_Camera.aspect; // this is the width of the current camera
        float half_Target_Width = testWidthValue / 2;
        float half_Width = width / 2;
        Width_Half = half_Width;

        x_Min = -half_Target_Width + half_Width;
        x_Max = half_Target_Width - half_Width;
    }

    public SunControl Sun;
    public void UpdateCameraX(float x)
    {
        if (!Main.Instance.CanCameraMove)
            return;
        
        // the incoming x is in pixel
        if (acceleration > 0)
        {
            Target_X_Pixel = transform.position.x * POSTOPIXEL;
            acceleration = 0;
        }
        Target_X_Pixel -= x;
        Target_X_Pixel = Mathf.Clamp(Target_X_Pixel, x_Min * POSTOPIXEL, x_Max * POSTOPIXEL);

        float temp = Mathf.Abs(x) / POSTOPIXEL / Time.deltaTime / 4;
        Speed = Mathf.Max(temp, Speed);
        Speed = Mathf.Clamp(Speed, 0, 40);

    }

    private void Update()
    {
        if (!Main.Instance.CanCameraMove)
            return;

        // converted pixel into position
        float target = Target_X_Pixel / POSTOPIXEL;


        if (transform.position.x != target)
        {
            int sign = transform.position.x > target ? -1 : 1;

            float delta = Time.deltaTime * Speed * sign;
            Speed -= acceleration * Time.deltaTime;
            transform.position += Vector3.right * delta;

            int afterSign = transform.position.x > target ? -1 : 1;

            if (sign != afterSign)
            {
                transform.position = new Vector3(target, transform.position.y);

                if (Speed > 0 && acceleration == 0)
                {
                    Speed = Mathf.Clamp(Speed, 0, 40);
                    acceleration = Speed;
                    target += Speed * (Speed / acceleration) / 2 * sign;
                    Target_X_Pixel = Mathf.Clamp(target * POSTOPIXEL, x_Min * POSTOPIXEL, x_Max * POSTOPIXEL);
                }
            }

        }
    }

    public void ResetCamera()
    {
        Target_X_Pixel = 0;
        transform.position = new Vector3(0, 7.68f);
    }

    public float GetCameraOffset()
    {
        float value = (transform.position.x - x_Min) / (x_Max - x_Min);
        return value;
    }
}
