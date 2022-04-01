using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectInteractor : MonoBehaviour
{
    Interact interact;
    int layerMask = 1 << 6;

    public static bool podGrabbed = false;

    private void Start()
    {
        podGrabbed = false;
        InputController.Interact += Select;
    }

    private void OnDestroy()
    {
        InputController.Interact -= Select;
    }

    private void LateUpdate()
    {
        if (podGrabbed) return;

        // If an interactive object is hit
        if(Physics.Raycast(transform.position,transform.forward,out RaycastHit hit, 5, layerMask))
        {
            // If any other object is hit beforehand
            if(Physics.Raycast(transform.position, transform.forward, out RaycastHit hit2, 5))
            {
                // If the other object is not the interactive object
                if(hit2.collider != hit.collider)
                {
                    Deselect();
                    return;
                }
            }

            // If the interactive object has an interactive component
            if (hit.collider.TryGetComponent(out Interact newInteract))
            {
                // If the interactive object is not the current object
                if (interact != newInteract)
                {
                    Deselect();
                    interact = newInteract;
                    interact.OnEnter();
                }
            }
        }
        else
        {
            Deselect();
        }
    }

    void Deselect()
    {
        if (interact != null)
        {
            interact.OnExit();
            interact = null;
        }
    }

    public void Select()
    {
        if (interact != null) interact.OnSelect();
    }
}
