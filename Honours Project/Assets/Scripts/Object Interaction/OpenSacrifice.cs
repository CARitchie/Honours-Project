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
        PauseMenu.OpenSacrificeMenu();
        HUD.DisableObjectiveMarker(1);
    }
}
