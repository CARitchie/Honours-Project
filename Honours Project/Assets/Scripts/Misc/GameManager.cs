using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    [SerializeField] Material teleportMat;
    [SerializeField] ParticleSystem teleportFX;
    public static GameManager Instance;

    private void Awake()
    {
        Instance = this;
    }

    public static Material GetTeleportMaterial()
    {
        if (Instance == null) return null;
        return Instance.teleportMat;
    }

    public static ParticleSystem GetTeleportFX()
    {
        if (Instance == null) return null;
        return Instance.teleportFX;
    }
}
