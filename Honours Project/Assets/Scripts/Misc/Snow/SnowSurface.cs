using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Adapted from https://youtu.be/-yaqhzX-7qo
public class SnowSurface : MonoBehaviour
{
    [SerializeField] int resolution = 1024;
    [SerializeField] Shader shader;
    [SerializeField] MeshRenderer meshRenderer;

    Material snowMat;
    Material drawMat;
    RenderTexture displacementMap;

    List<SnowImprinter> imprinters = new List<SnowImprinter>();


    private void Awake()
    {
        drawMat = new Material(shader);
        drawMat.SetColor("_Color", Color.red);

        snowMat = meshRenderer.material;
        displacementMap = new RenderTexture(resolution, resolution, 0, RenderTextureFormat.ARGBFloat);
        displacementMap.wrapMode = TextureWrapMode.Repeat;
        snowMat.SetTexture("_DispTex", displacementMap);
    }

    // Update is called once per frame
    void Update()
    {
        snowMat.SetVector("_Origin", transform.position);

        for(int i = 0; i < imprinters.Count; i++)
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
        drawMat.SetFloat("_Size", size);
        RenderTexture temp = RenderTexture.GetTemporary(resolution, resolution, 0, RenderTextureFormat.ARGBFloat);
        Graphics.Blit(displacementMap, temp);
        Graphics.Blit(temp, displacementMap, drawMat);
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
