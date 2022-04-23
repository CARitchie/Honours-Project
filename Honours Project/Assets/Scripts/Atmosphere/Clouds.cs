using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Clouds : MonoBehaviour
{
    [SerializeField] Material mat;
    [SerializeField] float minRadius;
    [SerializeField] float maxRadius;
    [SerializeField] float globalCoverage = 1;
    [SerializeField] Color sunsetColour;
    [SerializeField] float sunsetStartAngle;
    [SerializeField] float sunsetEndAngle;
    [SerializeField] float darknessStartAngle;
    [SerializeField] float darknessFullAngle;
    [SerializeField] Vector2 weatherSpeed;

    Vector2 offset;

    private void Update()
    {
        // Increase the offset of the weather map, gives the illusion of moving clouds
        offset += weatherSpeed * Time.deltaTime;
    }

    // Retrieve the material and update its properties
    public Material GetMaterial()
    {
        mat.SetVector("_PlanetPos", transform.position);
        mat.SetVector("_WeatherOffset", offset);

        // These don't need to be updated every frame and so could be optimised
        mat.SetFloat("_MinHeight", minRadius);
        mat.SetFloat("_MaxHeight", maxRadius);
        mat.SetFloat("_Gc", globalCoverage);
        mat.SetColor("_SunColour", sunsetColour);
        mat.SetFloat("_StartSunSet", sunsetStartAngle);
        mat.SetFloat("_EndSunSet", sunsetEndAngle);
        mat.SetFloat("_StartDarkness", darknessStartAngle);
        mat.SetFloat("_EndDarkness", darknessFullAngle);

        return mat;
    }

    public float GetSpeed()
    {
        return weatherSpeed.x;
    }
}
