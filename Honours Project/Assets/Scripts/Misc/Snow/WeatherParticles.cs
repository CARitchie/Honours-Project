using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeatherParticles : MonoBehaviour
{
    float speed;

    ParticleSystem particles;
    Transform planet;
    float height;

    ParticleSystem.VelocityOverLifetimeModule velocity;

    private void Awake()
    {
        particles = GetComponent<ParticleSystem>();
        planet = GetComponentInParent<PlanetGravity>().transform;
        height = Vector3.Distance(transform.position, planet.position);

        var main = particles.main;
        main.simulationSpace = ParticleSystemSimulationSpace.Custom;
        main.customSimulationSpace = planet;

        velocity = particles.velocityOverLifetime;

        speed = -GetComponentInParent<Clouds>().GetSpeed() * 360;           // Match the speed to that of the clouds

    }

    // Update is called once per frame
    void Update()
    {
        Vector3 dir = -(transform.position - planet.position) / height;
        Vector3 up = -dir;
        Vector3 right = Vector3.Cross(up, planet.up);
        Vector3 forward = Vector3.Cross(right, up);

        transform.rotation = Quaternion.LookRotation(forward, up);      // Rotate the particles so that they are always perpendicular to the surface, and pointing towards the north pole

        dir *= 2;

        // Update the particles velocity based on the new direction
        velocity.x = new ParticleSystem.MinMaxCurve(dir.x, dir.x * 2);
        velocity.y = new ParticleSystem.MinMaxCurve(dir.y, dir.y * 2);
        velocity.z = new ParticleSystem.MinMaxCurve(dir.z, dir.z * 2);

        transform.RotateAround(planet.position, planet.up, Time.deltaTime * speed);     // Move around the planet at the same speed as the clouds
    }
}
