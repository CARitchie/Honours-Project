using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectInteractor : MonoBehaviour
{
    Interact interact;
    int layerMask = 1 << 6;

    private void Start()
    {
        InputController.Interact += Select;
    }

    private void OnDestroy()
    {
        InputController.Interact -= Select;
    }

    private void LateUpdate()
    {
        if(Physics.Raycast(transform.position,transform.forward,out RaycastHit hit, 5, layerMask))
        {
            if(Physics.Raycast(transform.position, transform.forward, out RaycastHit hit2, 5))
            {
                if(hit2.collider != hit.collider)
                {
                    Deselect();
                    return;
                }
            }

            if (hit.collider.TryGetComponent(out Interact newInteract))
            {
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
