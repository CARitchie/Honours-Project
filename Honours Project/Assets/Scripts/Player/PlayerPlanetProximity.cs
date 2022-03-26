using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerPlanetProximity : MonoBehaviour
{
    [SerializeField] PlayerController player;
    [SerializeField] GravityReceiver ship;
    [SerializeField] GravitySource shipSource;
    [SerializeField] MeshRenderer forceField;
    [SerializeField] GravitySource morkald;

    Material mat;

    private void Awake()
    {
        mat = forceField.sharedMaterial;
    }


    private void FixedUpdate()
    {
        GravitySource source;
        if (player.gameObject.activeInHierarchy)
        {
            source = player.GetNearestSource();

            float inAtmosphere = 0;
            if (source != null)
            {
                inAtmosphere = source.HasAtmosphere() ? source.SoundPercent(player.transform.position) : 0;
            }
            AudioControl.AtmosphereInterpolation(inAtmosphere);

            if(source == shipSource)
            {
                ShipCheck();
            }
            else
            {
                SetForceFieldOpacity(source, inAtmosphere);
            }
            
        }
        else
        {
            ShipCheck();
        }
    }

    void ShipCheck()
    {
        GravitySource source = GravityController.FindClosest(ship);
        SetForceFieldOpacity(source, source != null && source.HasAtmosphere() ? source.SoundPercent(ship.transform.position) : 0);
    }

    void SetForceFieldOpacity(GravitySource source,float heightPercent)
    {
        if (source == morkald) return;

        Color colour = mat.color;
        colour.a = (1 - heightPercent) * 0.43f;
        mat.color = colour;
    }
}
