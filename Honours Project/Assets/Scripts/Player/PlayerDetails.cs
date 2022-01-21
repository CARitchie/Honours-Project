using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerDetails : PersonDetails
{
    public override void OnShot(float damage)
    {
        base.OnShot(damage);
        Debug.Log("Ouch: " + HealthPercent());
    }
}
