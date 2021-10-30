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
    }

    public void OnExit()
    {
    }

    public void OnSelect()
    {
        ship.Activate();
    }
}
