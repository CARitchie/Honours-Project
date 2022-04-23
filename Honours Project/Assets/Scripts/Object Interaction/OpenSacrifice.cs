using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OpenSacrifice : MonoBehaviour, Interact
{
    public void OnEnter()
    {
        HUD.SetInteractText("Open Upgrades");
    }

    public void OnExit()
    {
        HUD.ClearInteractText();
    }

    public void OnSelect()
    {
        PauseMenu.OpenSacrificeMenu();      // Open the sacrifice menu
        HUD.DisableObjectiveMarker(1);      // Remove the objective marker
    }
}
