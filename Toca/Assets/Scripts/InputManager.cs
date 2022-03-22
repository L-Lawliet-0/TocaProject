using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InputManager : MonoBehaviour
{
    public bool InSelection; // the input is currently selecting something
    public TouchHandler SelectedObject; // the object this input is selected

    private void Awake()
    {
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
                SelectedObject = collider.GetComponentInParent<TouchHandler>();
                if (!SelectedObject)
                    SelectedObject = collider.GetComponent<TouchHandler>();
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


}
