using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemySpawnPoint : MonoBehaviour
{
    [SerializeField] float spawnDelay;
    [SerializeField] EnemyDetails enemy;
    [SerializeField] bool inScene = false;
    [SerializeField] bool fadeIn = false;
    [SerializeField] float teleportTime;
    EnemyWave wave;

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
        EnemyDetails details = enemy.GetComponent<EnemyDetails>();
        EnemyController controller = enemy.GetComponentInChildren<EnemyController>();
        details.SetImmune(true);
        controller.SetActive(false);

        ParticleSystem particles = GetParticles();
        particles.transform.parent = enemy.transform;
        particles.transform.localPosition = Vector3.zero;
        particles.Play();

        var emission = particles.emission;
        var shape = particles.shape;
        var main = particles.main;
        main.customSimulationSpace = transform;

        float initialRadius = shape.radius;
        ParticleSystem.MinMaxCurve initialLifetime = main.startLifetime;

        MeshRenderer[] renderers = enemy.GetComponentsInChildren<MeshRenderer>();
        MeshMaterial[] meshMaterials = new MeshMaterial[renderers.Length];
        Material mat = GetMaterial();
        for(int i = 0; i < meshMaterials.Length; i++)
        {
            meshMaterials[i] = new MeshMaterial(renderers[i], mat);
        }

        yield return new WaitForSeconds(0.75f);

        float timer = 0;
        while(timer < teleportTime)
        {
            float percent = timer / teleportTime;

            foreach (MeshMaterial renderer in meshMaterials)
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

        foreach (MeshMaterial renderer in meshMaterials)
        {
            renderer.Restore();
        }

        details.SetImmune(false);
        controller.SetActive(true);
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
        enemy.SetWave(wave);
        enemy.gameObject.SetActive(true);
        enemy.GetComponentInChildren<Rigidbody>().AddForce(wave.GetVelocity(), ForceMode.VelocityChange);

        if (fadeIn) StartCoroutine(FadeIn());
    }

    public void SetWave(EnemyWave wave)
    {
        this.wave = wave;
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

    ParticleSystem GetParticles()
    {
        GameObject particles = Instantiate(GameManager.GetTeleportFX().gameObject);
        return particles.GetComponent<ParticleSystem>();
    }
}

class MeshMaterial
{
    MeshRenderer renderer;
    Material originalMat;

    public MeshMaterial(MeshRenderer renderer, Material newMat)
    {
        this.renderer = renderer;
        originalMat = renderer.material;
        this.renderer.material = newMat;
        this.renderer.material.mainTexture = originalMat.mainTexture;
        SetThreshold(0);
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
