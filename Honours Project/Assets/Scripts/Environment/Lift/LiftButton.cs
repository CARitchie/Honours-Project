using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LiftButton : MonoBehaviour, Interact
{
    [SerializeField] Lift lift;
    [SerializeField] bool up;

    public void OnEnter()
    {
        if (up)
        {
            HUD.SetInteractText("Go Up");
        }
        else
        {
            HUD.SetInteractText("Go Down");
        }
    }

    public void OnExit()
    {
        HUD.ClearInteractText();
    }

    public void OnSelect()
    {
        lift.MoveToTarget(up);
    }
}
