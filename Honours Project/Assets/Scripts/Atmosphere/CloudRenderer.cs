using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode, ImageEffectAllowedInSceneView]
public class CloudRenderer : MonoBehaviour
{
    [SerializeField] Transform sun;
    [SerializeField] Clouds[] clouds;

    Transform self;

    private void Start()
    {
        self = Camera.main.transform;
    }

    [ImageEffectOpaque]
    private void OnRenderImage(RenderTexture source, RenderTexture destination)
    {
        if (clouds == null || clouds.Length == 0) return;

        RenderTexture tempTexture = RenderTexture.GetTemporary(source.width, source.height, 0, source.format);

        Material mat;
        for (int i = 0; i < clouds.Length - 1; i++)
        {
            if (clouds[i] != null && CloudsVisible(clouds[i]))
            {
                mat = clouds[i].GetMaterial();
                mat.SetVector("_SunPosition", sun.position);
                Graphics.Blit(source, tempTexture, mat);
                Graphics.Blit(tempTexture, source);
            }
        }

        if (clouds[clouds.Length - 1] != null && CloudsVisible(clouds[clouds.Length - 1]))
        {
            mat = clouds[clouds.Length - 1].GetMaterial();
            mat.SetVector("_SunPosition", sun.position);
            Graphics.Blit(source, destination, mat);
        }else if(clouds.Length > 1)
        {
            Graphics.Blit(source, destination);
        }

        RenderTexture.ReleaseTemporary(tempTexture);

    }

    bool CloudsVisible(Clouds clouds)
    {
        if (self == null) return true;
        Vector3 toAtmos = clouds.transform.position - self.position;
        return Vector3.Dot(self.forward, toAtmos.normalized) > 0 || toAtmos.sqrMagnitude < 60000;
    }
}
