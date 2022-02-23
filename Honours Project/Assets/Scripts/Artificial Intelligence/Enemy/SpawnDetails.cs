using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnDetails : MonoBehaviour
{
    [SerializeField] float teleportTime;
    [SerializeField] float noiseScale;

    private void Start()
    {
        Destroy(this, 10);
    }

    public float GetTeleportTime()
    {
        return teleportTime;
    }

    public float GetNoiseScale()
    {
        return noiseScale;
    }

}
