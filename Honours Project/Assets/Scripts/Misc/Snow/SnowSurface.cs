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
        drawMat = new Material(shader);
        drawMat.SetColor("_Color", Color.red);

        fallMat = new Material(snowFallShader);

        snowMat = meshRenderer.material;
        displacementMap = new RenderTexture(resolution, resolution, 0, RenderTextureFormat.ARGBFloat);
        displacementMap.wrapMode = TextureWrapMode.Repeat;
        snowMat.SetTexture("_DispTex", displacementMap);

        fallers = transform.parent.GetComponentsInChildren<SnowFall>();
    }

    // Update is called once per frame
    void Update()
    {
        snowMat.SetVector("_Origin", transform.position);

        if (QualitySettings.GetQualityLevel() < 2) return;

        if (fallers != null && fallers.Length > 0)
        {
            for (int i = 0; i < fallers.Length; i++)
            {
                AddSnow(fallers[i]);
            }
        }

        for (int i = 0; i < imprinters.Count; i++)
        {
            if(imprinters[i] != null)
            {
                if(imprinters[i].Contact(out Vector3 pos))
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
        Graphics.Blit(displacementMap, temp);
        Graphics.Blit(temp, displacementMap, drawMat);
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
        Graphics.Blit(displacementMap, temp);
        Graphics.Blit(temp, displacementMap, fallMat);
        RenderTexture.ReleaseTemporary(temp);
    }

    private void OnTriggerEnter(Collider other)
    {
        SnowImprinter[] imprints = other.attachedRigidbody.GetComponentsInChildren<SnowImprinter>(true);

        if (imprints == null || imprints.Length < 1) return;

        for (int i = 0; i < imprints.Length; i++)
        {
            if (!imprinters.Contains(imprints[i])) imprinters.Add(imprints[i]);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        SnowImprinter[] imprints = other.attachedRigidbody.GetComponentsInChildren<SnowImprinter>(true);

        if (imprints == null || imprints.Length < 1) return;

        for (int i = 0; i < imprints.Length; i++)
        {
            if (imprinters.Contains(imprints[i])) imprinters.Remove(imprints[i]);
        }
    }
}
