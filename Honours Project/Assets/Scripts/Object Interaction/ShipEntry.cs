using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShipEntry : MonoBehaviour, Interact
{
    ShipController ship;

    private void Awake()
    {
        ship = GetComponentInParent<ShipController>();
    }

    public void OnEnter()
    {
        if (ship.IsActive()) return;

        HUD.SetInteractText("Control/Exit Spaceship");
    }

    public void OnExit()
    {
        HUD.ClearInteractText();
    }

    public void OnSelect()
    {
        ship.Activate();
        OnExit();
    }
}
