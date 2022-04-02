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

    private void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
        InputController.SetMap("Player");

        ShipController ship = ShipController.Instance;
        ship.SetVelocity(junglePlanet.GetVelocity());

        if (SaveManager.save != null)
        {
            Vector3 playerRelativePos = SaveManager.GetRelativePlayerPos();
            if (playerRelativePos == new Vector3(-450000, 0, 0)) return;

            string key = SaveManager.GetGravitySource();
            if (key == "null") return;

            if(GravityController.FindSource(key, out GravitySource source))
            {
                PlayerController.Instance.SetPosition(playerRelativePos + source.transform.position);
                PlayerController.Instance.SetAllRotation(SaveManager.save.GetLocalRot(), SaveManager.save.GetParentRot());
                PlayerController.Instance.ForceVelocity(source.GetVelocity());
            }

            Vector3 shipRelativePos = SaveManager.GetRelativeShipPos();
            if(ship != null && shipRelativePos != new Vector3(-450000, 0, 0))
            {
                key = SaveManager.save.GetShipSource();
                if (key != "null" && GravityController.FindSource(key, out source))
                {
                    ship.transform.position = shipRelativePos + source.transform.position;
                    ship.transform.localEulerAngles = SaveManager.save.GetShipRot();
                    ship.SetVelocity(source.GetVelocity());
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
