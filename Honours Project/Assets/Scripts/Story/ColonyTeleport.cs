using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ColonyTeleport : MonoBehaviour
{
    [SerializeField] float teleportTime;
    [SerializeField] float noiseScale;
    [SerializeField] MeshRenderer[] brokenRenderers;
    [SerializeField] Material fixedMaterial;
    [SerializeField] ParticleSystem teleportFx;
    [SerializeField] GameObject[] disableThese;
    [SerializeField] GameObject interior;

    public void Teleport()
    {
        interior.SetActive(false);
        foreach(MeshRenderer renderer in brokenRenderers)
        {
            renderer.material = fixedMaterial;
        }

        StartCoroutine(FadeOut());
    }

    // Works the same as in EnemySpawnPoint.cs
    // Teleporting in/out should have been a separate component to reduce so much repetition of code
    IEnumerator FadeOut()
    {
        yield return new WaitForSeconds(1);

        ParticleSystem particles = teleportFx;
        particles.Play();

        var emission = particles.emission;
        var main = particles.main;
        var shape = particles.shape;
        main.customSimulationSpace = transform;

        ParticleSystem.MinMaxCurve initialLifetime = main.startLifetime;

        List<RendererMaterial> rendererMaterials = GetRendererMaterials(1);

        float timer = 0;
        while (timer < 0.75f)
        {
            particles.transform.up = transform.forward;
            timer += Time.deltaTime;
            yield return new WaitForEndOfFrame();
        }
        timer = 0;

        if (teleportTime == 0) teleportTime = 1.5f;

        while (timer < teleportTime)
        {
            float percent = 1 - timer / teleportTime;

            foreach (RendererMaterial renderer in rendererMaterials)
            {
                renderer.SetThreshold(percent);
            }

            emission.rateOverTime = (1 - percent) * (200 - 60) + 60;

            shape.angle = 3 + (20 - 3) * (percent - 0.1f);

            timer += Time.deltaTime;

            particles.transform.up = transform.forward;
            yield return new WaitForEndOfFrame();
        }

        particles.transform.parent = transform;

        yield return new WaitForSeconds(0.2f);

        foreach(GameObject gameObject in disableThese)
        {
            gameObject.SetActive(false);
        }

        particles.Stop();
        
    }

    List<RendererMaterial> GetRendererMaterials(float threshold)
    {
        Renderer[] renderers = GetComponentsInChildren<Renderer>(false);
        List<RendererMaterial> rendererMaterials = new List<RendererMaterial>();
        Material mat = GameManager.GetTeleportMaterial();
        for (int i = 0; i < renderers.Length; i++)
        {
            if (renderers[i].GetComponent<MeshRenderer>() || renderers[i].GetComponent<SkinnedMeshRenderer>())
            {
                rendererMaterials.Add(new RendererMaterial(renderers[i], mat, threshold, noiseScale));
            }
        }

        return rendererMaterials;
    }
}
