using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InputManager : MonoBehaviour
{
    public bool TouchMode;

    private static InputManager m_Instance;
    public static InputManager Instance { get { return m_Instance; } }
    public bool InSelection; // the input is currently selecting something
    public TouchControl SelectedObject; // the object this input is selected

    private Dictionary<Touch, TouchInfo> Touches;

    private class TouchInfo
    {
        private TouchControl SelectedObject;
        public bool InSelection;
        public Touch MyTouch;

        public TouchInfo(Touch touch)
        {
            MyTouch = touch;
            InSelection = false;
            SelectedObject = null;
        }

        public void FrameUpdate()
        {
            if (MyTouch.phase == TouchPhase.Began)
                OnTouch();
            else if (MyTouch.phase == TouchPhase.Canceled || MyTouch.phase == TouchPhase.Ended)
                Detouch();
            else
                UpdatePos();
        }

        public void OnTouch()
        {
            Vector3 pos = GlobalParameter.Instance.ScreenPosToGamePos(MyTouch.position);

            RaycastHit2D hit = Physics2D.Raycast(Camera.main.ScreenToWorldPoint(Input.mousePosition), Vector2.zero, 999, 1 << LayerMask.NameToLayer("Selection"));
            if (hit)
            {
                SelectedObject = hit.collider.GetComponentInParent<TouchControl>();
            }
            else
                SelectedObject = null;

            if (SelectedObject)
            {
                // finger touched object
                SelectedObject.OnTouch(pos);
                InSelection = true;
            }
        }

        public void Detouch()
        {
            if (InSelection)
            {
                InSelection = false;
                SelectedObject.OnDeTouch();
            }
        }

        public void UpdatePos()
        {
            if (InSelection)
            {
                SelectedObject.OnTouchPositionChanged(GlobalParameter.Instance.ScreenPosToGamePos(MyTouch.position));
            }
        }
    }

    private void Awake()
    {
        m_Instance = this;
        InSelection = false;

        Touches = new Dictionary<Touch, TouchInfo>();
    }

    // The job of the input manager is to detect input and fire events

    private void Update()
    {
        if (!TouchMode)
        {
            if (!InSelection)
            {
                SelectedObject = null;
                Vector3 pos = GlobalParameter.Instance.ScreenPosToGamePos(Input.mousePosition);
                pos.z = 0;

                RaycastHit2D[] hits = Physics2D.RaycastAll(Camera.main.ScreenToWorldPoint(Input.mousePosition), Vector2.zero, 999, 1 << LayerMask.NameToLayer("Selection"));
                float maxLayer = float.MinValue;
                foreach (RaycastHit2D hit in hits)
                {
                    TouchControl tc = hit.collider.GetComponentInParent<TouchControl>();
                    LayerControl lc = (LayerControl)tc.TocaObject.GetTocaFunction<LayerControl>();
                    float layerValue = lc.OrderValue;
                    if (layerValue > maxLayer)
                    {
                        maxLayer = layerValue;
                        SelectedObject = tc;
                    }
                }

                /*
                if (hit)
                {
                    SelectedObject = hit.collider.GetComponentInParent<TouchControl>();
                }
                else
                    SelectedObject = null;

                /*
                Collider2D collider = Physics2D.OverlapPoint(pos, 1 << LayerMask.NameToLayer("Selection"));
                if (collider)
                {
                    SelectedObject = collider.GetComponentInParent<TouchControl>();
                    if (!SelectedObject)
                        SelectedObject = collider.GetComponent<TouchControl>();
                }
                else
                    SelectedObject = null;
                */

                if (Input.GetMouseButtonDown(0) && SelectedObject)
                {
                    // finger touched object
                    SelectedObject.OnTouch(pos);
                    InSelection = true;
                }
            }
            else
            {
                if (Input.GetMouseButtonUp(0))
                {
                    // finger lifted up
                    SelectedObject.OnDeTouch();
                    InSelection = false;
                }
                else if (Input.GetMouseButton(0))
                {
                    // finger position changed
                    SelectedObject.OnTouchPositionChanged(GlobalParameter.Instance.ScreenPosToGamePos(Input.mousePosition));
                }
            }
        }
        else
        {
            Touch[] touches = Input.touches;
            foreach (Touch touch in touches)
            {
                if (!Touches.ContainsKey(touch))
                {
                    TouchInfo info = new TouchInfo(touch);
                    Touches.Add(touch, info);
                }
            }

            List<Touch> removeThisFrame = new List<Touch>();
            foreach (Touch touch in touches)
            {
                Touches[touch].FrameUpdate();
                if (touch.phase == TouchPhase.Canceled || touch.phase == TouchPhase.Ended)
                    removeThisFrame.Add(touch);
            }

            foreach (Touch touch in removeThisFrame)
            {
                Touches.Remove(touch);
            }
        }
    }

    public Vector3 GetTouchPosition(int id = -1, bool world = true)
    {
        if (world)
        {
            return GlobalParameter.Instance.ScreenPosToGamePos(Input.mousePosition);
        }
        else
            return Input.mousePosition;
    }
}
