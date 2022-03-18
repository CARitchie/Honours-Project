using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode, ImageEffectAllowedInSceneView]
public class AtmosphereRenderer : MonoBehaviour
{
    [SerializeField] Atmosphere[] atmospheres;
    [SerializeField] Transform sun;

    [ImageEffectOpaque]
    private void OnRenderImage(RenderTexture source, RenderTexture destination)
    {
        if (atmospheres == null || atmospheres.Length == 0) return;

        RenderTexture tempTexture = RenderTexture.GetTemporary(source.width, source.height, 0, source.format);

        Material mat;
        for(int i = 0; i < atmospheres.Length - 1; i++)
        {
            if (atmospheres[i] != null)
            {
                mat = atmospheres[i].GetMaterial();
                mat.SetVector("_LightOrigin", sun.position);
                Graphics.Blit(source, tempTexture, mat);
                Graphics.Blit(tempTexture, source);
            }
        }

        if (atmospheres[atmospheres.Length - 1] != null)
        {
            mat = atmospheres[atmospheres.Length - 1].GetMaterial();
            mat.SetVector("_LightOrigin", sun.position);
            Graphics.Blit(source, destination, mat);
        }

        RenderTexture.ReleaseTemporary(tempTexture);

    }
}
