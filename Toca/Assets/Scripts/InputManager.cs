using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InputManager : MonoBehaviour
{
    private static InputManager m_Instance;
    public static InputManager Instance { get { return m_Instance; } }
    public bool InSelection; // the input is currently selecting something
    public TouchControl SelectedObject; // the object this input is selected


    private void Awake()
    {
        m_Instance = this;
        InSelection = false;
    }

    // The job of the input manager is to detect input and fire events

    private void Update()
    {
        if (!InSelection)
        {
            Vector3 pos = GlobalParameter.Instance.ScreenPosToGamePos(Input.mousePosition);

            Collider2D collider = Physics2D.OverlapBox(pos, Vector2.one, 0, 1 << LayerMask.NameToLayer("Selection"));
            if (collider)
            {
                SelectedObject = collider.GetComponentInParent<TouchControl>();
                if (!SelectedObject)
                    SelectedObject = collider.GetComponent<TouchControl>();
            }
            else
                SelectedObject = null;

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
