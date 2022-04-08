using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AmmoItem : MonoBehaviour
{
    [SerializeField] float percent = 30;
    [SerializeField] bool active = true;

    private void OnTriggerEnter(Collider other)
    {
        if (!active) return;
        if (other.attachedRigidbody == null) return;
        if (other.attachedRigidbody.TryGetComponent(out PlayerDetails player))
        {
            //if (player.HealUp(amount)) Destroy(gameObject);
        }
    }

    public void SetActive(bool val)
    {
        active = val;
    }
}
