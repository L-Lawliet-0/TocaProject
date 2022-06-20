using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InputManager : MonoBehaviour
{
    public bool TouchMode;

    private static InputManager m_Instance;
    public static InputManager Instance { get { return m_Instance; } }
    public bool InSelection; // the input is currently selecting something
    public Vector2 LastScreenPosition; // the last input scrren position, in pixel
    public TouchControl SelectedObject; // the object this input is selected

    private Dictionary<int, TouchInfo> Touches;

    private float ClickingTime;

    private class TouchInfo
    {
        private TouchControl SelectedObject;
        public bool InSelection;
        public Touch MyTouch;
        private Vector2 LastScreenPosition;

        private bool UpdateCamera;
        private float ClickingTime;

        public TouchInfo(Touch touch) 
        {
            MyTouch = touch;
            InSelection = false;
            SelectedObject = null;
            ClickingTime = 0;
        }

        public void FrameUpdate()
        {
            if (MyTouch.phase == TouchPhase.Began)
            {
                OnTouch();
            }
            else if (MyTouch.phase == TouchPhase.Canceled || MyTouch.phase == TouchPhase.Ended)
            {
                Detouch();
            }
            else
            {
                UpdatePos();
            }
        }

        public void OnTouch()
        {
            LastScreenPosition = MyTouch.position;
            UpdateSelectedObject();
            UpdateCamera = (SelectedObject == null || (!SelectedObject.TocaObject.GetTocaFunction<SelectionControl>() && SelectedObject.PositionChangeCallBacksV3.Count == 0));

            //if (SelectedObject)
            //    SelectedObject.OnClick(GlobalParameter.Instance.ScreenPosToGamePos(MyTouch.position));
        }

        private void UpdateSelectedObject()
        {
            SelectedObject = null;
            RaycastHit2D[] hits = Physics2D.RaycastAll(Camera.main.ScreenToWorldPoint(MyTouch.position), Vector2.zero, 999, 1 << LayerMask.NameToLayer("Selection"));
            float maxLayer = float.MinValue;
            foreach (RaycastHit2D hit in hits)
            {
                TouchControl tc = hit.collider.GetComponentInParent<TouchControl>();
                LayerControl lc = (LayerControl)tc.TocaObject.GetTocaFunction<LayerControl>();
                float layerValue = lc.OrderValue;
                if (tc.GetComponent<SpriteRenderer>())
                    layerValue = tc.GetComponent<SpriteRenderer>().sortingOrder;
                if (layerValue > maxLayer)
                {
                    maxLayer = layerValue;
                    SelectedObject = tc;
                }
            }
        }

        public void Detouch()
        {
            if (SelectedObject && !InSelection && ClickingTime < .15f)
                SelectedObject.OnClick(GlobalParameter.Instance.ScreenPosToGamePos(MyTouch.position));

            if (InSelection)
            {
                InSelection = false;
                SelectedObject.OnDeTouch();

                if (SelectedObject.TocaObject.GetTocaFunction<MoveControl>())
                    ((MoveControl)SelectedObject.TocaObject.GetTocaFunction<MoveControl>()).InstantGo = false;
            }
        }

        public void UpdatePos()
        {
            ClickingTime += Time.deltaTime;
            if (UpdateCamera)
            {
                // not selecting object, update camera position
                CameraController.Instance.UpdateCameraX(MyTouch.position.x - LastScreenPosition.x);
                LastScreenPosition = MyTouch.position;
            }
            else
            {
                if (!InSelection)
                {
                    UpdateSelectedObject();
                    if (SelectedObject)
                    {
                        if (ClickingTime > .15f || (Vector2.Distance(LastScreenPosition, MyTouch.position) > 1))
                        {
                            Vector3 pos = GlobalParameter.Instance.ScreenPosToGamePos(MyTouch.position);
                            SelectedObject.OnTouch(pos);
                            InSelection = true;
                        }
                    }
                    else
                    {
                        UpdateCamera = true;
                        
                    }
                    LastScreenPosition = MyTouch.position;
                }
                else
                {
                    SelectedObject.OnTouchPositionChanged(GlobalParameter.Instance.ScreenPosToGamePos(MyTouch.position));
                    bool instantGo = false;
                    // if the touch position is at the border, move the camera position
                    if (MyTouch.position.x > Screen.width * .95f)
                    {
                        float multiplier = (MyTouch.position.x - Screen.width * .9f) / (Screen.width * .1f) * .005f;
                        CameraController.Instance.UpdateCameraX(-Screen.width * multiplier);
                        instantGo = true;
                    }
                    else if (MyTouch.position.x < Screen.width * .1f)
                    {
                        float multiplier = ((Screen.width * .1f - MyTouch.position.x) / (Screen.width * .1f)) * .005f;
                        CameraController.Instance.UpdateCameraX(Screen.width * multiplier);
                        instantGo = true;
                    }

                    if (SelectedObject.TocaObject.GetTocaFunction<MoveControl>())
                        ((MoveControl)SelectedObject.TocaObject.GetTocaFunction<MoveControl>()).InstantGo = instantGo;
                }
            }
        }
    }

    private void Awake()
    {
        m_Instance = this;
        InSelection = false;

        Touches = new Dictionary<int, TouchInfo>();
    }

    // The job of the input manager is to detect input and fire events
    private bool UpdateCamera;
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
                    if (tc.GetComponent<SpriteRenderer>())
                        layerValue = tc.GetComponent<SpriteRenderer>().sortingOrder;
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

                if (Input.GetMouseButtonDown(0))
                {
                    EmoteControl.Instance.EmoteCheck(Input.mousePosition);
                    LastScreenPosition = Input.mousePosition;
                    UpdateCamera = (SelectedObject == null || (!SelectedObject.TocaObject.GetTocaFunction<SelectionControl>() && SelectedObject.PositionChangeCallBacksV3.Count == 0));
                    //if (SelectedObject)
                    //    SelectedObject.OnClick(GlobalParameter.Instance.ScreenPosToGamePos(Input.mousePosition));
                }
                else if (Input.GetMouseButton(0))
                {
                    if (SelectedObject && !UpdateCamera)
                    {
                        if (ClickingTime > .15f)
                        {
                            // finger touched object
                            pos.z = GlobalParameter.Depth;
                            SelectedObject.OnTouch(pos);
                            InSelection = true;
                            ClickingTime = 0;
                        }
                        LastScreenPosition = Input.mousePosition;
                    }
                    else
                    {
                        UpdateCamera = true;
                        CameraController.Instance.UpdateCameraX(Input.mousePosition.x - LastScreenPosition.x);
                        LastScreenPosition = Input.mousePosition;
                    }
                    ClickingTime += Time.deltaTime;
                }
                else if (Input.GetMouseButtonUp(0))
                {
                    if (SelectedObject && ClickingTime < .15f)
                        SelectedObject.OnClick(GlobalParameter.Instance.ScreenPosToGamePos(Input.mousePosition));
                    ClickingTime = 0;
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
                if (!Touches.ContainsKey(touch.fingerId))
                {
                    TouchInfo info = new TouchInfo(touch);
                    Touches.Add(touch.fingerId, info);

                    EmoteControl.Instance.EmoteCheck(touch.position);
                }
            }

            List<Touch> removeThisFrame = new List<Touch>();
            foreach (Touch touch in touches)
            {
                Touches[touch.fingerId].MyTouch = touch;
                Touches[touch.fingerId].FrameUpdate();
                if (touch.phase == TouchPhase.Canceled || touch.phase == TouchPhase.Ended)
                    removeThisFrame.Add(touch);
            }

            foreach (Touch touch in removeThisFrame)
            {
                Touches.Remove(touch.fingerId);
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
