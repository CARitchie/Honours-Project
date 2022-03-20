using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PowerItem : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        if(other.attachedRigidbody.TryGetComponent(out PlayerDetails player))
        {
            player.AddPowerCell();
            Destroy(gameObject);
        }
    }
}