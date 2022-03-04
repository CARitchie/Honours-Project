using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LiftSummon : MonoBehaviour, Interact
{
    [SerializeField] Lift lift;
    [SerializeField] int index;

    public void OnEnter()
    {
    }

    public void OnExit()
    {
    }

    public void OnSelect()
    {
        lift.Summon(index);
    }
}
