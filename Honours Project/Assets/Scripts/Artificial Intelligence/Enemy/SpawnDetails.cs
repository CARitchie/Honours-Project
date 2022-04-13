using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnDetails : MonoBehaviour
{
    [SerializeField] float teleportTime;
    [SerializeField] float noiseScale;
    [SerializeField] Vector3 particleOffset;

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

    public Vector3 GetParticleOffset()
    {
        return particleOffset;
    }

}
