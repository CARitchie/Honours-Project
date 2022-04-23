using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DamageIndicatorController : MonoBehaviour
{
    [SerializeField] Transform player;
    List<DamageIndicator> indicators = new List<DamageIndicator>();

    ObjectPool pool;

    private void Start()
    {
        pool = ObjectPool.GetPool("DamageIndicator");
    }

    public void AddIndicator(Transform newTarget)
    {
        if (!NotVisible(newTarget)) return;                                                 // Don't create a damage indicator if the damage causer is alredy visible

        for(int i = 0; i < indicators.Count; i++)
        {
            if (indicators[i].EqualTargets(newTarget)) return;                              // Find out if the damage causer already has a linked damage indicator
        }
        DamageIndicator newIndicator = pool.GetObject().GetComponent<DamageIndicator>();    // Get a new damage indicatorfrom the pool
        newIndicator.SetTarget(newTarget);                                                  // Link the indicator to the causer
        indicators.Add(newIndicator);
    }

    // Update is called once per frame
    void Update()
    {
        // Rotate all damage indicators towards their causers
        // Remove them from the list if they have become invisible
        for(int i = 0; i < indicators.Count; i++)
        {
            indicators[i].RotateTo(player);
            if (indicators[i].Fade())
            {
                indicators.RemoveAt(i);
                i--;
            }
        }
    }

    // Determine whether the player can see a transform
    public bool NotVisible(Transform target)
    {
        Vector3 dir = target.position - player.position;
        Vector3 right = Vector3.Cross(dir, player.up).normalized;
        dir = Vector3.Cross(-right, player.up);

        return Vector3.Dot(player.forward, dir) < 0.5f;
    }

}
