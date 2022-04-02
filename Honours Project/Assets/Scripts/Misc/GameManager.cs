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

    private void Start()
    {
        InputController.SetMap("Player");

        if(SaveManager.save != null)
        {
            Vector3 playerRelativePos = SaveManager.GetRelativePlayerPos();
            if (playerRelativePos == new Vector3(-450000, 0, 0)) return;

            GravitySource[] sources = FindObjectsOfType<GravitySource>();

            string source = SaveManager.GetGravitySource();
            if (source == "null") return;

            for (int i = 0; i < sources.Length;i++)
            {
                if(sources[i].Key == source)
                {
                    PlayerController.Instance.SetPosition(playerRelativePos + sources[i].transform.position);
                    PlayerController.Instance.SetAllRotation(SaveManager.save.player.GetLocalRot(), SaveManager.save.player.GetParentRot());
                    PlayerController.Instance.ForceVelocity(sources[i].GetVelocity());
                    break;
                }
            }     
        }
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
