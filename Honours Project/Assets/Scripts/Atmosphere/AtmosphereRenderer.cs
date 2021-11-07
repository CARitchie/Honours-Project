using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode, ImageEffectAllowedInSceneView]
public class AtmosphereRenderer : MonoBehaviour
{
    [SerializeField] Material material;

    [SerializeField] Atmosphere atmosphere;
    [SerializeField] Transform sun;

    private void Awake()
    {
        atmosphere = FindObjectOfType<Atmosphere>();
    }

    private void OnRenderImage(RenderTexture source, RenderTexture destination)
    {
        if (atmosphere == null) return;

        material = atmosphere.ModifyMaterial(material);
        material.SetVector("_LightOrigin", sun.position);

        Graphics.Blit(source, destination, material);
    }
}
