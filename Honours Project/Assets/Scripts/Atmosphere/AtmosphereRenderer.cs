using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode, ImageEffectAllowedInSceneView]
public class AtmosphereRenderer : MonoBehaviour
{
    [SerializeField] Atmosphere[] atmospheres;
    [SerializeField] Transform sun;

    Transform self;

    private void Start()
    {
        self = Camera.main.transform;
    }

    [ImageEffectOpaque]
    private void OnRenderImage(RenderTexture source, RenderTexture destination)
    {
        if (atmospheres == null || atmospheres.Length == 0) return;

        RenderTexture tempTexture = RenderTexture.GetTemporary(source.width, source.height, 0, source.format);

        Material mat;
        for(int i = 0; i < atmospheres.Length - 1; i++)
        {
            if (atmospheres[i] != null && AtmosVisible(atmospheres[i]))
            {
                mat = atmospheres[i].GetMaterial();
                mat.SetVector("_LightOrigin", sun.position);
                Graphics.Blit(source, tempTexture, mat);
                Graphics.Blit(tempTexture, source);
            }
        }

        if (atmospheres[atmospheres.Length - 1] != null && AtmosVisible(atmospheres[atmospheres.Length - 1]))
        {
            mat = atmospheres[atmospheres.Length - 1].GetMaterial();
            mat.SetVector("_LightOrigin", sun.position);
            Graphics.Blit(source, destination, mat);
        }else if(atmospheres.Length > 1)
        {
            Graphics.Blit(source, destination);
        }

        RenderTexture.ReleaseTemporary(tempTexture);

    }

    bool AtmosVisible(Atmosphere atmos)
    {
        if (self == null) return true;
        Vector3 toAtmos = atmos.transform.position - self.position;
        return Vector3.Dot(self.forward, toAtmos.normalized) > 0 || toAtmos.sqrMagnitude < 60000;
    }
}
