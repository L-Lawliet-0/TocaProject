using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InputManager : MonoBehaviour
{
    public bool InSelection; // the input is currently selecting something
    public ISelectable SelectedObject; // the object this input is selected

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

            Collider2D collider = Physics2D.OverlapBox(pos, Vector2.one, 0, 1 << LayerMask.NameToLayer("Player"));
            if (collider)
                SelectedObject = collider.GetComponentInParent<ISelectable>();
            else
                SelectedObject = null;

            if (Input.GetMouseButtonDown(0) && SelectedObject)
            {
                // used to fire up onselect event
                SelectedObject.OnSelect(pos);
                InSelection = true;
            }
        }
        else
        {
            if (Input.GetMouseButtonUp(0))
            {
                // used to fire up ondeselect event
                SelectedObject.OnDeSelect();
                InSelection = false;
            }
            else if (Input.GetMouseButton(0))
            {
                // calculate follow position of currently selected object
                SelectedObject.UpdateTargetPosition(GlobalParameter.Instance.ScreenPosToGamePos(Input.mousePosition));
            }
        }
    }


}
