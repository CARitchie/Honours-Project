using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SummonedAmmo : MonoBehaviour
{
    [SerializeField] float teleportTime;
    [SerializeField] float noiseScale;

    public void StartSpawn()
    {
        StartCoroutine(FadeIn());
    }

    public void OnSpawned()
    {
        foreach(GravityReceiver receiver in GetComponentsInChildren<GravityReceiver>(true))
        {
            receiver.enabled = true;
        }

        foreach (Rigidbody rb in GetComponentsInChildren<Rigidbody>(true))
        {
            rb.isKinematic = false;
        }

        foreach (AmmoItem ammo in GetComponentsInChildren<AmmoItem>(true))
        {
            ammo.SetActive(true);
        }
    }

    IEnumerator FadeIn()
    {
        ParticleSystem particles = GetParticles(0);
        particles.Play();

        var emission = particles.emission;
        var shape = particles.shape;
        var main = particles.main;
        main.customSimulationSpace = transform;

        float initialRadius = shape.radius;
        ParticleSystem.MinMaxCurve initialLifetime = main.startLifetime;

        List<RendererMaterial> rendererMaterials = GetRendererMaterials(0);

        yield return new WaitForSeconds(0.75f);

        float timer = 0;
        while (timer < teleportTime)
        {
            float percent = timer / teleportTime;

            foreach (RendererMaterial renderer in rendererMaterials)
            {
                renderer.SetThreshold(percent);
            }

            percent -= 0.2f;
            emission.rateOverTime = percent * (200 - 60) + 60;
            shape.radius = (1 - percent) * initialRadius;

            ParticleSystem.MinMaxCurve newLifetime = new ParticleSystem.MinMaxCurve(initialLifetime.constantMin * (1 - percent), initialLifetime.constantMax * (1.2f - percent));
            main.startLifetime = newLifetime;

            timer += Time.deltaTime;
            yield return new WaitForEndOfFrame();
        }

        foreach (RendererMaterial renderer in rendererMaterials)
        {
            renderer.Restore();
        }
        particles.Stop();

        OnSpawned();
    }

    ParticleSystem GetParticles(int index)
    {
        GameObject particles = Instantiate(GameManager.GetTeleportFX(index).gameObject);
        particles.transform.parent = transform;
        particles.transform.localPosition = Vector3.zero + Vector3.up * 0.3f;
        particles.transform.up = transform.up;
        return particles.GetComponent<ParticleSystem>();
    }

    List<RendererMaterial> GetRendererMaterials(float threshold)
    {
        Renderer[] renderers = GetComponentsInChildren<Renderer>();
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
