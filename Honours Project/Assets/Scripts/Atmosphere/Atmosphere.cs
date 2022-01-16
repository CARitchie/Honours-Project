using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Atmosphere : MonoBehaviour
{
    [SerializeField] float planetRadius;
    [SerializeField] float atmosphereRadius;

    public Material ModifyMaterial(Material mat)
    {
        mat.SetVector("_PlanetOrigin", transform.position);
        mat.SetFloat("_PlanetRadius", planetRadius);
        mat.SetFloat("_AtmosphereRadius", atmosphereRadius);

        return mat;
    }
}
