using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HitMarker : MonoBehaviour
{
    public void SaveFromDeath()
    {
        gameObject.SetActive(false);
        transform.parent = GameManager.GetHitMarkerContainer();
    }
}
