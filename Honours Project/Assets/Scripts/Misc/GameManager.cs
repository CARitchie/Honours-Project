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
        // If the player reloads when in the ship, the controls can get stuck in the ship map
        // This guarantees that the player can still be in control once they've reloaded
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

    // Function to start the autosave process
    public static void Autosave()
    {
        if (Instance == null) return;

        HUD.SpinSaveIcon(false);                                // Start spinning the save icon
        Instance.saving = true;
        Instance.StartCoroutine(Instance.RunAutosave());        // Start the autosave coroutine
    }

    IEnumerator RunAutosave()
    {
        bool saved = false;
        while (!saved)                                      // Loop while the game hasn't saved
        {
            if (PlayerController.Instance.IsDead())         // Stop if the player dies
            {
                saved = true;
                break;
            }
            else
            {
                saved = SaveManager.AttemptSave();          // Make a save attempt
            }  
            yield return new WaitForEndOfFrame();
        }

        HUD.StopSaving();                                   // Stop spinning the save icon

        saving = false;
    }
}
