using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HitMarker : MonoBehaviour
{
    // If the hitmarker is the child of an object about to be destroyed, change its parent so that it doesn't get destroyed
    public void SaveFromDeath()
    {
        //gameObject.SetActive(false);
        transform.parent = GameManager.GetHitMarkerContainer();
    }
}
