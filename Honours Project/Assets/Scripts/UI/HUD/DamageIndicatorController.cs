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
        if (!NotVisible(newTarget)) return;

        for(int i = 0; i < indicators.Count; i++)
        {
            if (indicators[i].EqualTargets(newTarget)) return;
        }
        DamageIndicator newIndicator = pool.GetObject().GetComponent<DamageIndicator>();
        newIndicator.SetTarget(newTarget);
        indicators.Add(newIndicator);
    }

    // Update is called once per frame
    void Update()
    {
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

    public bool NotVisible(Transform target)
    {
        Vector3 dir = target.position - player.position;
        Vector3 right = Vector3.Cross(dir, player.up).normalized;
        dir = Vector3.Cross(-right, player.up);

        return Vector3.Dot(player.forward, dir) < 0.5f;
    }

}
