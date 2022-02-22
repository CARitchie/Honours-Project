using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnDetails : MonoBehaviour
{
    [SerializeField] float teleportTime;

    private void Start()
    {
        Destroy(this, 10);
    }

    public float GetTeleportTime()
    {
        return teleportTime;
    }

}
