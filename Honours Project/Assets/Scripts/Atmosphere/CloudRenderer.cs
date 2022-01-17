using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode, ImageEffectAllowedInSceneView]
public class CloudRenderer : MonoBehaviour
{
    [SerializeField] Material material;
    [SerializeField] Transform sun;
    [SerializeField] Transform planet;
    [SerializeField] Vector2 weatherSpeed;

    static Vector2 offset;

    [ImageEffectOpaque]
    private void OnRenderImage(RenderTexture source, RenderTexture destination)
    {
        material.SetVector("_SunPosition", sun.position);
        material.SetVector("_PlanetPos", planet.position);

        offset += weatherSpeed * Time.deltaTime;
        material.SetVector("_WeatherOffset", offset);

        Graphics.Blit(source, destination, material);
    }
}
