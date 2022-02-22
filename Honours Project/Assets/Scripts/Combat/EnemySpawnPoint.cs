using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemySpawnPoint : MonoBehaviour
{
    [SerializeField] float spawnDelay;
    [SerializeField] EnemyDetails enemy;
    [SerializeField] bool inScene = false;
    [SerializeField] bool teleportIn = false;
    EnemyWave wave;
    Transform centre;
    float teleportTime;

    public void Spawn()
    {
        if (spawnDelay > 0) StartCoroutine(DelayedSpawn());
        else InstantSpawn();
    }

    IEnumerator DelayedSpawn()
    {
        float timer = 0;
        while(timer < spawnDelay)
        {
            timer += Time.deltaTime;
            yield return new WaitForEndOfFrame();
        }
        InstantSpawn();
    }

    IEnumerator FadeIn()
    {
        EnemyController controller = enemy.GetComponentInChildren<EnemyController>();
        enemy.SetImmune(true);
        controller.SetActive(false);

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
        while(timer < teleportTime)
        {
            float percent = timer / teleportTime;

            foreach (RendererMaterial renderer in rendererMaterials)
            {
                renderer.SetThreshold(percent);
            }

            percent -= 0.2f;
            emission.rateOverTime = percent * (200 - 60) + 60;
            shape.radius = (1 - percent) * initialRadius;

            ParticleSystem.MinMaxCurve newLifetime = new ParticleSystem.MinMaxCurve(initialLifetime.constantMin * (1 - percent), initialLifetime.constantMax * (1.2f-percent));
            main.startLifetime = newLifetime;

            timer += Time.deltaTime;
            yield return new WaitForEndOfFrame();
        }

        foreach (RendererMaterial renderer in rendererMaterials)
        {
            renderer.Restore();
        }

        enemy.SetImmune(false);
        controller.SetActive(true);
        particles.Stop();
    }

    IEnumerator FadeOut()
    {
        yield return new WaitForSeconds(1);

        ParticleSystem particles = GetParticles(1);
        particles.Play();

        var emission = particles.emission;
        var main = particles.main;
        var shape = particles.shape;
        main.customSimulationSpace = transform;

        ParticleSystem.MinMaxCurve initialLifetime = main.startLifetime;

        List<RendererMaterial> rendererMaterials = GetRendererMaterials(1);

        float timer = 0;
        while(timer < 0.75f)
        {
            particles.transform.up = GetParticleUp(particles.transform);
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

            particles.transform.up = GetParticleUp(particles.transform);
            yield return new WaitForEndOfFrame();
        }

        Destroy(enemy.gameObject);
        particles.transform.parent = transform;

        yield return new WaitForSeconds(0.2f);

        particles.Stop();
        
    }

    public void InstantSpawn()
    {
        if (!inScene)
        {
            enemy = Instantiate(enemy, transform).GetComponent<EnemyDetails>();
            enemy.GetComponentInChildren<EnemyController>()?.SetNearestSource(wave.GetSource());
            enemy.gameObject.SetActive(false);
            enemy.transform.localPosition = Vector3.zero;
            enemy.transform.localEulerAngles = Vector3.zero;
        }
        enemy.SetWave(wave, this);
        enemy.gameObject.SetActive(true);
        enemy.GetComponentInChildren<Rigidbody>().AddForce(wave.GetVelocity(), ForceMode.VelocityChange);

        if(enemy.TryGetComponent(out SpawnDetails details))
        {
            teleportTime = details.GetTeleportTime();
        }

        if (teleportIn) StartCoroutine(FadeIn());
    }

    public Vector3 GetParticleUp(Transform particles)
    {
        return particles.position - centre.position;
    }

    public void SetWave(EnemyWave wave)
    {
        this.wave = wave;
        centre = wave.GetSource().transform;
    }

    public bool IsAlive()
    {
        return enemy != null && enemy.IsAlive();
    }

    public GravitySource GetSource()
    {
        return wave.GetSource();
    }

    Material GetMaterial()
    {
        return GameManager.GetTeleportMaterial();
    }

    ParticleSystem GetParticles(int index)
    {
        GameObject particles = Instantiate(GameManager.GetTeleportFX(index).gameObject);
        particles.transform.parent = enemy.transform;
        particles.transform.localPosition = Vector3.zero;
        particles.transform.up = GetParticleUp(particles.transform);
        return particles.GetComponent<ParticleSystem>();
    }

    List<RendererMaterial> GetRendererMaterials(float threshold)
    {
        Renderer[] renderers = enemy.GetComponentsInChildren<Renderer>();
        List<RendererMaterial> rendererMaterials = new List<RendererMaterial>();
        Material mat = GetMaterial();
        for (int i = 0; i < renderers.Length; i++)
        {
            if (renderers[i].GetComponent<MeshRenderer>() || renderers[i].GetComponent<SkinnedMeshRenderer>())
            {
                rendererMaterials.Add(new RendererMaterial(renderers[i], mat, threshold));
            }
        }

        return rendererMaterials;
    }

    public void OnDeath()
    {
        StartCoroutine(FadeOut());
    }
}

class RendererMaterial
{
    Renderer renderer;
    Material originalMat;

    public RendererMaterial(Renderer renderer, Material newMat, float threshold)
    {
        this.renderer = renderer;
        originalMat = renderer.material;
        this.renderer.material = newMat;
        CopyMaterialSettings();
        SetThreshold(threshold);
    }

    void CopyMaterialSettings()
    {
        renderer.material.mainTexture = originalMat.mainTexture;
        renderer.material.SetTexture("_Emission", originalMat.GetTexture("_EmissionMap"));
        renderer.material.SetFloat("_Glossiness", originalMat.GetFloat("_Glossiness"));
    }

    public void SetThreshold(float value)
    {
        renderer.material.SetFloat("_Threshold", value);
    }

    public void Restore()
    {
        renderer.material = originalMat;
    }
}
