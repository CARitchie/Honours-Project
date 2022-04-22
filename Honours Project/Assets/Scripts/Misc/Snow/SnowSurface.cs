using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Adapted from https://youtu.be/-yaqhzX-7qo
public class SnowSurface : MonoBehaviour
{
    [SerializeField] int resolution = 1024;
    [SerializeField] Shader shader;
    [SerializeField] Shader snowFallShader;
    [SerializeField] MeshRenderer meshRenderer;
    [SerializeField] float flakeAmount;
    [SerializeField] float flakeOpacity;
    [SerializeField] float radius;

    Material snowMat;
    Material drawMat;
    Material fallMat;
    RenderTexture displacementMap;

    List<SnowImprinter> imprinters = new List<SnowImprinter>();
    SnowFall[] fallers;


    private void Awake()
    {
        drawMat = new Material(shader);             // Create the material to be used for drawing to the displacement texture
        drawMat.SetColor("_Color", Color.red);

        fallMat = new Material(snowFallShader);     // Create the material that reduces the displacement

        snowMat = meshRenderer.material;
        displacementMap = new RenderTexture(resolution, resolution, 0, RenderTextureFormat.ARGBFloat);      // Create the displacement map render texture
        displacementMap.wrapMode = TextureWrapMode.Repeat;
        snowMat.SetTexture("_DispTex", displacementMap);

        fallers = transform.parent.GetComponentsInChildren<SnowFall>();
    }

    // Update is called once per frame
    void Update()
    {
        snowMat.SetVector("_Origin", transform.position);

        if (QualitySettings.GetQualityLevel() < 2) return;

        if (fallers != null && fallers.Length > 0)          // For every object that adds snow
        {
            for (int i = 0; i < fallers.Length; i++)
            {
                AddSnow(fallers[i]);
            }
        }

        for (int i = 0; i < imprinters.Count; i++)          // For every object that leaves a trail
        {
            if(imprinters[i] != null)
            {
                if(imprinters[i].Contact(out Vector3 pos))  // If the object is making contact
                {
                    DrawToTexture(pos, imprinters[i].GetSize());
                }
            }
        }
    }

    void DrawToTexture(Vector3 contactPoint, float size)
    {
        Vector3 point = (contactPoint - transform.position).normalized;

        drawMat.SetVector("_Coordinate", point);
        drawMat.SetFloat("_Size", size * radius / 10);
        RenderTexture temp = RenderTexture.GetTemporary(resolution, resolution, 0, RenderTextureFormat.ARGBFloat);
        Graphics.Blit(displacementMap, temp);                   // Copy the displacement map into the temporary render texture
        Graphics.Blit(temp, displacementMap, drawMat);          // Copy the temporary render texture back into the displacement map, using the material that draws more displacement
        RenderTexture.ReleaseTemporary(temp);
    }

    void AddSnow(SnowFall faller)
    {
        Vector3 details = faller.GetDetails();

        fallMat.SetFloat("_RadiusSquare", Mathf.Pow(details.x / radius, 2));
        fallMat.SetFloat("_FlakeAmount", details.y);
        fallMat.SetFloat("_FlakeOpacity", details.z);
        fallMat.SetVector("_Origin", (faller.transform.position - transform.position) / radius);

        RenderTexture temp = RenderTexture.GetTemporary(resolution, resolution, 0, RenderTextureFormat.ARGBFloat);
        Graphics.Blit(displacementMap, temp);                   // Copy the displacement map into the temporary render texture
        Graphics.Blit(temp, displacementMap, fallMat);          // Copy the temporary render texture back into the displacement map, using the material that reduces the displacement
        RenderTexture.ReleaseTemporary(temp);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.attachedRigidbody == null) return;

        SnowImprinter[] imprints = other.attachedRigidbody.GetComponentsInChildren<SnowImprinter>(true);

        if (imprints == null || imprints.Length < 1) return;

        for (int i = 0; i < imprints.Length; i++)
        {
            if (!imprinters.Contains(imprints[i])) imprinters.Add(imprints[i]);     // Add all new imprinters into the list if they are not already present
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.attachedRigidbody == null) return;

        SnowImprinter[] imprints = other.attachedRigidbody.GetComponentsInChildren<SnowImprinter>(true);

        if (imprints == null || imprints.Length < 1) return;

        for (int i = 0; i < imprints.Length; i++)
        {
            if (imprinters.Contains(imprints[i])) imprinters.Remove(imprints[i]);   // Remove all of the exiting imprinters from the list
        }
    }
}
