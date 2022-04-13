using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LiftSummon : MonoBehaviour, Interact
{
    [SerializeField] Lift lift;
    [SerializeField] int index;

    public void OnEnter()
    {
        HUD.SetInteractText("Call Lift");
    }

    public void OnExit()
    {
        HUD.ClearInteractText();
    }

    public void OnSelect()
    {
        lift.Summon(index);
    }
}
