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

        if (other.CompareTag("Player"))         // If the player has entered the collider
        {
            if (PlayerController.Instance.AddAmmo(percent / 100)) Useful.DestroyGameObject(gameObject);     // Attempt to increase the player's ammo, destroy self if successful
        }
    }

    public void SetActive(bool val)
    {
        active = val;
    }
}
