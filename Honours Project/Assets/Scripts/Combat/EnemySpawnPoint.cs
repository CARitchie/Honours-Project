using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemySpawnPoint : MonoBehaviour
{
    [SerializeField] float spawnDelay;
    [SerializeField] EnemyDetails enemy;
    [SerializeField] bool inScene = false;
    [SerializeField] bool teleportIn = false;

    float teleportTime;
    float noiseScale = 961.1f;

    EnemyWave wave;
    GravitySource centre;
    Vector3 offset = Vector3.zero;

    public void Spawn()
    {
        if (spawnDelay > 0) StartCoroutine(DelayedSpawn());
        else InstantSpawn();
    }

    IEnumerator DelayedSpawn()
    {
        // This should probably be replaced with yield return new WaitForSeconds(spawnDelay)
        // but I don't want to make any changes this close to submission in case anything breaks
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
        // Disable the enemy while they are fading in
        EnemyController controller = enemy.GetComponentInChildren<EnemyController>();
        enemy.SetImmune(true);
        controller.SetActive(false);

        ParticleSystem particles = GetParticles(0);
        particles.transform.localPosition += offset;
        particles.Play();

        // Retrieve the necessary components from the particle system
        var emission = particles.emission;
        var shape = particles.shape;
        var main = particles.main;
        main.customSimulationSpace = transform;

        float initialRadius = shape.radius;
        ParticleSystem.MinMaxCurve initialLifetime = main.startLifetime;

        List<RendererMaterial> rendererMaterials = GetRendererMaterials(0);

        yield return new WaitForSeconds(0.75f);

        float timer = 0;
        bool audioPlayed = false;
        while(timer < teleportTime)
        {
            float percent = timer / teleportTime;

            if(!audioPlayed && percent > 0.35f)
            {
                audioPlayed = true;
                particles.GetComponentInChildren<AudioSource>()?.Play();
            }

            foreach (RendererMaterial renderer in rendererMaterials)
            {
                renderer.SetThreshold(percent);
            }

            percent -= 0.2f;
            emission.rateOverTime = percent * (200 - 60) + 60;      // Change the emission rate depending upon the percentage
            shape.radius = (1 - percent) * initialRadius;           // Decrease the size of the particle spawn area with the percentage

            ParticleSystem.MinMaxCurve newLifetime = new ParticleSystem.MinMaxCurve(initialLifetime.constantMin * (1 - percent), initialLifetime.constantMax * (1.2f-percent));
            main.startLifetime = newLifetime;                       // Decrease the lifetime of the particles with the percentage

            timer += Time.deltaTime;
            yield return new WaitForEndOfFrame();
        }

        foreach (RendererMaterial renderer in rendererMaterials)
        {
            renderer.Restore();                                     // Give all meshes their original material
        }

        // Activate the enemy
        enemy.SetImmune(false);
        controller.SetActive(true);
        particles.Stop();
    }

    IEnumerator FadeOut()
    {
        yield return new WaitForSeconds(1);

        ParticleSystem particles = GetParticles(1);
        particles.Play();

        // Retrieve the necessary components from the particle system
        var emission = particles.emission;
        var main = particles.main;
        var shape = particles.shape;
        main.customSimulationSpace = transform;

        ParticleSystem.MinMaxCurve initialLifetime = main.startLifetime;

        List<RendererMaterial> rendererMaterials = GetRendererMaterials(1);

        // Play just the particles for 0.75 seconds
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

            emission.rateOverTime = (1 - percent) * (200 - 60) + 60;            // Decrease the emition rate with the percentage

            shape.angle = 3 + (20 - 3) * (percent - 0.1f);                      // The larger the percentage, the larger the angle

            timer += Time.deltaTime;

            particles.transform.up = GetParticleUp(particles.transform);
            yield return new WaitForEndOfFrame();
        }

        enemy.DestroyEnemy();                           // Despawn the enemy
        particles.transform.parent = transform;

        yield return new WaitForSeconds(0.2f);

        particles.Stop();
        
    }

    // Function to spawn the enemy
    public void InstantSpawn()
    {
        if (!inScene)                                                                                   // If the enemy isn't already in the scene
        {
            enemy = Instantiate(enemy, transform).GetComponent<EnemyDetails>();                         // Instantiate the enemy prefab
            enemy.GetComponentInChildren<EnemyController>()?.SetNearestSource(wave.GetSource());        // Tell the enemy which gravity source they are closest to
            enemy.gameObject.SetActive(false);
            enemy.transform.localPosition = Vector3.zero;
            enemy.transform.localEulerAngles = Vector3.zero;
        }
        enemy.SetWave(wave, this);                  // Tell the enemy which wave they are in
        enemy.gameObject.SetActive(true);
        enemy.GetComponentInChildren<Rigidbody>().AddForce(wave.GetVelocity(), ForceMode.VelocityChange);       // Match the enemey's velocity to that of the nearest gravity source

        // Retrieve the enemy's teleport details
        if(enemy.TryGetComponent(out SpawnDetails details))
        {
            teleportTime = details.GetTeleportTime();
            noiseScale = details.GetNoiseScale();
            offset = details.GetParticleOffset();
        }

        if (teleportIn) StartCoroutine(FadeIn());           // If the enemy is supposed to teleport in, start the fade in coroutine
    }

    public Vector3 GetParticleUp(Transform particles)
    {
        if(centre != null)
        {
            return centre.GetUp(particles.position);
        }
        else
        {
            return transform.up;
        }
    }

    public void SetWave(EnemyWave wave)
    {
        this.wave = wave;
        centre = wave.GetSource();
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

    // Function to retrieve the desired teleport particle system
    ParticleSystem GetParticles(int index)
    {
        GameObject particles = Instantiate(GameManager.GetTeleportFX(index).gameObject);
        particles.transform.parent = enemy.transform;
        particles.transform.localPosition = Vector3.zero;
        particles.transform.up = GetParticleUp(particles.transform);
        return particles.GetComponent<ParticleSystem>();
    }

    // Function to find all of the enemy's renderers and create a RendererMaterial for each
    List<RendererMaterial> GetRendererMaterials(float threshold)
    {
        Renderer[] renderers = enemy.GetComponentsInChildren<Renderer>();
        List<RendererMaterial> rendererMaterials = new List<RendererMaterial>();
        Material mat = GetMaterial();
        for (int i = 0; i < renderers.Length; i++)
        {
            if (renderers[i].GetComponent<MeshRenderer>() || renderers[i].GetComponent<SkinnedMeshRenderer>())
            {
                rendererMaterials.Add(new RendererMaterial(renderers[i], mat, threshold, noiseScale));              // Create a new RendererMaterial for the renderer
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

    public RendererMaterial(Renderer renderer, Material newMat, float threshold, float scale)
    {
        this.renderer = renderer;
        originalMat = renderer.material;
        this.renderer.material = newMat;
        CopyMaterialSettings(scale);
        SetThreshold(threshold);
    }

    // Function to copy the properties of the renderers default material into its teleport material
    void CopyMaterialSettings(float scale)
    {
        renderer.material.mainTexture = originalMat.mainTexture;
        renderer.material.SetTexture("_Emission", originalMat.GetTexture("_EmissionMap"));
        renderer.material.SetFloat("_Glossiness", originalMat.HasProperty("_Glossiness") ? originalMat.GetFloat("_Glossiness") : 0);
        renderer.material.SetFloat("_Scale", scale);
    }

    public void SetThreshold(float value)
    {
        renderer.material.SetFloat("_Threshold", value);
    }

    // Function to set the renderers material back to its original
    public void Restore()
    {
        renderer.material = originalMat;
    }
}
