using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DamageIndicator : PoolObject
{
    [SerializeField] Image image;
    Transform target;
    float timer;

    protected override void Start()
    {
    }

    public override void OnExitQueue()
    {
        base.OnExitQueue();

        ResetTimer();
    }

    public void SetTarget(Transform target)
    {
        gameObject.SetActive(true);
        this.target = target;
    }

    public void RotateTo(Transform player)
    {
        if (target == null) return;
        Vector3 dir = target.position - player.position;
        Vector3 right = Vector3.Cross(dir, player.up).normalized;
        dir = Vector3.Cross(-right, player.up);

        float multi = Vector3.Dot(player.right, dir) > 0 ? 1 : -1;
        transform.localEulerAngles = Vector3.forward * (Vector3.Dot(player.forward, dir) * 90 - 90) * multi;
    }

    public bool EqualTargets(Transform otherTarget)
    {
        if(otherTarget == target)
        {
            ResetTimer();
            return true;
        }
        return false;
    }

    public bool Fade()
    {
        timer -= Time.deltaTime;
        Color colour = image.color;
        colour.a -= Time.deltaTime / 3;
        image.color = colour;

        if(timer <= 0)
        {
            gameObject.SetActive(false);
            return true;
        }
        
        return false;
    }

    void ResetTimer()
    {
        timer = 3;
        Color colour = image.color;
        colour.a = 1;
        image.color = colour;
    }
}
