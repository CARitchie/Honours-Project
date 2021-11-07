using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Atmosphere : MonoBehaviour
{
    [SerializeField] float planetRadius;
    [SerializeField] float atmosphereRadius;
    [Range(1,500)] [SerializeField] int numberOfSteps;

    public Material ModifyMaterial(Material mat)
    {
        mat.SetVector("_PlanetOrigin", transform.position);
        mat.SetFloat("_AtmosphereRadius", atmosphereRadius);
        mat.SetInt("_NumberOfSteps", numberOfSteps);
        return mat;
    }
}
