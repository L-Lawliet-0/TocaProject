using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChuiSFX : MonoBehaviour
{
    private GameObject SFX;
    private SelectionControl SelectionControl;

    private void Start()
    {
        SelectionControl = GetComponent<SelectionControl>();
    }

    private void Update()
    {
        if (SelectionControl.Selected)
        {
            Collider2D[] colliders = Physics2D.OverlapCircleAll(transform.position + Vector3.up * .2f, 1.5f, 1 << LayerMask.NameToLayer("Selection"));

            bool find = false;
            foreach (Collider2D collider in colliders)
            {
                if (collider && collider.name.Equals("Head"))
                {
                    find = true;
                    break;
                }
            }

            if (find)
            {
                if (!SFX)
                    SFX = SoundManager.Instance.PlaySFX(5, false, transform.position);
            }
            else
            {
                if (SFX)
                    Destroy(SFX);
            }
        }
        else if (SFX)
            Destroy(SFX);
    }
}
