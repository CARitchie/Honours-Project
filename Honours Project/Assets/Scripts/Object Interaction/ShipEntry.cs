using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShipEntry : MonoBehaviour, Interact
{
    [SerializeField] GameObject message;

    ShipController ship;

    private void Awake()
    {
        ship = GetComponentInParent<ShipController>();
    }

    public void OnEnter()
    {
        message.SetActive(true);
    }

    public void OnExit()
    {
        message.SetActive(false);
    }

    public void OnSelect()
    {
        ship.Activate();
    }
}
