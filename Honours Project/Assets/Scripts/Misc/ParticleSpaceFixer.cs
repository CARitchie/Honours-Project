using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ParticleSpaceFixer : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        ParticleSystem particles = GetComponent<ParticleSystem>();
        EnemyController controller = GetComponentInParent<EnemyController>();

        var main = particles.main;
        main.customSimulationSpace = controller.GetNearestSource().transform;       // Make the particle's simulation space the enemy's nearest gravity source
        Destroy(this);
    }
}
