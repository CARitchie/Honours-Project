using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DoorControl : MonoBehaviour, Interact
{
    [SerializeField] Animator anim;
    [SerializeField] bool open;

    private void Start()
    {
        anim.SetBool("Open", open);
    }

    public void OnEnter()
    {
        DisplayText();
    }

    public void OnExit()
    {
        HUD.ClearInteractText();
    }

    public void OnSelect()
    {
        open = !open;                   // Toggle the state of the door
        DisplayText();
        anim.SetBool("Open", open);     // Play the appropriate door animation
    }

    void DisplayText()
    {
        if (open)
        {
            HUD.SetInteractText("Close");
        }
        else
        {
            HUD.SetInteractText("Open");
        }
    }
}
