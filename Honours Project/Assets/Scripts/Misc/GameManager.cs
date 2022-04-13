using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    [SerializeField] Material teleportMat;
    [SerializeField] ParticleSystem[] teleportFX;
    [SerializeField] Transform hitMarkerContainer;
    [SerializeField] GravitySource junglePlanet;
    public static GameManager Instance;

    bool saving = false;

    private void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
        InputController.SetMap("Player");
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

    public static void Autosave()
    {
        if (Instance == null) return;

        HUD.SpinSaveIcon(false);
        Instance.saving = true;
        Instance.StartCoroutine(Instance.RunAutosave());
    }

    IEnumerator RunAutosave()
    {
        while (!SaveManager.AttemptSave())
        {
            yield return new WaitForEndOfFrame();
        }

        HUD.StopSaving();

        saving = false;
    }
}
