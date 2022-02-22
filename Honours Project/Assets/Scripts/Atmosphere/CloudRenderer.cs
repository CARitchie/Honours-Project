using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode, ImageEffectAllowedInSceneView]
public class CloudRenderer : MonoBehaviour
{
    [SerializeField] Transform sun;
    [SerializeField] Clouds[] clouds;

    [ImageEffectOpaque]
    private void OnRenderImage(RenderTexture source, RenderTexture destination)
    {
        if (clouds == null || clouds.Length == 0) return;

        RenderTexture tempTexture = RenderTexture.GetTemporary(source.width, source.height, 0, source.format);

        Material mat;
        for (int i = 0; i < clouds.Length - 1; i++)
        {
            if (clouds[i] != null)
            {
                mat = clouds[i].GetMaterial();
                mat.SetVector("_SunPosition", sun.position);
                Graphics.Blit(source, tempTexture, mat);

                source = tempTexture;
            }
        }

        if (clouds[clouds.Length - 1] != null)
        {
            mat = clouds[clouds.Length - 1].GetMaterial();
            mat.SetVector("_SunPosition", sun.position);
            Graphics.Blit(source, destination, mat);
        }

        RenderTexture.ReleaseTemporary(tempTexture);

    }
}
