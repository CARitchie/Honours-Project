using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SnowImprinter : MonoBehaviour
{
    [SerializeField] Shader snowPrint;
    [SerializeField] MeshRenderer snowRenderer;

    RenderTexture displacementMap;
    Material drawMat;
    Material snowMat;

    // Start is called before the first frame update
    void Start()
    {
        drawMat = new Material(snowPrint);
        drawMat.SetColor("_Color", Color.red);

        snowMat = snowRenderer.material;
        displacementMap = new RenderTexture(1024, 1024, 0, RenderTextureFormat.ARGBFloat);
        displacementMap.wrapMode = TextureWrapMode.Repeat;
        snowMat.SetTexture("_DispTex", displacementMap);
    }

    // Update is called once per frame
    void Update()
    {
        snowMat.SetVector("_Origin", snowRenderer.transform.position);

        if(Physics.Raycast(transform.position,-transform.up,out RaycastHit hit, 3, 1 << 8))
        {
            if (!hit.collider.CompareTag("Snow")) return;

            Vector3 point = (hit.point - snowRenderer.transform.position).normalized;

            drawMat.SetVector("_Coordinate", point);
            RenderTexture temp = RenderTexture.GetTemporary(1024, 1024, 0, RenderTextureFormat.ARGBFloat);
            Graphics.Blit(displacementMap, temp);
            Graphics.Blit(temp, displacementMap, drawMat);
            RenderTexture.ReleaseTemporary(temp);
        }
    }

    Vector3 squareToSphere(Vector2 pos)
    {
        float lat = pos.y  * 3.14159f - 3.14159f / 2;
        float lon = pos.x  * 2 * 3.14159f - 3.14159f;

        return new Vector3(Mathf.Cos(lat) * Mathf.Cos(lon), Mathf.Sin(lat), Mathf.Cos(lat) * Mathf.Sin(lon));
    }

    Vector2 SphereToSquare(Vector3 pos)
    {
        float lat = Mathf.Asin(pos.y);
        float lon = Mathf.Asin(pos.z / Mathf.Cos(lat));

        float y = lat / 3.14159f + 0.5f;
        float x = lon / (2 * 3.14159f) - 0.5f;
        return new Vector2(x, y);
    }
}
