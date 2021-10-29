using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShipEntry : MonoBehaviour, Interact
{
    public void OnEnter()
    {
        Debug.Log("Enter Ship");
    }

    public void OnExit()
    {
        Debug.Log("Exit Ship");
    }

    public void OnSelect()
    {
        Debug.Log("Select Ship");
    }
}
