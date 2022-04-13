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
        offset += weatherSpeed * Time.deltaTime;
    }

    public Material GetMaterial()
    {
        mat.SetVector("_PlanetPos", transform.position);

        // These don't need to be updated every frame
        mat.SetFloat("_MinHeight", minRadius);
        mat.SetFloat("_MaxHeight", maxRadius);
        mat.SetFloat("_Gc", globalCoverage);
        mat.SetColor("_SunColour", sunsetColour);
        mat.SetFloat("_StartSunSet", sunsetStartAngle);
        mat.SetFloat("_EndSunSet", sunsetEndAngle);
        mat.SetFloat("_StartDarkness", darknessStartAngle);
        mat.SetFloat("_EndDarkness", darknessFullAngle);

        mat.SetVector("_WeatherOffset", offset);

        return mat;
    }

    public float GetSpeed()
    {
        return weatherSpeed.x;
    }
}
