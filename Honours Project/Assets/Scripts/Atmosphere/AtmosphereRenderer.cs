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

    [ImageEffectOpaque]         // This tag makes this function run before opaque geometry is rendered, while not perfect it does mean that postprocessing will affect the atmospheres
    private void OnRenderImage(RenderTexture source, RenderTexture destination)
    {
        if (atmospheres == null || atmospheres.Length == 0) return;

        RenderTexture tempTexture = RenderTexture.GetTemporary(source.width, source.height, 0, source.format);          // Generate a temporary render texture

        Material mat;
        for(int i = 0; i < atmospheres.Length - 1; i++)                                                                 // For all but the last atmosphere
        {
            if (atmospheres[i] != null && AtmosVisible(atmospheres[i]))                                                 // If the atmosphere is not null and is visible
            {
                mat = atmospheres[i].GetMaterial();                                                                     // Retrieve the atmosphere's material
                mat.SetVector("_LightOrigin", sun.position);                                                            // Set the position of the sun
                Graphics.Blit(source, tempTexture, mat);                                                                // Use the material to alter the source render texture
                Graphics.Blit(tempTexture, source);                                                                     // Put the altered render texture back into the source
            }
        }

        if (atmospheres[atmospheres.Length - 1] != null && AtmosVisible(atmospheres[atmospheres.Length - 1]))           // If the last atmosphere is not null and is visible
        {
            mat = atmospheres[atmospheres.Length - 1].GetMaterial();
            mat.SetVector("_LightOrigin", sun.position);
            Graphics.Blit(source, destination, mat);                                                                    // Use the material to alter the source render texture and send it to the destination
        }else if(atmospheres.Length > 1)
        {
            Graphics.Blit(source, destination);                                                                         // Otherwise, copy the source render texture into the destination
        }

        RenderTexture.ReleaseTemporary(tempTexture);                                                                    // Release the temporary render texture

    }

    // Function to determine whether an atmosphere is visible
    bool AtmosVisible(Atmosphere atmos)
    {
        if (self == null) return true;
        Vector3 toAtmos = atmos.transform.position - self.position;
        return Vector3.Dot(self.forward, toAtmos.normalized) > 0 || toAtmos.sqrMagnitude < 60000;
    }
}
