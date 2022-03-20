using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RechargeStation : MonoBehaviour
{
    [SerializeField] float rechargeRate;
    PlayerDetails player;


    private void OnTriggerEnter(Collider other)
    {
        if (other.attachedRigidbody.TryGetComponent(out PlayerDetails player))
        {
            this.player = player;
            StartCoroutine(RechargePlayer());
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.attachedRigidbody.TryGetComponent(out PlayerDetails player))
        {
            this.player = null;
        }
    }

    IEnumerator RechargePlayer()
    {
        while(player != null)
        {
            player?.Recharge(rechargeRate * Time.deltaTime);
            yield return new WaitForEndOfFrame();
        }
    }
}