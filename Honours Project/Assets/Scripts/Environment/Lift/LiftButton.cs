using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LiftButton : MonoBehaviour, Interact
{
    [SerializeField] Lift lift;
    [SerializeField] bool up;

    public void OnEnter()
    {
    }

    public void OnExit()
    {
    }

    public void OnSelect()
    {
        lift.MoveToTarget(up);
    }
}
