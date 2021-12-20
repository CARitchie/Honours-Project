using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Atmosphere : MonoBehaviour
{
    [SerializeField] float planetRadius;
    [SerializeField] float atmosphereRadius;
    [Range(2,100)] [SerializeField] int numberOfSteps;
    [SerializeField] float strength;
    [SerializeField] float sunsetStrength;
    [SerializeField] Vector3 wavelengths = new Vector3(700,530,440);
    [SerializeField] float scatterStrength = 1;

    public Material ModifyMaterial(Material mat)
    {
        mat.SetVector("_PlanetOrigin", transform.position);
        mat.SetFloat("_PlanetRadius", planetRadius);
        mat.SetFloat("_AtmosphereRadius", atmosphereRadius);
        mat.SetInt("_NumberOfSteps", numberOfSteps);
        mat.SetFloat("_Strength", strength);
        mat.SetFloat("_SunsetStrength", sunsetStrength);

        float scatterR = Mathf.Pow(400 / wavelengths.x,4) * scatterStrength;
        float scatterG = Mathf.Pow(400 / wavelengths.y,4) * scatterStrength;
        float scatterB = Mathf.Pow(400 / wavelengths.z,4) * scatterStrength;
        Vector3 scatteringCoefficients = new Vector3(scatterR,scatterG,scatterB);
        mat.SetVector("scatteringCoefficients",scatteringCoefficients);

        return mat;
    }
}
