using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
public class AtmosphereRenderer : MonoBehaviour
{
    [SerializeField] Material material;

    [SerializeField] Atmosphere atmosphere;

    private void Awake()
    {
        atmosphere = FindObjectOfType<Atmosphere>();
    }

    private void OnRenderImage(RenderTexture source, RenderTexture destination)
    {
        if (atmosphere == null) return;

        material = atmosphere.ModifyMaterial(material);

        Graphics.Blit(source, destination, material);
    }
}
