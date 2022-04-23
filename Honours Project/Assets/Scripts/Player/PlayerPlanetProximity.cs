using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerPlanetProximity : MonoBehaviour
{
    [SerializeField] bool active;
    [SerializeField] PlayerController player;
    [SerializeField] GravityReceiver ship;
    [SerializeField] GravitySource shipSource;
    [SerializeField] MeshRenderer forceField;
    [SerializeField] GravitySource morkald;

    Material mat;

    private void Awake()
    {
        mat = forceField.sharedMaterial;

        Color colour = mat.color;
        colour.a = 0.43f;
        mat.color = colour;

        if (!active) enabled = false;
    }


    private void FixedUpdate()
    {
        GravitySource source;
        if (player.gameObject.activeInHierarchy)
        {
            source = player.GetNearestSource();         // Find the player's closest gravity source

            float inAtmosphere = 0;
            if (source != null)
            {
                inAtmosphere = source.HasAtmosphere() ? source.SoundPercent(player.transform.position) : 0;     // Work out the player's height percentage within the atmosphere
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

    // Function to find the ship's height percentage rather than the player's
    void ShipCheck()
    {
        GravitySource source = GravityController.FindClosest(ship);
        SetForceFieldOpacity(source, source != null && source.HasAtmosphere() ? source.SoundPercent(ship.transform.position) : 0);
    }

    // Function to set the opacity of Morkald's forcefield
    // This is done because the forcefield looked strange when viewed from within an atmosphere
    void SetForceFieldOpacity(GravitySource source,float heightPercent)
    {
        if (source == morkald) return;

        Color colour = mat.color;
        colour.a = (1 - heightPercent) * 0.43f;
        mat.color = colour;
    }
}
