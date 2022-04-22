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

    [ImageEffectOpaque]             // This tag makes this function run before opaque geometry is rendered, while not perfect it does mean that postprocessing will affect the clouds
    private void OnRenderImage(RenderTexture source, RenderTexture destination)
    {
        if (clouds == null || clouds.Length == 0) return;

        RenderTexture tempTexture = RenderTexture.GetTemporary(source.width, source.height, 0, source.format);          // Generate a temporary render texture

        Material mat;
        for (int i = 0; i < clouds.Length - 1; i++)                                                                     // For all but the last clouds
        {
            if (clouds[i] != null && CloudsVisible(clouds[i]))                                                          // If the clouds are not null and are visible
            {
                mat = clouds[i].GetMaterial();                                                                          // Retrieve the cloud's material
                mat.SetVector("_SunPosition", sun.position);                                                            // Set the position of the sun
                Graphics.Blit(source, tempTexture, mat);                                                                // Use the material to alter the source render texture
                Graphics.Blit(tempTexture, source);                                                                     // Put the altered render texture back into the source
            }
        }

        if (clouds[clouds.Length - 1] != null && CloudsVisible(clouds[clouds.Length - 1]))                              // If the last clouds are not null and are visible
        {
            mat = clouds[clouds.Length - 1].GetMaterial();
            mat.SetVector("_SunPosition", sun.position);
            Graphics.Blit(source, destination, mat);                                                                    // Use the material to alter the source render texture and send it to the destination
        }
        else if(clouds.Length > 1)
        {
            Graphics.Blit(source, destination);                                                                         // Otherwise, copy the source render texture into the destination
        }

        RenderTexture.ReleaseTemporary(tempTexture);                                                                    // Release the temporary render texture

    }

    // Function to determine whether clouds are visible
    bool CloudsVisible(Clouds clouds)
    {
        if (self == null) return true;
        Vector3 toAtmos = clouds.transform.position - self.position;
        return Vector3.Dot(self.forward, toAtmos.normalized) > 0 || toAtmos.sqrMagnitude < 60000;
    }
}
