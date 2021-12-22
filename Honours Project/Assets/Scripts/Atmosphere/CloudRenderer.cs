using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode, ImageEffectAllowedInSceneView]
public class CloudRenderer : MonoBehaviour
{
    [SerializeField] Material material;
    [SerializeField] Transform sun;

    private void OnRenderImage(RenderTexture source, RenderTexture destination)
    {
        material.SetVector("_SunPosition", sun.position);

        Graphics.Blit(source, destination, material);
    }
}
