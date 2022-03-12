using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    [SerializeField] Material teleportMat;
    [SerializeField] ParticleSystem[] teleportFX;
    [SerializeField] Transform hitMarkerContainer;
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

    public static ParticleSystem GetTeleportFX(int index)
    {
        if (Instance == null) return null;
        return Instance.teleportFX[index];
    }

    public static Transform GetHitMarkerContainer()
    {
        return Instance == null ? null : Instance.hitMarkerContainer;
    }

    public void LoadScene(string scene)
    {
        UnityEngine.SceneManagement.SceneManager.LoadScene(scene);
    }
}
